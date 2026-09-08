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

        public void EnsureSchema()
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"CREATE TABLE IF NOT EXISTS users (
                    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
                    username VARCHAR(50) NOT NULL,
                    password_hash VARCHAR(255) NOT NULL,
                    role VARCHAR(20) NOT NULL DEFAULT 'user',
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (id),
                    UNIQUE KEY uq_users_username (username)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}