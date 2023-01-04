namespace RobusAPI.WebApi.Models.Apiv2.Courses
{
    public class CourseDto
    {
        /// <summary>
        /// The Id of the course
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// The name of the course (ex: "Introduction to Chemistry")
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The course department (ex: "Chemistry")
        /// </summary>
        public string? Department { get; set; }
        // <summary>
        /// The course code (ex: "CHM-101")
        /// </summary>
        public string? Code { get; set; }
        // <summary>
        /// The professor full name
        /// </summary>
        public string? ProfessorName { get; set; }
    }
}
