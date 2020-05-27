using MongoDB.Driver;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DocumentDb;

namespace MongoDb
{
    internal sealed class WriteRepository<TDocument> : ReadRepository<TDocument>, IWriteRepository<TDocument> where TDocument : Document
    {
        public WriteRepository(IMongoCollection<TDocument> collection) : base(collection) { }

        public void Create(TDocument document)
        {
            document = SetTimestampsForCreate(document);
            Collection.InsertOne(document);
        }

        public async Task CreateAsync(TDocument document)
        {
            document = SetTimestampsForCreate(document);
            await Collection.InsertOneAsync(document);
        }

        public void Update(TDocument document)
        {
            document = SetTimestampForUpdate(document);
            Collection.ReplaceOne(x => x.Id == document.Id, document);
        }

        public async Task<DocumentDb.UpdateResult> UpdateAsync(TDocument document)
        {
            document = SetTimestampForUpdate(document);
            var result = await Collection.ReplaceOneAsync(x => x.Id == document.Id, document);
            return new DocumentDb.UpdateResult
            {
                IsAcknowledged = result.IsAcknowledged,
                MatchedCount = result.MatchedCount,
                ModifiedCount = result.ModifiedCount
            };
        }

        public void UpdatePartial(string id, object updateObject)
        {
            var filter = GetUpdatePartialFilterDefinition(id);
            UpdateDefinition<TDocument> updateDefinition = GetUpdatePartialUpdateDefinition(updateObject);
            UpdateOptions updateOptions = GetUpdatePartialOptions();
            Collection.UpdateOne(filter, updateDefinition, updateOptions);
        }

        public async Task<DocumentDb.UpdateResult> UpdatePartialAsync(string id, object updateObject)
        {
            var filter = GetUpdatePartialFilterDefinition(id);
            UpdateDefinition<TDocument> updateDefinition = GetUpdatePartialUpdateDefinition(updateObject);
            UpdateOptions updateOptions = GetUpdatePartialOptions();
            var result = await Collection.UpdateOneAsync(filter, updateDefinition, updateOptions);
            return new DocumentDb.UpdateResult
            {
                IsAcknowledged = result.IsAcknowledged,
                MatchedCount = result.MatchedCount,
                ModifiedCount = result.ModifiedCount
            };
        }

        public void Delete(string id)
        {
            Collection.DeleteOne(x => x.Id == id);
        }

        public async Task<DocumentDb.DeleteResult> DeleteAsync(string id)
        {
            var builder = Builders<TDocument>.Filter;
            var filter = builder.Eq("_id", id);
            var result = await Collection.DeleteOneAsync(filter);
            return new DocumentDb.DeleteResult
            {
                IsAcknowledged = result.IsAcknowledged,
                DeletedCount = result.DeletedCount
            };
        }

        public async Task<DocumentDb.DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> predicate)
        {
            var filter = Builders<TDocument>.Filter.Where(predicate);
            var result = await Collection.DeleteManyAsync(filter);
            return new DocumentDb.DeleteResult
            {
                IsAcknowledged = result.IsAcknowledged,
                DeletedCount = result.DeletedCount
            };
        }

        public bool AddToSet<TItem>(string documentId, string setName, TItem item)
        {
            var filter = Builders<TDocument>.Filter.Eq("_id", documentId);
            var updateDefinition = Builders<TDocument>.Update.AddToSet(setName, item);
            var updateResult = Collection.UpdateOne(filter, updateDefinition);
            return updateResult.ModifiedCount > 0;
        }

        public async Task<bool> AddToSetAsync<TItem>(string documentId, string setName, TItem item)
        {
            var filter = Builders<TDocument>.Filter.Eq("_id", documentId);
            var updateDefinition = Builders<TDocument>.Update.AddToSet(setName, item);
            var updateResult = await Collection.UpdateOneAsync(filter, updateDefinition);
            return updateResult.ModifiedCount > 0;
        }

        private static UpdateDefinition<TDocument> GetUpdatePartialUpdateDefinition(object updateObject)
        {
            var updateProperties = updateObject.GetType().GetRuntimeProperties();
            var updateBuilder = Builders<TDocument>
                .Update;
            UpdateDefinition<TDocument> updateDefinition = null;
            foreach (var propertyInfo in updateProperties)
            {
                var value = propertyInfo.GetValue(updateObject);
                if (value is Array || value is ICollection || value is IList)
                {
                    //Collections are not supported
                    throw new NotImplementedException("Collections are not supported");
                }

                var fullName = propertyInfo.Name.Replace('_', '.');
                var propertyTree = fullName.Split('.');
                PropertyInfo currentProperty = null;
                for (var treeDepthIndex = 0; treeDepthIndex < propertyTree.Count(); treeDepthIndex++)
                {
                    if (currentProperty == null)
                    {
                        if (!TryGetPropertyInfo(typeof(TDocument), propertyTree[treeDepthIndex], out currentProperty))
                        {
                            throw new Exception("No matching property found for the partial update.");
                        }
                    }
                    else
                    {
                        if (!TryGetPropertyInfo(currentProperty.GetType(), propertyTree[treeDepthIndex], out currentProperty))
                        {
                            throw new Exception("No matching property found for the partial update.");
                        }
                    }
                }
                if (updateDefinition == null)
                {
                    updateDefinition = updateBuilder.Set(fullName, value);
                }
                else
                {
                    updateDefinition = updateDefinition.Set(fullName, value);
                }
            }

            bool TryGetPropertyInfo(Type parentType, string propertyName, out PropertyInfo propertyInfo)
            {
                var documentPropertyNames = parentType.GetRuntimeProperties().ToList();
                propertyInfo = documentPropertyNames.FirstOrDefault(m => m.Name == propertyName);
                return propertyInfo != null;
            }

            updateDefinition = updateDefinition.Set(x => x.UpdatedAtUtc, DateTime.UtcNow);

            return updateDefinition;
        }

        private static FilterDefinition<TDocument> GetUpdatePartialFilterDefinition(string id)
        {
            var filter = Builders<TDocument>.Filter.Eq("_id", id);

            return filter;
        }

        private static UpdateOptions GetUpdatePartialOptions()
        {
            return new UpdateOptions()
            {
                IsUpsert = false
            };
        }

        private static TDocument SetTimestampsForCreate(TDocument document)
        {
            var timestamp = DateTime.UtcNow;

            document.CreatedAtUtc = timestamp;
            document.UpdatedAtUtc = timestamp;

            return document;
        }

        private static TDocument SetTimestampForUpdate(TDocument document)
        {
            var timestamp = DateTime.UtcNow;

            document.UpdatedAtUtc = timestamp;

            return document;
        }
    }
}
