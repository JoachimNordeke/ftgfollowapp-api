using System;
using Microsoft.AspNetCore.Mvc;
using MongoDb;
using DocumentDb;

namespace API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public readonly IDocumentDatabase DocumentDatabase;

        public BaseController()
        {
            var mongoDbConnection = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_DEVELOPMENT");
            DocumentDatabase = new MongoDbStorage(mongoDbConnection, "Tele2");
        }
    }
}
