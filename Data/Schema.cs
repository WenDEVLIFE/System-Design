using MySql.Data.MySqlClient;

namespace System_Design
{
    /// <summary>
    /// Idempotently provisions the full application schema (users, courses, students,
    /// enrollments). Called once at startup so the app self-provisions without needing
    /// a manual .sql import. Tables are created in dependency order so the foreign keys
    /// resolve. The database itself must already exist (create it by importing
    /// database.sql, or the connection will fail with a friendly startup message).
    /// </summary>
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
                last_name VARCHAR(50) NOT NULL,
                email VARCHAR(100) NULL,
                phone VARCHAR(20) NULL,
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
