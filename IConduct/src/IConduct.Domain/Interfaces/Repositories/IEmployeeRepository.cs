using IConduct.Domain.Entities;

namespace IConduct.Domain.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetFlatEmployeeDataByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> EnableEmployeeAsync(int id, CancellationToken cancellationToken = default);
    }
}
