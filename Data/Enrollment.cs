using System;

namespace System_Design
{
    internal sealed class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public decimal? Prelim { get; set; }

        public decimal? Midterm { get; set; }

        public decimal? Finals { get; set; }

        public decimal? Activities { get; set; }

        public decimal? Grade { get; set; }

        public decimal? Gpa { get; set; }

        public string SchoolYear { get; set; }

        public string Semester { get; set; }

        public DateTime EnrolledAt { get; set; }

        /// <summary>Joined fields used by the UI for display.</summary>
        public string StudentNumber { get; set; }

        public string StudentName { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }
    }
}
