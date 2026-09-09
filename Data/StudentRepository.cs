using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class StudentRepository
    {
        public List<Student> GetAll()
        {
            List<Student> students = new List<Student>();

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT s.id, s.student_number, s.first_name, s.last_name, s.email, s.phone,
                         s.course_id, c.name AS course_name
                  FROM students s
                  LEFT JOIN courses c ON c.id = s.course_id
                  ORDER BY s.student_number;",
                connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    students.Add(Map(reader));
                }
            }

            return students;
        }

        public Student GetById(int id)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT s.id, s.student_number, s.first_name, s.last_name, s.email, s.phone,
                         s.course_id, c.name AS course_name
                  FROM students s
                  LEFT JOIN courses c ON c.id = s.course_id
                  WHERE s.id = @id
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

        public int Create(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException("student");
            }

            using (MySqlConnection connection = Database.OpenConnection())
            {
                using (MySqlCommand command = new MySqlCommand(
                    @"INSERT INTO students (student_number, first_name, last_name, email, phone, course_id)
                      VALUES (@studentNumber, @firstName, @lastName, @email, @phone, @courseId);",
                    connection))
                {
                    AddParameters(command, student);
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

        public bool Update(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException("student");
            }

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"UPDATE students
                  SET student_number = @studentNumber,
                      first_name = @firstName,
                      last_name = @lastName,
                      email = @email,
                      phone = @phone,
                      course_id = @courseId
                  WHERE id = @id;",
                connection))
            {
                AddParameters(command, student);
                command.Parameters.AddWithValue("@id", student.Id);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                "DELETE FROM students WHERE id = @id;",
                connection))
            {
                command.Parameters.AddWithValue("@id", id);
                return command.ExecuteNonQuery() > 0;
            }
        }

        private static void AddParameters(MySqlCommand command, Student student)
        {
            command.Parameters.AddWithValue("@studentNumber", student.StudentNumber);
            command.Parameters.AddWithValue("@firstName", student.FirstName);
            command.Parameters.AddWithValue("@lastName", student.LastName);
            command.Parameters.AddWithValue("@email", (object)student.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@phone", (object)student.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@courseId", (object)student.CourseId ?? DBNull.Value);
        }

        private static Student Map(MySqlDataReader reader)
        {
            Student student = new Student
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                StudentNumber = reader.GetString(reader.GetOrdinal("student_number")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Email = ReadNullableString(reader, "email"),
                Phone = ReadNullableString(reader, "phone"),
                CourseId = ReadNullableInt32(reader, "course_id")
            };

            if (!reader.IsDBNull(reader.GetOrdinal("course_name")))
            {
                student.CourseName = reader.GetString(reader.GetOrdinal("course_name"));
            }

            return student;
        }

        private static string ReadNullableString(MySqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static int? ReadNullableInt32(MySqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }
    }
}
