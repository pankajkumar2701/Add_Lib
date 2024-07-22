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
    /// The booksService responsible for managing books related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting books information.
    /// </remarks>
    public interface IBooksService
    {
        /// <summary>Retrieves a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <returns>The books data</returns>
        Books GetById(Guid id);

        /// <summary>Retrieves a list of bookss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of bookss</returns>
        List<Books> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new books</summary>
        /// <param name="model">The books data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Books model);

        /// <summary>Updates a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <param name="updatedEntity">The books data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Books updatedEntity);

        /// <summary>Updates a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <param name="updatedEntity">The books data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Books> updatedEntity);

        /// <summary>Deletes a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The booksService responsible for managing books related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting books information.
    /// </remarks>
    public class BooksService : IBooksService
    {
        private Add_LibContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Books class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public BooksService(Add_LibContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <returns>The books data</returns>
        public Books GetById(Guid id)
        {
            var entityData = _dbContext.Books.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of bookss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of bookss</returns>/// <exception cref="Exception"></exception>
        public List<Books> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetBooks(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new books</summary>
        /// <param name="model">The books data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Books model)
        {
            model.Id = CreateBooks(model);
            return model.Id;
        }

        /// <summary>Updates a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <param name="updatedEntity">The books data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Books updatedEntity)
        {
            UpdateBooks(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <param name="updatedEntity">The books data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Books> updatedEntity)
        {
            PatchBooks(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific books by its primary key</summary>
        /// <param name="id">The primary key of the books</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteBooks(id);
            return true;
        }
        #region
        private List<Books> GetBooks(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Books.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Books>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Books), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Books, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateBooks(Books model)
        {
            _dbContext.Books.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateBooks(Guid id, Books updatedEntity)
        {
            _dbContext.Books.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteBooks(Guid id)
        {
            var entityData = _dbContext.Books.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Books.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchBooks(Guid id, JsonPatchDocument<Books> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Books.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Books.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}