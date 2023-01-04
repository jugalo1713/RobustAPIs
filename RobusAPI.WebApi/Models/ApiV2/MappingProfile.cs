using AutoMapper;
using RobusAPI.Models.Courses;
using RobusAPI.WebApi.Models.Apiv2.Courses;
using RobusAPI.WebApi.Models.Courses;
using Zirpl.WebApi.Models.ApiV2.Students;

namespace RobusAPI.WebApi.Models.ApiV2
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(x => x.ProfessorName, options => options.MapFrom(s => s.Professor == null? null : $"{s.Professor.FirstName} {s.Professor.LastName}"));

            CreateMap<AddCourseRequest, Course>().AfterMap(AfterMap);

            CreateMap<UpdateCourseRequest, Course>().AfterMap(AfterMap);

            CreateMap<Student, StudentDto>();

            CreateMap<AddStudentToCourseRequestDto, Student>();

            CreateMap<UpdateStudentInCourseDto, Student>();
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
