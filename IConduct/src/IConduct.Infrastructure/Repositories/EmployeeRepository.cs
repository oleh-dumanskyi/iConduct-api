using IConduct.Domain.Entities;
using IConduct.Domain.Interfaces.Repositories;
using IConduct.Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IConduct.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string? _connectionString;
        private readonly ILogger<EmployeeRepository> _logger;
        public EmployeeRepository(IConfiguration configuration, ILogger<EmployeeRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Invalid connection string");

            _logger = logger;
        }
        public async Task<bool> EnableEmployeeAsync(int id, CancellationToken cancellationToken = default)
        {
            // Better to store query as Stored Procedure in production code for clarity and security reasons
            string sqlQuery = @"UPDATE [dbo].[Employees]
                    SET [Enabled] = 1 - [Enabled]
                    WHERE Id = @Id";

            try
            {
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        await connection.OpenAsync(cancellationToken);

                        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                string message = $"Error occured on getting employee data, id = {id}";
                _logger.LogError(ex, message, id);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<List<Employee>> GetFlatEmployeeDataByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var flatList = new List<Employee>();
            // Better to store query as Stored Procedure in production code for clarity and security reasons
            string sqlQuery = @"
                WITH EmployeeHierarchy AS
                (
                    SELECT 
                        Id, 
                        [Name], 
                        ManagerId, 
                        [Enabled],
                        -- saving full path to avoid cycles
                        CAST('/' + CAST(Id AS VARCHAR(MAX)) + '/' AS VARCHAR(MAX)) AS Path
                    FROM 
                        [dbo].[Employees]
                    WHERE 
                        Id = @RootEmployeeId

                    UNION ALL

                    SELECT 
                        e.Id, 
                        e.[Name], 
                        e.ManagerId, 
                        e.[Enabled],
                        eh.Path + CAST(e.Id AS VARCHAR(MAX)) + '/' AS Path
                    FROM 
                        [dbo].[Employees] e
                    INNER JOIN 
                        EmployeeHierarchy eh ON e.ManagerId = eh.ID
                    WHERE
                        CHARINDEX('/' + CAST(e.Id AS VARCHAR(MAX)) + '/', eh.Path) = 0
                )
                SELECT 
                    Id, [Name], ManagerId, [Enabled]
                FROM 
                    EmployeeHierarchy
                OPTION (MAXRECURSION 100);
                ";

            try
            {
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@RootEmployeeId", id);

                        await connection.OpenAsync(cancellationToken);

                        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                var employee = new Employee
                                {
                                    Id = (int)reader["Id"],
                                    Name = (string)reader["Name"],
                                    Enable = (bool)reader["Enabled"],
                                    ManagerId = reader["ManagerId"] == DBNull.Value
                                                  ? null
                                                  : (int?)reader["ManagerId"]
                                };

                                flatList.Add(employee);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                string message = $"Error occured on getting employee data, id = {id}";
                _logger.LogError(ex, message, id);
                throw new RepositoryException(message, ex);
            }

            return flatList;
        }
    }
}
