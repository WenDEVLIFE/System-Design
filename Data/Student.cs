namespace System_Design
{
    internal sealed class Student
    {
        public int Id { get; set; }

        public string StudentNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int? CourseId { get; set; }

        /// <summary>Course name, populated from the JOIN when students are listed.</summary>
        public string CourseName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName).Trim(); }
        }

        public string DisplayName
        {
            get { return string.Format("{0} - {1}", StudentNumber, FullName).Trim(); }
        }
    }
}
