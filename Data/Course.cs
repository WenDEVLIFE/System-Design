namespace System_Design
{
    internal sealed class Course
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Units { get; set; }

        public string DisplayName
        {
            get { return string.Format("{0} - {1}", Code, Name); }
        }
    }
}
