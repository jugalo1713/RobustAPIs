namespace RobusAPI.Models.Courses
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] StudentIdImageFile { get; set; }
        public string StudentIdImageFileName { get; set; }
    }
}
