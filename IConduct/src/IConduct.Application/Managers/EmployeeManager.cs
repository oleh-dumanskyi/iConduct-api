using IConduct.Application.Entities;
using IConduct.Application.Exceptions;
using IConduct.Application.Interfaces;
using IConduct.Domain.Entities;
using IConduct.Domain.Interfaces.Repositories;

namespace IConduct.Application.Managers
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeManager(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<bool> EnableEmployeeAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _employeeRepository.EnableEmployeeAsync(id, cancellationToken);
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var data = await _employeeRepository.GetFlatEmployeeDataByIdAsync(id, cancellationToken);
            var dictionary = data.ToDictionary(e=>e.Id);

            foreach (var employee in data)
            {
                if (employee.ManagerId.HasValue &&
                    employee.ManagerId != employee.Id &&
                    dictionary.TryGetValue(employee.ManagerId.Value, out Employee manager))
                {
                    manager.Employees.Add(employee);
                }
            }

            if (dictionary.TryGetValue(id, out Employee rootNode))
            {
                return MapToDto(rootNode);
            }

            throw new UserNotFoundException(id.ToString());
        }

        private EmployeeDTO MapToDto(Employee employee)
        {
            var dto = new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                ManagerId = employee.ManagerId
            };

            if (employee.Employees != null && employee.Employees.Any())
            {
                foreach (var child in employee.Employees)
                    dto.Employees.Add(MapToDto(child));
            }
            return dto;
        }
    }
}

