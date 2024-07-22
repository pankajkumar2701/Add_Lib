using Microsoft.AspNetCore.Mvc;
using Add_Lib.Models;
using Add_Lib.Services;
using Add_Lib.Entities;
using Add_Lib.Filter;
using Add_Lib.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace Add_Lib.Controllers
{
    /// <summary>
    /// Controller responsible for managing roleentitlement related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting roleentitlement information.
    /// </remarks>
    [Route("api/roleentitlement")]
    [Authorize]
    public class RoleEntitlementController : ControllerBase
    {
        private readonly IRoleEntitlementService _roleEntitlementService;

        /// <summary>
        /// Initializes a new instance of the RoleEntitlementController class with the specified context.
        /// </summary>
        /// <param name="iroleentitlementservice">The iroleentitlementservice to be used by the controller.</param>
        public RoleEntitlementController(IRoleEntitlementService iroleentitlementservice)
        {
            _roleEntitlementService = iroleentitlementservice;
        }

        /// <summary>Adds a new roleentitlement</summary>
        /// <param name="model">The roleentitlement data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("RoleEntitlement",Entitlements.Create)]
        public IActionResult Post([FromBody] RoleEntitlement model)
        {
            var id = _roleEntitlementService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of roleentitlements based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of roleentitlements</returns>
        [HttpGet]
        [UserAuthorize("RoleEntitlement",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _roleEntitlementService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <returns>The roleentitlement data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("RoleEntitlement",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _roleEntitlementService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("RoleEntitlement",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _roleEntitlementService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <param name="updatedEntity">The roleentitlement data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("RoleEntitlement",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] RoleEntitlement updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _roleEntitlementService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific roleentitlement by its primary key</summary>
        /// <param name="id">The primary key of the roleentitlement</param>
        /// <param name="updatedEntity">The roleentitlement data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("RoleEntitlement",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<RoleEntitlement> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _roleEntitlementService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}