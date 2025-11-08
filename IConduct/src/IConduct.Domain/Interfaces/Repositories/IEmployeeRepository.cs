using IConduct.Domain.Entities;

namespace IConduct.Domain.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetEmployeeById(int id);
        bool EnableEmployee(int id);
    }
}
