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
    /// The authorService responsible for managing author related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting author information.
    /// </remarks>
    public interface IAuthorService
    {
        /// <summary>Retrieves a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <returns>The author data</returns>
        Author GetById(Guid id);

        /// <summary>Retrieves a list of authors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of authors</returns>
        List<Author> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new author</summary>
        /// <param name="model">The author data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Author model);

        /// <summary>Updates a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <param name="updatedEntity">The author data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Author updatedEntity);

        /// <summary>Updates a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <param name="updatedEntity">The author data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Author> updatedEntity);

        /// <summary>Deletes a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The authorService responsible for managing author related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting author information.
    /// </remarks>
    public class AuthorService : IAuthorService
    {
        private Add_LibContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Author class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AuthorService(Add_LibContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <returns>The author data</returns>
        public Author GetById(Guid id)
        {
            var entityData = _dbContext.Author.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of authors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of authors</returns>/// <exception cref="Exception"></exception>
        public List<Author> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAuthor(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new author</summary>
        /// <param name="model">The author data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Author model)
        {
            model.Id = CreateAuthor(model);
            return model.Id;
        }

        /// <summary>Updates a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <param name="updatedEntity">The author data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Author updatedEntity)
        {
            UpdateAuthor(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <param name="updatedEntity">The author data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Author> updatedEntity)
        {
            PatchAuthor(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific author by its primary key</summary>
        /// <param name="id">The primary key of the author</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAuthor(id);
            return true;
        }
        #region
        private List<Author> GetAuthor(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Author.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Author>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Author), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Author, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAuthor(Author model)
        {
            _dbContext.Author.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAuthor(Guid id, Author updatedEntity)
        {
            _dbContext.Author.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAuthor(Guid id)
        {
            var entityData = _dbContext.Author.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Author.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAuthor(Guid id, JsonPatchDocument<Author> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Author.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Author.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}