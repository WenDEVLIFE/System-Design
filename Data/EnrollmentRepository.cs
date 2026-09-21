using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class EnrollmentRepository
    {
        public List<Enrollment> GetAll()
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT e.id, e.student_id, e.course_id, e.prelim, e.midterm, e.finals, e.activities,
                         e.grade, e.gpa, e.school_year, e.semester, e.enrolled_at,
                         s.student_number, CONCAT(s.first_name, ' ', s.last_name) AS student_name,
                         c.code AS course_code, c.name AS course_name
                  FROM enrollments e
                  JOIN students s ON s.id = e.student_id
                  JOIN courses c ON c.id = e.course_id
                  ORDER BY e.enrolled_at DESC, e.id DESC;",
                connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    enrollments.Add(Map(reader));
                }
            }

            return enrollments;
        }

        public List<Enrollment> GetByCourse(int courseId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT e.id, e.student_id, e.course_id, e.prelim, e.midterm, e.finals, e.activities,
                         e.grade, e.gpa, e.school_year, e.semester, e.enrolled_at,
                         s.student_number, CONCAT(s.first_name, ' ', s.last_name) AS student_name,
                         c.code AS course_code, c.name AS course_name
                  FROM enrollments e
                  JOIN students s ON s.id = e.student_id
                  JOIN courses c ON c.id = e.course_id
                  WHERE e.course_id = @courseId
                  ORDER BY s.student_number;",
                connection))
            {
                command.Parameters.AddWithValue("@courseId", courseId);

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

        public List<Enrollment> GetByStudent(int studentId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"SELECT e.id, e.student_id, e.course_id, e.prelim, e.midterm, e.finals, e.activities,
                         e.grade, e.gpa, e.school_year, e.semester, e.enrolled_at,
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

        public bool SaveGradeBreakdown(int studentId, int courseId, decimal? prelim, decimal? midterm, decimal? finals, decimal? activities, decimal? grade, decimal? gpa, string schoolYear, string semester)
        {
            using (MySqlConnection connection = Database.OpenConnection())
            using (MySqlCommand command = new MySqlCommand(
                @"UPDATE enrollments
                  SET prelim = @prelim,
                      midterm = @midterm,
                      finals = @finals,
                      activities = @activities,
                      grade = @grade,
                      gpa = @gpa,
                      school_year = @schoolYear,
                      semester = @semester
                  WHERE student_id = @studentId AND course_id = @courseId;",
                connection))
            {
                command.Parameters.AddWithValue("@prelim", (object)prelim ?? DBNull.Value);
                command.Parameters.AddWithValue("@midterm", (object)midterm ?? DBNull.Value);
                command.Parameters.AddWithValue("@finals", (object)finals ?? DBNull.Value);
                command.Parameters.AddWithValue("@activities", (object)activities ?? DBNull.Value);
                command.Parameters.AddWithValue("@grade", (object)grade ?? DBNull.Value);
                command.Parameters.AddWithValue("@gpa", (object)gpa ?? DBNull.Value);
                command.Parameters.AddWithValue("@schoolYear", (object)schoolYear ?? "2024-2025");
                command.Parameters.AddWithValue("@semester", (object)semester ?? "1st Semester");
                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("@courseId", courseId);

                return command.ExecuteNonQuery() > 0;
            }
        }

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
                Prelim = ReadNullableDecimal(reader, "prelim"),
                Midterm = ReadNullableDecimal(reader, "midterm"),
                Finals = ReadNullableDecimal(reader, "finals"),
                Activities = ReadNullableDecimal(reader, "activities"),
                Grade = ReadNullableDecimal(reader, "grade"),
                Gpa = ReadNullableDecimal(reader, "gpa"),
                SchoolYear = ReadNullableString(reader, "school_year"),
                Semester = ReadNullableString(reader, "semester"),
                EnrolledAt = reader.GetDateTime(reader.GetOrdinal("enrolled_at")),
                StudentNumber = reader.GetString(reader.GetOrdinal("student_number")),
                StudentName = reader.GetString(reader.GetOrdinal("student_name")),
                CourseCode = reader.GetString(reader.GetOrdinal("course_code")),
                CourseName = reader.GetString(reader.GetOrdinal("course_name"))
            };
        }

        private static string ReadNullableString(MySqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static decimal? ReadNullableDecimal(MySqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }
    }
}
