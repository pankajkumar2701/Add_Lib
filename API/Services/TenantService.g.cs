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
    /// The tenantService responsible for managing tenant related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting tenant information.
    /// </remarks>
    public interface ITenantService
    {
        /// <summary>Retrieves a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <returns>The tenant data</returns>
        Tenant GetById(Guid id);

        /// <summary>Retrieves a list of tenants based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of tenants</returns>
        List<Tenant> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new tenant</summary>
        /// <param name="model">The tenant data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Tenant model);

        /// <summary>Updates a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <param name="updatedEntity">The tenant data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Tenant updatedEntity);

        /// <summary>Updates a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <param name="updatedEntity">The tenant data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Tenant> updatedEntity);

        /// <summary>Deletes a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The tenantService responsible for managing tenant related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting tenant information.
    /// </remarks>
    public class TenantService : ITenantService
    {
        private Add_LibContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Tenant class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TenantService(Add_LibContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <returns>The tenant data</returns>
        public Tenant GetById(Guid id)
        {
            var entityData = _dbContext.Tenant.FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of tenants based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of tenants</returns>/// <exception cref="Exception"></exception>
        public List<Tenant> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTenant(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new tenant</summary>
        /// <param name="model">The tenant data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Tenant model)
        {
            model.Id = CreateTenant(model);
            return model.Id;
        }

        /// <summary>Updates a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <param name="updatedEntity">The tenant data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Tenant updatedEntity)
        {
            UpdateTenant(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <param name="updatedEntity">The tenant data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Tenant> updatedEntity)
        {
            PatchTenant(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific tenant by its primary key</summary>
        /// <param name="id">The primary key of the tenant</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTenant(id);
            return true;
        }
        #region
        private List<Tenant> GetTenant(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Tenant.AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Tenant>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Tenant), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Tenant, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTenant(Tenant model)
        {
            _dbContext.Tenant.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTenant(Guid id, Tenant updatedEntity)
        {
            _dbContext.Tenant.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTenant(Guid id)
        {
            var entityData = _dbContext.Tenant.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Tenant.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTenant(Guid id, JsonPatchDocument<Tenant> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Tenant.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Tenant.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}