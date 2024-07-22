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
    /// The roleentitlementService responsible for managing roleentitlement related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting roleentitlement information.
    /// </remarks>
    public interface IRoleEntitlementService
    {
        /// <summary>Retrieves a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <returns>The roleentitlement data</returns>
        RoleEntitlement GetById(Guid id);

        /// <summary>Retrieves a list of roleentitlements based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of roleentitlements</returns>
        List<RoleEntitlement> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new roleentitlement</summary>
        /// <param name="model">The roleentitlement data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(RoleEntitlement model);

        /// <summary>Updates a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <param name="updatedEntity">The roleentitlement data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, RoleEntitlement updatedEntity);

        /// <summary>Updates a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <param name="updatedEntity">The roleentitlement data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<RoleEntitlement> updatedEntity);

        /// <summary>Deletes a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The roleentitlementService responsible for managing roleentitlement related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting roleentitlement information.
    /// </remarks>
    public class RoleEntitlementService : IRoleEntitlementService
    {
        private Add_LibContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the RoleEntitlement class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public RoleEntitlementService(Add_LibContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <returns>The roleentitlement data</returns>
        public RoleEntitlement GetById(Guid id)
        {
            var entityData = _dbContext.RoleEntitlement.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of roleentitlements based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of roleentitlements</returns>/// <exception cref="Exception"></exception>
        public List<RoleEntitlement> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetRoleEntitlement(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new roleentitlement</summary>
        /// <param name="model">The roleentitlement data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(RoleEntitlement model)
        {
            model.Id = CreateRoleEntitlement(model);
            return model.Id;
        }

        /// <summary>Updates a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <param name="updatedEntity">The roleentitlement data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, RoleEntitlement updatedEntity)
        {
            UpdateRoleEntitlement(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <param name="updatedEntity">The roleentitlement data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<RoleEntitlement> updatedEntity)
        {
            PatchRoleEntitlement(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteRoleEntitlement(id);
            return true;
        }
        #region
        private List<RoleEntitlement> GetRoleEntitlement(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.RoleEntitlement.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<RoleEntitlement>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(RoleEntitlement), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<RoleEntitlement, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateRoleEntitlement(RoleEntitlement model)
        {
            _dbContext.RoleEntitlement.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateRoleEntitlement(Guid id, RoleEntitlement updatedEntity)
        {
            _dbContext.RoleEntitlement.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteRoleEntitlement(Guid id)
        {
            var entityData = _dbContext.RoleEntitlement.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.RoleEntitlement.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchRoleEntitlement(Guid id, JsonPatchDocument<RoleEntitlement> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.RoleEntitlement.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.RoleEntitlement.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}