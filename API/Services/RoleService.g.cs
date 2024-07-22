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
    /// The roleService responsible for managing role related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting role information.
    /// </remarks>
    public interface IRoleService
    {
        /// <summary>Retrieves a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <returns>The role data</returns>
        Role GetById(Guid id);

        /// <summary>Retrieves a list of roles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of roles</returns>
        List<Role> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new role</summary>
        /// <param name="model">The role data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Role model);

        /// <summary>Updates a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <param name="updatedEntity">The role data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Role updatedEntity);

        /// <summary>Updates a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <param name="updatedEntity">The role data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Role> updatedEntity);

        /// <summary>Deletes a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The roleService responsible for managing role related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting role information.
    /// </remarks>
    public class RoleService : IRoleService
    {
        private Add_LibContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Role class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public RoleService(Add_LibContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <returns>The role data</returns>
        public Role GetById(Guid id)
        {
            var entityData = _dbContext.Role.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of roles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of roles</returns>/// <exception cref="Exception"></exception>
        public List<Role> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetRole(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new role</summary>
        /// <param name="model">The role data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Role model)
        {
            model.Id = CreateRole(model);
            return model.Id;
        }

        /// <summary>Updates a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <param name="updatedEntity">The role data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Role updatedEntity)
        {
            UpdateRole(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <param name="updatedEntity">The role data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Role> updatedEntity)
        {
            PatchRole(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific role by its primary key</summary>
        /// <param name="id">The primary key of the role</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteRole(id);
            return true;
        }
        #region
        private List<Role> GetRole(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Role.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Role>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Role), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Role, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateRole(Role model)
        {
            _dbContext.Role.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateRole(Guid id, Role updatedEntity)
        {
            _dbContext.Role.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteRole(Guid id)
        {
            var entityData = _dbContext.Role.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Role.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchRole(Guid id, JsonPatchDocument<Role> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Role.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Role.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}