using System;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class UserRepository
    {
        public User ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                "SELECT id, username, role, password_hash FROM users WHERE username = @username LIMIT 1",
                connection))
            {
                command.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    string storedHash = reader.GetString(reader.GetOrdinal("password_hash"));
                    if (!PasswordHasher.Verify(password, storedHash))
                    {
                        return null;
                    }

                    return new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        Role = reader.IsDBNull(reader.GetOrdinal("role"))
                            ? "user"
                            : reader.GetString(reader.GetOrdinal("role"))
                    };
                }
            }
        }

        /// <summary>
        /// Creates a new user, hashing the password with BCrypt. The username is never
        /// stored in plaintext form beyond its literal value, and the password is never
        /// stored or returned. Returns null when the username is already taken.
        /// </summary>
        public User CreateUser(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            string normalizedRole = string.IsNullOrWhiteSpace(role) ? "user" : role.Trim();
            string passwordHash = PasswordHasher.HashPassword(password);

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"INSERT INTO users (username, password_hash, role)
                  VALUES (@username, @passwordHash, @role);",
                connection))
            {
                command.Parameters.AddWithValue("@username", username.Trim());
                command.Parameters.AddWithValue("@passwordHash", passwordHash);
                command.Parameters.AddWithValue("@role", normalizedRole);

                int inserted;
                try
                {
                    inserted = command.ExecuteNonQuery();
                }
                catch (MySqlException exception)
                {
                    if (exception.Number == 1062)
                    {
                        // Duplicate entry on uq_users_username.
                        return null;
                    }

                    throw;
                }

                if (inserted <= 0)
                {
                    return null;
                }

                return new User
                {
                    Username = username.Trim(),
                    Role = normalizedRole
                };
            }
        }

        /// <summary>
        /// Replaces the password for an existing user with a fresh BCrypt hash.
        /// Returns false when no user with that username exists.
        /// </summary>
        public bool UpdatePassword(string username, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
            {
                return false;
            }

            string passwordHash = PasswordHasher.HashPassword(newPassword);

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"UPDATE users
                  SET password_hash = @passwordHash
                  WHERE username = @username;",
                connection))
            {
                command.Parameters.AddWithValue("@username", username.Trim());
                command.Parameters.AddWithValue("@passwordHash", passwordHash);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public void EnsureSchema()
        {
            Schema.EnsureAll();
        }
    }
}
