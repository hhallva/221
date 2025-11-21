using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WindowsLogin.Models;

namespace WindowsLogin.Services
{
    public class DatabaseService
    {
        private string _connectionString;

        public DatabaseService()
        {
            var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.db");
            _connectionString = $"Data Source={databasePath};";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = new SqliteCommand(@"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        PasswordHash TEXT,
                        PublicKey BLOB,
                        EncryptedToken BLOB,
                        Role TEXT NOT NULL
                    )", connection);

                command.ExecuteNonQuery();
            }
        }

        public bool RegisterUserWithPassword(string username, string password, string role)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Хеширование пароля
                using (var sha256 = SHA256.Create())
                {
                    var passwordHash = Convert.ToBase64String(
                        sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));

                    var command = new SqliteCommand(@"
                        INSERT INTO Users (Username, PasswordHash, Role)
                        VALUES (@username, @passwordHash, @role)", connection);

                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);
                    command.Parameters.AddWithValue("@role", role);

                    try
                    {
                        return command.ExecuteNonQuery() > 0;
                    }
                    catch (SqliteException)
                    {
                        return false;
                    }
                }
            }
        }

        public bool RegisterUserWithRsa(string username, byte[] publicKey, byte[] encryptedToken, string role)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = new SqliteCommand(@"
                    INSERT INTO Users (Username, PublicKey, EncryptedToken, Role)
                    VALUES (@username, @publicKey, @encryptedToken, @role)", connection);

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@publicKey", publicKey);
                command.Parameters.AddWithValue("@encryptedToken", encryptedToken);
                command.Parameters.AddWithValue("@role", role);

                try
                {
                    return command.ExecuteNonQuery() > 0;
                }
                catch (SqliteException)
                {
                    return false;
                }
            }
        }

        public User AuthenticateWithPassword(string username, string password)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var sha256 = SHA256.Create())
                {
                    var passwordHash = Convert.ToBase64String(
                        sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));

                    var command = new SqliteCommand(@"
                        SELECT Id, Username, Role FROM Users 
                        WHERE Username = @username AND PasswordHash = @passwordHash", connection);

                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Role = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User GetUserForRsaAuth(string username)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = new SqliteCommand(@"
                    SELECT Id, Username, PublicKey, EncryptedToken, Role FROM Users 
                    WHERE Username = @username AND PublicKey IS NOT NULL", connection);

                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PublicKey = reader["PublicKey"] as byte[],
                            EncryptedToken = reader["EncryptedToken"] as byte[],
                            Role = reader.GetString(4)
                        };
                    }
                }
            }
            return null;
        }
    }
}
