using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class CourseRepository
    {
        public List<Course> GetAll()
        {
            List<Course> courses = new List<Course>();

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT id, code, name, units
                  FROM courses
                  ORDER BY code;",
                connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    courses.Add(Map(reader));
                }
            }

            return courses;
        }

        public Course GetById(int id)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT id, code, name, units
                  FROM courses
                  WHERE id = @id
                  LIMIT 1;",
                connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? Map(reader) : null;
                }
            }
        }

        public int Create(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException("course");
            }

            using (MySqlConnection connection = Database.OpenConnection())
            {
                using (MySqlCommand command = new MySqlCommand(
                    @"INSERT INTO courses (code, name, units)
                      VALUES (@code, @name, @units);",
                    connection))
                {
                    command.Parameters.AddWithValue("@code", course.Code);
                    command.Parameters.AddWithValue("@name", course.Name);
                    command.Parameters.AddWithValue("@units", course.Units);
                    command.ExecuteNonQuery();
                }

                using (MySqlCommand command = new MySqlCommand(
                    "SELECT LAST_INSERT_ID();",
                    connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public bool Update(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException("course");
            }

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"UPDATE courses
                  SET code = @code, name = @name, units = @units
                  WHERE id = @id;",
                connection))
            {
                command.Parameters.AddWithValue("@code", course.Code);
                command.Parameters.AddWithValue("@name", course.Name);
                command.Parameters.AddWithValue("@units", course.Units);
                command.Parameters.AddWithValue("@id", course.Id);

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Deletes a course. Deleting is blocked (returns false) while the course is still
        /// referenced by any enrollment row; students who merely have the course assigned as
        /// their "course_id" are handled by the ON DELETE SET NULL foreign key, so their row
        /// is kept and the reference is cleared.
        /// </summary>
        public bool Delete(int id)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            {
                using (MySqlCommand command = new MySqlCommand(
                    @"SELECT COUNT(*) FROM enrollments WHERE course_id = @id;",
                    connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                    {
                        return false;
                    }
                }

                using (MySqlCommand command = new MySqlCommand(
                    @"DELETE FROM courses WHERE id = @id;",
                    connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        private static Course Map(MySqlDataReader reader)
        {
            return new Course
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Code = reader.GetString(reader.GetOrdinal("code")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Units = reader.GetInt32(reader.GetOrdinal("units"))
            };
        }
    }
}
