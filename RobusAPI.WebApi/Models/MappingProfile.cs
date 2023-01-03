using AutoMapper;
using RobusAPI.Models.Courses;
using RobusAPI.WebApi.Models.Courses;

namespace RobusAPI.WebApi.Models
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(x => x.ProfessorName, options => options.MapFrom(s => s.Professor == null? null : $"{s.Professor.FirstName} {s.Professor.LastName}"));

            CreateMap<AddCourseRequest, Course>().AfterMap(AfterMap);

            CreateMap<UpdateCourseRequest, Course>().AfterMap(AfterMap);
        }

        private void AfterMap(AddCourseRequest source, Course destination)
        {
            destination.Professor = new Professor
            {
                FirstName = source.ProfessorFirstName,
                LastName = source.ProfessorLastName
            };
        }

        private void AfterMap(UpdateCourseRequest source, Course destination)
        {
            destination.Professor = new Professor
            {
                FirstName = source.ProfessorFirstName,
                LastName = source.ProfessorLastName
            };
        }
    }
}
