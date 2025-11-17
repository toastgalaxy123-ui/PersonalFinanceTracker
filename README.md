# PersonalFinanceTracker
This is a highly secure and scalable RESTful API deisgned to be the brain of a modern personal finance tracking application. This app demonstrates expertise in structured, object-oriented programming, relational database management.

Features
Financial Data Integrity : Enforces data integrity through SQL foreign key constraints through EF Core

Calculated Financial Data : Automatically calculates the balance of any account by aggregating all transactions in real-time

Secure User Ownership : Every resource is stricly linked to a single User, preventing data leakage

Enterprise-Grade Security : Implements JWT Bearer Token Authentication using ASP.NET Core identity user management.

Data Model Relationships

User(Primary Parent, managed by Identity)
Account (e.g., Checking) -> linked to User
Category (e.g., Rent) -> linked to User
Transaction (e.g., Core Data) -> linked to Account and Category

Quick Start and Testing

Prerequisites
.NET 8 SDK
SQL Server LocalDB(installed with Visual Studio) or a connected SQL Server instance
Postman or SwaggerUI

Setup steps
1. Update Connection String : Ensure your appsettings.json file contains a valid DefaultConnection string for your local database
2. Run Migrations : Open the terminal in the project root and execute the following steps to create the database schema:
dotnet ef database update
dotnet run

Test Flow(Postman)
1. Register User : POST /api/Auth/Register (Body: email, password, firstName)
2. Login: POST /api/Auth/Login (Body: email, password). Copy the JWT Token.
3. Create dependencies(using JWT Token in Authorization: Bearer [token])
- POST /api/Categories (Create "Groceries")
- POST /api/Accounts (Create "Checking Account")
4. Create Transaction: POST /api/Transactions (use IDs from the previous step).
5. Verify Business Logic: GET /api/Accounts. Check that the returned currentBalance reflects the sum of the initialBalance and all successful Transactions.



