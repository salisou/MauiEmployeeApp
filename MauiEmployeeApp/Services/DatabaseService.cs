using MauiEmployeeApp.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiEmployeeApp.Services
{
    public class DatabaseService
    {
        private readonly string _databasePath;
        private readonly string _connectionString;

        public DatabaseService()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "employees.db");
            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _databasePath
            }.ToString();
        }

        public Task InitializeDatabaseAync()
        {
            return Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS Employees (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Position TEXT NOT NULL,
                        Salary REAL NOT NULL
                    );
                ";
                command.ExecuteNonQuery();
            });
        }

        public Task<List<Employee>> GetEmployeesAsync()
        {
            return Task.Run(() =>
            {
                var employees = new List<Employee>();
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, FullName, Position, Salary FROM Employees;";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        Id = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Position = reader.GetString(2),
                        Salary = reader.GetDecimal(3)
                    });
                }
                return employees;
            });
        }

        public Task AddEmployeeAsync(Employee employee)
        {
            return Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    INSERT INTO Employees (FullName, Position, Salary)
                    VALUES (@FullName, @Position, @Salary);
                ";
                command.Parameters.AddWithValue("@FullName", employee.FullName);
                command.Parameters.AddWithValue("@Position", employee.Position);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.ExecuteNonQuery();
            });
        }

        public Task UpdateEmployeeAsync(Employee employee)
        {
            return Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    UPDATE Employees
                    SET FullName = @FullName, Position = @Position, Salary = @Salary
                    WHERE Id = @Id;
                ";
                command.Parameters.AddWithValue("@Id", employee.Id);
                command.Parameters.AddWithValue("@FullName", employee.FullName);
                command.Parameters.AddWithValue("@Position", employee.Position);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.ExecuteNonQuery();
            });
        }

        public Task DeleteEmployeeAsync(int employeeId)
        {
            return Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    DELETE FROM Employees
                    WHERE Id = @Id;
                ";
                command.Parameters.AddWithValue("@Id", employeeId);
                command.ExecuteNonQuery();
            });
        }
    }
}
