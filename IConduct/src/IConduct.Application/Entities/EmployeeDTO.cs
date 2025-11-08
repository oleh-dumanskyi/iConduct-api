namespace IConduct.Application.Entities
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ManagerId { get; set; }
        public List<EmployeeDTO>? Employees { get; set; }
        public EmployeeDTO()
        {
            Employees = new List<EmployeeDTO>();
        }
    }
}
