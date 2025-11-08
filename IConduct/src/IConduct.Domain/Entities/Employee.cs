namespace IConduct.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ManagerId { get; set; }
        public bool Enable { get; set; }
        public List<Employee>? Employees { get; set; }
        public Employee()
        {
            Employees = new List<Employee>();
        }
    }
}
