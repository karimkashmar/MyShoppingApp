using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MyShoppingApp.Model;

namespace MyShoppingApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "database.db");
            _connectionString = $"Data Source={databasePath}";
        }

        public async Task<bool> InitializeDatabaseAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Items (
                                        ID INTEGER PRIMARY KEY,
                                        Price REAL NOT NULL,
                                        Name TEXT NOT NULL,
                                        Image Text NOT NULL,
                                        SDescription TEXT NOT NULL,
                                        LDescription TEXT NOT NULL,
                                        QtyInStock INTEGER NOT NULL,
                                        TrendingRating INTEGER NOT NULL
                                    );
                                    CREATE TABLE IF NOT EXISTS Orders (
                                        OrderID INTEGER PRIMARY KEY,
                                        UserID INTEGER NOT NULL,
                                        DeliveryAddress TEXT NOT NULL,
                                        TotalCost REAL NOT NULL,
                                        FOREIGN KEY (UserID) REFERENCES Users(Id)
                                    );
                                    CREATE TABLE IF NOT EXISTS OrderItems (
                                        OrderItemID INTEGER NOT NULL,
                                        ItemID INTEGER NOT NULL,
                                        OrderID INTEGER NOT NULL,
                                        OrderQty INTEGER NOT NULL,
                                        PRIMARY KEY (OrderItemID),
                                        FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
                                        FOREIGN KEY (ItemID) REFERENCES Items(ID)
                                    );
                                    CREATE TABLE IF NOT EXISTS Users (
                                        Id INTEGER PRIMARY KEY,
                                        Username TEXT NOT NULL,
                                        Email TEXT NOT NULL,
                                        Password TEXT NOT NULL,
                                        FName TEXT NOT NULL,
                                        LName TEXT NOT NULL
                                    );";
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users";
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    FName = reader.GetString(4),
                    LName = reader.GetString(5)
                });
            }

            return users;
        }

        public async Task<User> GetUserAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    FName = reader.GetString(4),
                    LName = reader.GetString(5)
                };
            }

            return null;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Username = @username";
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    FName = reader.GetString(4),
                    LName = reader.GetString(5)
                };
            }

            return null;
        }
        public async Task<User> ValidatePasswordAsync(string username, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Username = @username";
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var foundUser = new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    FName = reader.GetString(4),
                    LName = reader.GetString(5)
                };
                if (foundUser.Password == password)
                    return foundUser;
                else
                    return null;
            }
            return null;
        }
        public async Task<int> AddUserAsync(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Users (Username, Email, Password, FName, LName)
                             VALUES (@username, @email, @password, @fname, @lname)";
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@fname", user.FName);
            command.Parameters.AddWithValue("@lname", user.LName);

            return await command.ExecuteNonQueryAsync();
        }
    }


}