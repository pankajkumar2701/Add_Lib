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
    /// The userinroleService responsible for managing userinrole related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting userinrole information.
    /// </remarks>
    public interface IUserInRoleService
    {
        /// <summary>Retrieves a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <returns>The userinrole data</returns>
        UserInRole GetById(Guid id);

        /// <summary>Retrieves a list of userinroles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of userinroles</returns>
        List<UserInRole> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new userinrole</summary>
        /// <param name="model">The userinrole data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(UserInRole model);

        /// <summary>Updates a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <param name="updatedEntity">The userinrole data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, UserInRole updatedEntity);

        /// <summary>Updates a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <param name="updatedEntity">The userinrole data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<UserInRole> updatedEntity);

        /// <summary>Deletes a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The userinroleService responsible for managing userinrole related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting userinrole information.
    /// </remarks>
    public class UserInRoleService : IUserInRoleService
    {
        private Add_LibContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the UserInRole class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public UserInRoleService(Add_LibContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <returns>The userinrole data</returns>
        public UserInRole GetById(Guid id)
        {
            var entityData = _dbContext.UserInRole.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of userinroles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of userinroles</returns>/// <exception cref="Exception"></exception>
        public List<UserInRole> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetUserInRole(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new userinrole</summary>
        /// <param name="model">The userinrole data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(UserInRole model)
        {
            model.Id = CreateUserInRole(model);
            return model.Id;
        }

        /// <summary>Updates a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <param name="updatedEntity">The userinrole data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, UserInRole updatedEntity)
        {
            UpdateUserInRole(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <param name="updatedEntity">The userinrole data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<UserInRole> updatedEntity)
        {
            PatchUserInRole(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific userinrole by its primary key</summary>
        /// <param name="id">The primary key of the userinrole</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteUserInRole(id);
            return true;
        }
        #region
        private List<UserInRole> GetUserInRole(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.UserInRole.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<UserInRole>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(UserInRole), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<UserInRole, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateUserInRole(UserInRole model)
        {
            _dbContext.UserInRole.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateUserInRole(Guid id, UserInRole updatedEntity)
        {
            _dbContext.UserInRole.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteUserInRole(Guid id)
        {
            var entityData = _dbContext.UserInRole.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.UserInRole.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchUserInRole(Guid id, JsonPatchDocument<UserInRole> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.UserInRole.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.UserInRole.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}