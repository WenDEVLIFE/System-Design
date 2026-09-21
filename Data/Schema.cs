using System;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal static class Schema
    {
        public static void EnsureAll()
        {
            using (MySqlConnection connection = Database.OpenConnection())
            {
                Execute(connection, CreateUsers);
                Execute(connection, CreateCourses);
                Execute(connection, CreateStudents);
                Execute(connection, CreateEnrollments);

                // Migrations for student extended attributes
                AddColumnIfNotExists(connection, "students", "middle_name", "VARCHAR(50) NULL AFTER first_name");
                AddColumnIfNotExists(connection, "students", "gender", "VARCHAR(20) NULL AFTER last_name");
                AddColumnIfNotExists(connection, "students", "date_of_birth", "DATE NULL AFTER gender");
                AddColumnIfNotExists(connection, "students", "year_level", "VARCHAR(20) NULL AFTER date_of_birth");
                AddColumnIfNotExists(connection, "students", "address", "VARCHAR(255) NULL AFTER year_level");
                AddColumnIfNotExists(connection, "students", "status", "VARCHAR(20) NOT NULL DEFAULT 'Active' AFTER phone");
                AddColumnIfNotExists(connection, "students", "photo_path", "VARCHAR(255) NULL AFTER status");
            }
        }

        private static void AddColumnIfNotExists(MySqlConnection connection, string table, string column, string columnDef)
        {
            try
            {
                string sql = string.Format(
                    @"SELECT COUNT(*) FROM information_schema.COLUMNS 
                      WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}';",
                    table, column);

                using (MySqlCommand checkCmd = new MySqlCommand(sql, connection))
                {
                    long count = Convert.ToInt64(checkCmd.ExecuteScalar());
                    if (count == 0)
                    {
                        string alterSql = string.Format("ALTER TABLE `{0}` ADD COLUMN `{1}` {2};", table, column, columnDef);
                        using (MySqlCommand alterCmd = new MySqlCommand(alterSql, connection))
                        {
                            alterCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch
            {
                // Graceful fallback if information_schema check differs
            }
        }

        private static void Execute(MySqlConnection connection, string sql)
        {
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private const string CreateUsers =
            @"CREATE TABLE IF NOT EXISTS users (
                id INT UNSIGNED NOT NULL AUTO_INCREMENT,
                username VARCHAR(50) NOT NULL,
                password_hash VARCHAR(255) NOT NULL,
                role VARCHAR(20) NOT NULL DEFAULT 'user',
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (id),
                UNIQUE KEY uq_users_username (username)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

        private const string CreateCourses =
            @"CREATE TABLE IF NOT EXISTS courses (
                id INT UNSIGNED NOT NULL AUTO_INCREMENT,
                code VARCHAR(20) NOT NULL,
                name VARCHAR(100) NOT NULL,
                units TINYINT UNSIGNED NOT NULL DEFAULT 3,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (id),
                UNIQUE KEY uq_courses_code (code)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

        private const string CreateStudents =
            @"CREATE TABLE IF NOT EXISTS students (
                id INT UNSIGNED NOT NULL AUTO_INCREMENT,
                student_number VARCHAR(20) NOT NULL,
                first_name VARCHAR(50) NOT NULL,
                middle_name VARCHAR(50) NULL,
                last_name VARCHAR(50) NOT NULL,
                gender VARCHAR(20) NULL,
                date_of_birth DATE NULL,
                year_level VARCHAR(20) NULL,
                address VARCHAR(255) NULL,
                email VARCHAR(100) NULL,
                phone VARCHAR(20) NULL,
                status VARCHAR(20) NOT NULL DEFAULT 'Active',
                photo_path VARCHAR(255) NULL,
                course_id INT UNSIGNED NULL,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (id),
                UNIQUE KEY uq_students_student_number (student_number),
                KEY idx_students_course_id (course_id),
                CONSTRAINT fk_students_course FOREIGN KEY (course_id)
                    REFERENCES courses (id) ON DELETE SET NULL
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

        private const string CreateEnrollments =
            @"CREATE TABLE IF NOT EXISTS enrollments (
                id INT UNSIGNED NOT NULL AUTO_INCREMENT,
                student_id INT UNSIGNED NOT NULL,
                course_id INT UNSIGNED NOT NULL,
                grade DECIMAL(5,2) NULL,
                enrolled_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (id),
                UNIQUE KEY uq_enrollments_student_course (student_id, course_id),
                KEY idx_enrollments_course_id (course_id),
                CONSTRAINT fk_enrollments_student FOREIGN KEY (student_id)
                    REFERENCES students (id) ON DELETE CASCADE,
                CONSTRAINT fk_enrollments_course FOREIGN KEY (course_id)
                    REFERENCES courses (id) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
    }
}
