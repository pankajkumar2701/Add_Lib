using Add_Lib.Models;
using Add_Lib.Data;
using Add_Lib.Filter;
using Add_Lib.Entities;
using Add_Lib.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace Add_Lib.Services
{
    /// <summary>
    /// The userService responsible for managing user related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting user information.
    /// </remarks>
    public interface IUserService
    {
        /// <summary>Retrieves a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <returns>The user data</returns>
        User GetById(Guid id);

        /// <summary>Retrieves a list of users based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of users</returns>
        List<User> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new user</summary>
        /// <param name="model">The user data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(User model);

        /// <summary>Updates a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <param name="updatedEntity">The user data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, User updatedEntity);

        /// <summary>Updates a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <param name="updatedEntity">The user data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<User> updatedEntity);

        /// <summary>Deletes a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The userService responsible for managing user related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting user information.
    /// </remarks>
    public class UserService : IUserService
    {
        private Add_LibContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the User class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public UserService(Add_LibContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <returns>The user data</returns>
        public User GetById(Guid id)
        {
            var entityData = _dbContext.User.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of users based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of users</returns>/// <exception cref="Exception"></exception>
        public List<User> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetUser(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new user</summary>
        /// <param name="model">The user data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(User model)
        {
            model.Id = CreateUser(model);
            return model.Id;
        }

        /// <summary>Updates a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <param name="updatedEntity">The user data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, User updatedEntity)
        {
            UpdateUser(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <param name="updatedEntity">The user data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<User> updatedEntity)
        {
            PatchUser(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific user by its primary key</summary>
        /// <param name="id">The primary key of the user</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteUser(id);
            return true;
        }
        #region
        private List<User> GetUser(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.User.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<User>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(User), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<User, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = result.Skip(skip).Take(pageSize).ToList();
            return paginatedResult;
        }

        private Guid CreateUser(User model)
        {
            _dbContext.User.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateUser(Guid id, User updatedEntity)
        {
            _dbContext.User.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteUser(Guid id)
        {
            var entityData = _dbContext.User.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.User.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchUser(Guid id, JsonPatchDocument<User> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.User.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.User.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}