using System;
using System.Linq;
using System.Collections.Generic;
using API.Models.Users;

namespace API.Factories
{
    public static class UserFactory
    {
        public static User CreateUser(NewUpdateUser newUser)
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = newUser.Email,
                Firstname = newUser.Firstname,
                Lastname = newUser.Lastname,
                Phone = newUser.Phone,
                MainStoreId = newUser.MainStoreId,
                StoreIds = newUser.StoreIds != null && newUser.StoreIds.Count() > 0 ? newUser.StoreIds : new List<string>(),
                IsPasswordReset = false,
                Role = newUser.Role,
                IsActive = false
            };
        }

        public static UserDTO CreateUserDTOFromUser(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Phone = user.Phone,
                Role = user.Role,
                MainStoreId = user.MainStoreId,
                StoreIds = user.StoreIds,
                IsPasswordReset = user.IsPasswordReset,
                IsActive = user.IsActive,
                CreatedAtUtc = user.CreatedAtUtc,
                UpdatedAtUtc = user.UpdatedAtUtc
            };
        }

        public static User UpdateUser(User user, NewUpdateUser updateUser)
        {
            user.Firstname = updateUser.Firstname;
            user.Lastname = updateUser.Lastname;
            user.Email = updateUser.Email;
            user.Phone = updateUser.Phone;
            user.MainStoreId = updateUser.MainStoreId;
            user.Role = updateUser.Role;
            user.StoreIds = updateUser.StoreIds;

            return user;
        }
    }
}
