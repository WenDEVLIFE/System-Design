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
                @"SELECT s.id, s.student_number, s.first_name, s.middle_name, s.last_name,
                         s.gender, s.date_of_birth, s.year_level, s.address, s.email, s.phone,
                         s.status, s.photo_path, s.course_id, c.name AS course_name
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
                @"SELECT s.id, s.student_number, s.first_name, s.middle_name, s.last_name,
                         s.gender, s.date_of_birth, s.year_level, s.address, s.email, s.phone,
                         s.status, s.photo_path, s.course_id, c.name AS course_name
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
                    @"INSERT INTO students 
                      (student_number, first_name, middle_name, last_name, gender, date_of_birth, year_level, address, email, phone, status, photo_path, course_id)
                      VALUES 
                      (@studentNumber, @firstName, @middleName, @lastName, @gender, @dateOfBirth, @yearLevel, @address, @email, @phone, @status, @photoPath, @courseId);",
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
                      middle_name = @middleName,
                      last_name = @lastName,
                      gender = @gender,
                      date_of_birth = @dateOfBirth,
                      year_level = @yearLevel,
                      address = @address,
                      email = @email,
                      phone = @phone,
                      status = @status,
                      photo_path = @photoPath,
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
            command.Parameters.AddWithValue("@middleName", (object)student.MiddleName ?? DBNull.Value);
            command.Parameters.AddWithValue("@lastName", student.LastName);
            command.Parameters.AddWithValue("@gender", (object)student.Gender ?? DBNull.Value);
            command.Parameters.AddWithValue("@dateOfBirth", (object)student.DateOfBirth ?? DBNull.Value);
            command.Parameters.AddWithValue("@yearLevel", (object)student.YearLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@address", (object)student.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", (object)student.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@phone", (object)student.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(student.Status) ? "Active" : student.Status);
            command.Parameters.AddWithValue("@photoPath", (object)student.PhotoPath ?? DBNull.Value);
            command.Parameters.AddWithValue("@courseId", (object)student.CourseId ?? DBNull.Value);
        }

        private static Student Map(MySqlDataReader reader)
        {
            Student student = new Student
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                StudentNumber = reader.GetString(reader.GetOrdinal("student_number")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                MiddleName = ReadNullableString(reader, "middle_name"),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Gender = ReadNullableString(reader, "gender"),
                DateOfBirth = ReadNullableDateTime(reader, "date_of_birth"),
                YearLevel = ReadNullableString(reader, "year_level"),
                Address = ReadNullableString(reader, "address"),
                Email = ReadNullableString(reader, "email"),
                Phone = ReadNullableString(reader, "phone"),
                Status = ReadNullableString(reader, "status") ?? "Active",
                PhotoPath = ReadNullableString(reader, "photo_path"),
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

        private static DateTime? ReadNullableDateTime(MySqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        private static int? ReadNullableInt32(MySqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }
    }
}
