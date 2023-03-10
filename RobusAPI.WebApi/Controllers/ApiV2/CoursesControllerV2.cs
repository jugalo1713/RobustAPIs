using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RobusAPI.Models.Courses;
using RobusAPI.Services.Courses;
using RobusAPI.WebApi.Attributes;
using RobusAPI.WebApi.Models.Apiv2.Courses;

namespace RobusAPI.WebApi.Controllers.ApiV2
{
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("apiv2/courses")]
    [ApiController]
    [Produces("application/json", "application/xml")]
    [BasicAuthorization]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CoursesControllerV2 : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        public CoursesControllerV2(ICourseService courseService, IMapper mapper)
        {
            this._courseService = courseService;
            this._mapper = mapper;
        }

        /// <summary>
        /// Gets a pageable list of Courses
        /// that match the query from the DataStore 
        /// </summary>
        /// <param name="request">Query parameters</param>
        /// <returns>A list of Courses</returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public CourseDto[] GetList([FromQuery] GetCourseListRequest request)
        {
            var courses = _courseService.GetCourseList(request.Skip ?? 0, request.Take ?? 25, request.Search);
            var courseDtos = _mapper.Map<CourseDto[]>(courses);

            return courseDtos;
        }

        /// <summary>
        /// Gets a Course from the DataStore
        /// </summary>
        /// <param name="courseId">The Id of the Course</param>
        /// <returns></returns>
        /// <response code="404">When the course does not exist</response>
        [HttpGet]
        [Route("{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult  Get([FromRoute]int courseId)
        {
            var courseDto = _mapper.Map<CourseDto>(_courseService.GetCourse(courseId));

            if (courseDto == null) return NotFound();

            return Ok(courseDto);
        }

        /// <summary>
        /// Adds a Course to the DataStore
        /// </summary>
        /// <param name="request">The details of the new Course</param>
        /// <returns>The newly added course</returns>
        /// <response code="422">When there is a validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult Add([FromBody]AddCourseRequest request)
        {
            var courseentity = _mapper.Map<Course>(request);
            var id = _courseService.AddCourse(courseentity);
            var course = _courseService.GetCourse(id);
            var courseDto = _mapper.Map<CourseDto>(course);
            return CreatedAtAction(nameof(Get), new { courseId = id}, courseDto);
        }

        /// <summary>
        /// Updates a Course in the DataStore
        /// </summary>
        /// <param name="courseId">The id of the Course to update</param>
        /// <param name="request">The details of the Course to be updated</param>
        /// <response code="422">When there is a validation error</response>
        /// <response code="404">When the course was not found by its id</response>
        [HttpPut]
        [Route("{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult Update ([FromRoute] int courseId, [FromBody]UpdateCourseRequest request)
        {
            if (!_courseService.DoesCourseExists(courseId)) return NotFound();
            
            var courseToUpdate = _mapper.Map<Course>(request);
            courseToUpdate.Id = courseId;
            _courseService.UpdateCourse(courseToUpdate);

            return NoContent();
        }

        /// <summary>
        /// Deletes a Course from the DataStore
        /// </summary>
        /// <param name="courseId">The id of the Course to delete</param>
        /// <response code="404">When the course was not found by its id</response>
        [HttpDelete]
        [Route("{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int courseId)
        {
            if (!_courseService.DoesCourseExists(courseId)) return NotFound();

            _courseService.DeleteCourse(courseId);
            return NoContent();
        }
    }
}
