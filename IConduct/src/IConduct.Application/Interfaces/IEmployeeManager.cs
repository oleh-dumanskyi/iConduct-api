using IConduct.Application.Entities;

namespace IConduct.Application.Interfaces
{
    public interface IEmployeeManager
    {
        Task<EmployeeDTO> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> EnableEmployeeAsync(int id, CancellationToken cancellationToken = default);
    }
}
