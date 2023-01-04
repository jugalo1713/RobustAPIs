namespace RobusAPI.WebApi.Models.ApiV1.Courses
{
    public class GetCourseListRequest
    {
        /// <summary>
        /// The Number of courses to skip
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// The Number of courses to retrieve
        /// </summary>
        public int? Take { get; set; }
        /// <summary>
        /// A partial search query string
        /// </summary>
        public string? Search { get; set; }
    }
}
