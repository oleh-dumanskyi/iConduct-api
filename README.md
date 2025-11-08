# Test assignment

## How to use

Service is deployed as an Azure Web Application and is available via [link](https://employee-task.azurewebsites.net/).

## API Endpoints

All endpoints described below use the `/employees` base path and are protected by the global rate limiting policy (`global-limiter-policy`).

---

### 1. Get Employee by ID

`GET /employees/{id}`

Returns the complete hierarchical (tree) structure of subordinates for the specified employee.

**URL Parameters:**

* `id` (int, required): The ID of the employee whose hierarchy is being requested.

**Possible Responses:**

* **`200 OK`**: Success. Returns an `EmployeeDTO` in the response body containing the tree structure.
* **`400 Bad Request`**: Error. Returned in the following cases:
    * The employee with the specified `id` was not found.
    * A repository-level error occurred.
    * Another unexpected error occurred.
* **`429 Too Many Requests`**: The rate limit has been triggered.

---

### 2. Toggle Employee Status

`PATCH /employees/{id}`

Toggles the `Enable` status for the specified employee (e.g., from `true` to `false` or vice versa).

**URL Parameters:**

* `id` (int, required): The ID of the employee whose status needs to be changed.

**Possible Responses:**

* **`200 OK`**: The employee's status was successfully changed.
* **`400 Bad Request`**: Error. Returned in the following cases:
    * The method failed to change the status (e.g., the employee with the `id` was not found).
    * A repository-level error occurred.
    * Another unexpected error occurred.
* **`429 Too Many Requests`**: The rate limit has been triggered.

## Architectural approach

The project is based on the Clean “Onion” architecture, which ensures strict separation of concerns. The solution is divided into four key layers: UI, Application, Infrastructure, and Domain. This approach ensures that business logic and domain entitieshave no dependencies on external resources such as databases.  To access data, the “Repository” pattern is implemented, which completely abstracts the Application layer from the details of working with ADO.NET; all SQL query and mapping logic is encapsulated in the Infrastructure layer. For demonstration purposes, SQL commands were left in the code instead of being saved as stored procedures. To ensure the security and fault tolerance of the API at the UI layer, rate limiting is configured. Error handling is implemented through wrapping: low-level exceptions are intercepted and thrown up as custom exceptions, allowing the API layer to remain independent of infrastructure details. The entire stack, from the controller to the ADO.NET call, is implemented completely asynchronously with pass-through CancellationToken for immediate cancellation of SQL queries when the connection to the client is lost.

## Test dataset

During testing, I generated and used the following data set:
| Id | Name | ManagerId | Enabled |
| :--- | :--- | :--- | :--- |
| 1 | Andrey | NULL | True |
| 2 | Alexey | 2 | True |
| 3 | Roman | 2 | True |
| 4 | Ivan | 2 | True |
| 5 | Olga | 2 | False |
| 6 | Maria | 1 | True |
| 7 | Dmitry | 1 | True |
| 8 | Pavel | 3 | True |
| 9 | Elena | NULL | True |
| 10 | Svetlana | 9 | True |
| 11 | Maxim | 9 | False |
| 12 | Victor | 12 | True |
| 13 | Sergey | 6 | True |