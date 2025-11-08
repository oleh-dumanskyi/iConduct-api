using IConduct.Application.Exceptions;
using IConduct.Application.Interfaces;
using IConduct.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IConduct.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeManager _employeeManager;
        public EmployeesController(IEmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        /// <summary>
        /// Gets employee data by id
        /// </summary>
        /// <param name="id">Employee id</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _employeeManager.GetEmployeeByIdAsync(id, cancellationToken));
            }
            catch (Exception ex) when (ex is RepositoryException ||
                                       ex is UserNotFoundException)
            {
                return BadRequest($"Can not find user {id}");
            }
            catch (Exception)
            {
                return BadRequest("Error occured on handling request");
            }
        }

        /// <summary>
        /// Switches employee status
        /// </summary>
        /// <param name="id"></param>
        [HttpPatch("{id}")]
        public async Task<IActionResult> EnableEmployeeAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _employeeManager.EnableEmployeeAsync(id, cancellationToken);

                if (success) return Ok();
                else return BadRequest();
            }
            catch (RepositoryException)
            {
                return BadRequest($"Can not switch status of user {id}");
            }
            catch (Exception)
            {
                return BadRequest("Error occured on handling request");
            }
        }
    }
}
