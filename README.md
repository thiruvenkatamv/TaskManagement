# TaskManagement API

A simple task tracking API built with ASP.NET Core and tested using NUnit.

## Features
- CRUD for Task items
- Entity Framework Core with SQL Server
- NUnit test project included

## DB Connection Details
Server : taskdbround2.database.windows.net
- Authentication Type : SQL Login
- User Name : taskdb
- Password : Test@1993

## How to Run
```bash
dotnet build
dotnet test


#### 🧪 3. **Add More Test Cases**
- BadRequest on mismatched IDs
- Update/Delete non-existent task
- Invalid task creation (missing name, etc.)

#### 🔁 4. **Switch to In-Memory DB for Faster Testing**
If you're still using SQL Server for tests, we can convert to:
```csharp
.UseInMemoryDatabase("TestDb")
