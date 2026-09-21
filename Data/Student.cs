using System;

namespace System_Design
{
    internal sealed class Student
    {
        public int Id { get; set; }

        public string StudentNumber { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string YearLevel { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }

        public string PhotoPath { get; set; }

        public int? CourseId { get; set; }

        /// <summary>Course name, populated from the JOIN when students are listed.</summary>
        public string CourseName { get; set; }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MiddleName))
                {
                    return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName).Trim();
                }
                return string.Format("{0} {1}", FirstName, LastName).Trim();
            }
        }

        public string DisplayName
        {
            get { return string.Format("{0} - {1}", StudentNumber, FullName).Trim(); }
        }
    }
}
