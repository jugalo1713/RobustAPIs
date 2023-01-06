using RobusAPI.Models.Courses;

namespace RobusAPI.Integrations.CoursesAndStudentsApiV2
{
    public class AddCourseResponse
    {
        public Course Course { get; set; }
        public string ResourceUri { get; set; }
    }
}
