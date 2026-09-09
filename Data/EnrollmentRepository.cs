using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class EnrollmentRepository
    {
        /// <summary>
        /// Returns every enrollment for one student, joined with the course (and student)
        /// details so the UI can display course names without extra round trips.
        /// </summary>
        public List<Enrollment> GetByStudent(int studentId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT e.id, e.student_id, e.course_id, e.grade, e.enrolled_at,
                         s.student_number, CONCAT(s.first_name, ' ', s.last_name) AS student_name,
                         c.code AS course_code, c.name AS course_name
                  FROM enrollments e
                  JOIN students s ON s.id = e.student_id
                  JOIN courses c ON c.id = e.course_id
                  WHERE e.student_id = @studentId
                  ORDER BY c.code;",
                connection))
            {
                command.Parameters.AddWithValue("@studentId", studentId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        enrollments.Add(Map(reader));
                    }
                }
            }

            return enrollments;
        }

        /// <summary>
        /// Enrolls a student in a course. Returns false when the pair already exists
        /// (the unique key on student_id + course_id rejects the duplicate).
        /// </summary>
        public bool Enroll(int studentId, int courseId)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"INSERT INTO enrollments (student_id, course_id)
                  VALUES (@studentId, @courseId);",
                connection))
            {
                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("@courseId", courseId);

                try
                {
                    return command.ExecuteNonQuery() > 0;
                }
                catch (MySqlException exception)
                {
                    if (exception.Number == 1062)
                    {
                        // Duplicate entry on uq_enrollments_student_course.
                        return false;
                    }

                    throw;
                }
            }
        }

        public bool Unenroll(int studentId, int courseId)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"DELETE FROM enrollments
                  WHERE student_id = @studentId AND course_id = @courseId;",
                connection))
            {
                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("@courseId", courseId);

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Sets (or clears, when grade is null) the grade for an existing enrollment.
        /// Returns false when the student is not enrolled in the course.
        /// </summary>
        public bool SetGrade(int studentId, int courseId, decimal? grade)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"UPDATE enrollments
                  SET grade = @grade
                  WHERE student_id = @studentId AND course_id = @courseId;",
                connection))
            {
                command.Parameters.AddWithValue("@grade", (object)grade ?? DBNull.Value);
                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("@courseId", courseId);

                return command.ExecuteNonQuery() > 0;
            }
        }

        private static Enrollment Map(MySqlDataReader reader)
        {
            return new Enrollment
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                StudentId = reader.GetInt32(reader.GetOrdinal("student_id")),
                CourseId = reader.GetInt32(reader.GetOrdinal("course_id")),
                Grade = ReadNullableDecimal(reader, "grade"),
                EnrolledAt = reader.GetDateTime(reader.GetOrdinal("enrolled_at")),
                StudentNumber = reader.GetString(reader.GetOrdinal("student_number")),
                StudentName = reader.GetString(reader.GetOrdinal("student_name")),
                CourseCode = reader.GetString(reader.GetOrdinal("course_code")),
                CourseName = reader.GetString(reader.GetOrdinal("course_name"))
            };
        }

        private static decimal? ReadNullableDecimal(MySqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }
    }
}
