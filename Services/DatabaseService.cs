using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyShoppingAppDB.db");
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
                                        ItemID INTEGER PRIMARY KEY,
                                        Price REAL NOT NULL,
                                        Name TEXT NOT NULL,
                                        Image Text NOT NULL,
                                        SDescription TEXT NOT NULL,
                                        LDescription TEXT NOT NULL,
                                        QtyInStock INTEGER NOT NULL
                                    );
                                    CREATE TABLE IF NOT EXISTS Orders (
                                        OrderID INTEGER PRIMARY KEY,
                                        UserID INTEGER NOT NULL,
                                        TotalCost REAL NOT NULL,
                                        DateCreated DATETIME NOT NULL,
                                        ClientID INTEGER NOT NULL,
                                        FOREIGN KEY (UserID) REFERENCES Users(UserID),
                                        FOREIGN KEY (ClientID) REFERENCES Clients(ClientID)
                                    );

                                    CREATE TABLE IF NOT EXISTS OrderItems (
                                        OrderItemID INTEGER NOT NULL,
                                        ItemID INTEGER NOT NULL,
                                        OrderID INTEGER NOT NULL,
                                        OrderQty INTEGER NOT NULL,
                                        PRIMARY KEY (OrderItemID),
                                        FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
                                        FOREIGN KEY (ItemID) REFERENCES Items(ItemID)
                                    );
                                    CREATE TABLE IF NOT EXISTS Users (
                                        UserID INTEGER PRIMARY KEY,
                                        Username TEXT NOT NULL,
                                        Email TEXT NOT NULL,
                                        Password TEXT NOT NULL,
                                        FName TEXT NOT NULL,
                                        LName TEXT NOT NULL
                                    );
                                    CREATE TABLE IF NOT EXISTS Clients (
                                        ClientID INTEGER PRIMARY KEY,
                                        FName TEXT NOT NULL,
                                        LName TEXT NOT NULL,
                                        EmailAddress TEXT NOT NULL,
                                        PhoneNumber TEXT NOT NULL,
                                        DeliveryAddress TEXT NOT NULL
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

        #region User Crud Operations
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
                    UserID = reader.GetInt32(0),
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
            command.CommandText = "SELECT * FROM Users WHERE UserID = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserID = reader.GetInt32(0),
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
                    UserID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    FName = reader.GetString(4),
                    LName = reader.GetString(5)
                };
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
        #endregion

        #region Items Crud Operations
        public async Task<bool> CreateItemAsync(Item item)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Items (Price, Name, Image, SDescription, LDescription, QtyInStock)
                                VALUES ($price, $name, $image, $sdescription, $ldescription, $qtyinstock)";
            command.Parameters.AddWithValue("$price", item.Price);
            command.Parameters.AddWithValue("$name", item.Name);
            command.Parameters.AddWithValue("$image", item.Image);
            command.Parameters.AddWithValue("$sdescription", item.SDescription);
            command.Parameters.AddWithValue("$ldescription", item.LDescription);
            command.Parameters.AddWithValue("$qtyinstock", item.QtyInStock);

            int result = await command.ExecuteNonQueryAsync();
            return result == 1;
        }
        public async Task<List<Item>> GetItemsAsync()
        {
            var items = new List<Item>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT ItemID, Price, Name, Image, SDescription, LDescription, QtyInStock FROM Items";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var item = new Item
                {
                    ItemID = reader.GetInt32(0),
                    Price = reader.GetDouble(1),
                    Name = reader.GetString(2),
                    Image = reader.GetString(3),
                    SDescription = reader.GetString(4),
                    LDescription = reader.GetString(5),
                    QtyInStock = reader.GetInt32(6)
                };

                items.Add(item);
            }

            return items;
        }
        public async Task<Item> GetItemByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT ItemID, Price, Name, Image, SDescription, LDescription, QtyInStock FROM Items WHERE ItemID = $id";
            command.Parameters.AddWithValue("$id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var item = new Item
                {
                    ItemID = reader.GetInt32(0),
                    Price = reader.GetDouble(1),
                    Name = reader.GetString(2),
                    Image = reader.GetString(3),
                    SDescription = reader.GetString(4),
                    LDescription = reader.GetString(5),
                    QtyInStock = reader.GetInt32(6)
                };

                return item;
            }

            return null;
        }
        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Items WHERE ItemID = @id";
                    command.Parameters.AddWithValue("@id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return false;
            }
        }
        public async Task<bool> UpdateItemAsync(Item item)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Items SET Price = @price, Name = @name, Image = @image, SDescription = @sdescription, LDescription = @ldescription, QtyInStock = @qtyinstock WHERE ItemID = @id";
                    command.Parameters.AddWithValue("@id", item.ItemID);
                    command.Parameters.AddWithValue("@price", item.Price);
                    command.Parameters.AddWithValue("@name", item.Name);
                    command.Parameters.AddWithValue("@image", item.Image);
                    command.Parameters.AddWithValue("@sdescription", item.SDescription);
                    command.Parameters.AddWithValue("@ldescription", item.LDescription);
                    command.Parameters.AddWithValue("@qtyinstock", item.QtyInStock);

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return false;
            }
        }
        #endregion

        #region Orders Crud Operations
        public async Task<int> CreateOrderAsync(Order order)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // insert order
                var insertOrderCmd = connection.CreateCommand();
                insertOrderCmd.CommandText = @"
                INSERT INTO Orders (UserID, TotalCost, DateCreated, ClientID) 
                VALUES ($UserID, $TotalCost, $DateCreated, $ClientID);
                SELECT last_insert_rowid();
            ";
                insertOrderCmd.Parameters.AddWithValue("$UserID", order.UserID);
                insertOrderCmd.Parameters.AddWithValue("$TotalCost", order.TotalCost);
                insertOrderCmd.Parameters.AddWithValue("$DateCreated", order.DateCreated);
                insertOrderCmd.Parameters.AddWithValue("$ClientID", order.ClientID);
                int orderId = Convert.ToInt32(await insertOrderCmd.ExecuteScalarAsync());

                return orderId;
            }
        }
        public async Task<List<Order>> GetOrdersByUserIDAsync(int id)
        {
            var orders = new List<Order>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT OrderID, UserID, TotalCost, DateCreated, ClientID FROM Orders WHERE UserID = $id";
                command.Parameters.AddWithValue("$id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var order = new Order
                        {
                            OrderID = reader.GetInt32(0),
                            UserID = reader.GetInt32(1),
                            TotalCost = reader.GetDouble(2),
                            DateCreated = reader.GetDateTime(3),
                            ClientID = reader.GetInt32(4)
                        };

                        orders.Add(order);
                    }
                }
            }

            return orders;
        }
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT UserID, TotalCost, DateCreated, ClientID FROM Orders WHERE OrderID = @orderId";
                command.Parameters.AddWithValue("@orderId", orderId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var order = new Order
                        {
                            OrderID = orderId,
                            UserID = reader.GetInt32(0),
                            TotalCost = reader.GetDouble(1),
                            DateCreated = reader.GetDateTime(2),
                            ClientID = reader.GetInt32(3)
                        };

                        return order;
                    }
                }
            }

            return null;
        }
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Orders WHERE OrderID = @OrderId";
                command.Parameters.AddWithValue("@OrderId", orderId);
                var rowsAffected = await command.ExecuteNonQueryAsync();


                await transaction.CommitAsync();
                return rowsAffected >= 1;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> UpdateOrderAsync(Order order)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE Orders SET TotalCost = @TotalCost WHERE OrderID = @OrderId";
                command.Parameters.AddWithValue("@OrderId", order.OrderID);
                command.Parameters.AddWithValue("@TotalCost", order.TotalCost);
                var rowsAffected = await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return rowsAffected >= 1;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region OrderItems Crud Operations
        public async Task<bool> CreateOrderItemAsync(OrderItem orderItem)
        {
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection(_connectionString))
                {
                    db.Open();

                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    insertCommand.CommandText = @"
                INSERT INTO OrderItems (ItemID, OrderID, OrderQty)
                VALUES ($ItemID, $OrderID, $OrderQty)";

                    insertCommand.Parameters.AddWithValue("$ItemID", orderItem.ItemID);
                    insertCommand.Parameters.AddWithValue("$OrderID", orderItem.OrderID);
                    insertCommand.Parameters.AddWithValue("$OrderQty", orderItem.OrderQty);

                    int rowsAffected = await insertCommand.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<OrderItem>> GetOrderItemsByOrderIDAsync(int orderId)
        {
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection(_connectionString))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand();
                    selectCommand.Connection = db;

                    selectCommand.CommandText = @"
                SELECT OrderItemID, ItemID, OrderID, OrderQty
                FROM OrderItems
                WHERE OrderID = $orderId";

                    selectCommand.Parameters.AddWithValue("$orderId", orderId);

                    SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                    List<OrderItem> orderItems = new List<OrderItem>();

                    while (query.Read())
                    {

                        OrderItem orderItem = new OrderItem
                        {
                            OrderItemID = query.GetInt32(0),
                            ItemID = query.GetInt32(1),
                            OrderID = query.GetInt32(2),
                            OrderQty = query.GetInt32(3)
                        };

                        orderItems.Add(orderItem);
                    }

                    return orderItems;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool> DeleteOrderItemAsync(int orderItemId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM OrderItems WHERE OrderItemID = @OrderItemId";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@OrderItemId", orderItemId);

            return await command.ExecuteNonQueryAsync() > 0;
        }
        public async Task<bool> UpdateOrderItemAsync(OrderItem orderItem)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE OrderItems SET ItemID = @ItemId, OrderQty = @OrderQty WHERE OrderItemID = @OrderItemId";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@ItemId", orderItem.ItemID);
            command.Parameters.AddWithValue("@OrderQty", orderItem.OrderQty);
            command.Parameters.AddWithValue("@OrderId", orderItem.OrderID);

            return await command.ExecuteNonQueryAsync() > 0;
        }
        #endregion


        #region Client Crud Operations
        public async Task<Client> GetClientByIDAsync(int clientID)
        {
            Client client = null;

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand($"SELECT * FROM Clients WHERE ClientID = {clientID}", connection);
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    client = new Client
                    {
                        ClientID = reader.GetInt32(0),
                        FName = reader.GetString(1),
                        LName = reader.GetString(2),
                        EmailAddress = reader.GetString(3),
                        PhoneNumber = reader.GetString(4),
                        DeliveryAddress = reader.GetString(5)
                    };
                }

                reader.Close();
            }

            return client;
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            List<Client> clients = new List<Client>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand("SELECT * FROM Clients", connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Client client = new Client
                    {
                        ClientID = reader.GetInt32(0),
                        FName = reader.GetString(1),
                        LName = reader.GetString(2),
                        EmailAddress = reader.GetString(3),
                        PhoneNumber = reader.GetString(4),
                        DeliveryAddress = reader.GetString(5)
                    };

                    clients.Add(client);
                }

                reader.Close();
            }

            return clients;
        }

        #endregion

        #region Extra Functions
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
                    UserID = reader.GetInt32(0),
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
        #endregion
    }
}