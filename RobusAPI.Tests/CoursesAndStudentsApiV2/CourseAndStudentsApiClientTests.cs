using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobusAPI.Integrations.CoursesAndStudentsApiV2;
using RobusAPI.Models.Courses;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RobusAPI.Tests.CoursesAndStudentsApiV2
{
    [TestClass]
    public class CourseAndStudentsApiClientTests
    {
        private CoursesAndStudentsApiClient apiClient;

        [TestInitialize]
        public void TestInitialize()
        {
            apiClient = new CoursesAndStudentsApiClient("caller@zirpl.com", "Pass123!" );
        }


        [TestMethod]
        public async Task GetCourses_NoParameters()
        {
            var results = await apiClient.GetCourses();
            results.Should().NotBeNullOrEmpty();
            results.Length.Should().BeInRange(3,5);

        }

        [TestMethod]
        public async Task GetCourses_WithSkipAndTake()
        {
            var results = await apiClient.GetCourses(1, 1);
            results.Should().NotBeNullOrEmpty();
            results.Length.Should().Be(1);
        }

        [TestMethod]
        public async Task GetCourses_withSearch()
        {
            var results = await apiClient.GetCourses(null, null, "computer");
            results.Should().NotBeNullOrEmpty();
            results.Length.Should().Be(1);
        }

        [TestMethod]
        public async Task GetCourse_Exists()
        {
            var results = await apiClient.GetCourse(1);
            results.Should().NotBeNull();
            results.Name.Should().Be("Introduction to Computer Science");
        }

        [TestMethod]
        public async Task GetCourse_DoesNotExist()
        {
            var apiclient2 = apiClient;
            await new Func<Task<Course>>(async () =>
            {
                return await apiClient.GetCourse(1000);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();

        }

        [TestMethod]
        public async Task GetCourses_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("", "");

            await new Func<Task<Course>>(async () =>
            {
                return await apiClient.GetCourse(1);
            }).Should().ThrowAsync<AuthorizationException>();

        }
        [TestMethod]
        public async Task AddCourse_ValidInput()
        {
            var newCourse = new AddCourseRequest()
            {
                Code = "CS-200",
                Name = "Intro biology",
                Department = "Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black",
            };

            var response = await apiClient.AddCourse(newCourse);
            response.Should().NotBeNull();
            response.Course.Should().NotBeNull();
            response.Course.Department.Should().Be(newCourse.Department);
            response.Course.Code.Should().Be(newCourse.Code);
            response.ResourceUri.Should().NotBeNullOrEmpty();
            response.ResourceUri.Should().Be($"https://localhost:7199/apiv2/courses/{response.Course.Id}");
        }

        [TestMethod]
        public async Task AddCourse_InvalidInput()
        {
            var newCourse = new AddCourseRequest()
            {
                Code = "CS-200",
                Name = null,
                Department = "Computer Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black",
            };

            await new Func<Task<AddCourseResponse>>(async ()=>
            {
                return await apiClient.AddCourse(newCourse);
            }).Should().ThrowAsync<ValidationException>();
        }

        [TestMethod]
        public async Task AddCourse_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("", "");

            await new Func<Task<AddCourseResponse>>(async () => {
                var newCourse = new AddCourseRequest()
                {
                    Code = "CS-200",
                    Name = "Intro to Computer Vision",
                    Department = "Computer Science",
                    ProfessorFirstName = "Joe",
                    ProfessorLastName = "Black",
                };

                return await apiClient.AddCourse(newCourse);
            }).Should().ThrowAsync<AuthorizationException>();
        }

        [TestMethod]
        public async Task UpdateCourse_ValidInput()
        {
            var request = new UpdateCourseRequest()
            {
                Code = "CS-200",
                Name = "Intro to Computer Vision",
                Department = "Computer Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black",
            };

            await apiClient.UpdateCourse(1, request);
            var course = await apiClient.GetCourse(1);
            course.Should().NotBeNull();
            course.Department.Should().Be(request.Department);
            course.Name.Should().Be(request.Name);
            course.Code.Should().Be(request.Code);
        }

        [TestMethod]
        public async Task UpdateCourse_CourseDoesNotExists()
        {
            await new Func<Task>(async() => {
               await apiClient.UpdateCourse(1000, new UpdateCourseRequest());
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task UpdateCourse_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("","");

            await new Func<Task>(async () => {
                await apiClient.UpdateCourse(1, new UpdateCourseRequest());
            }).Should().ThrowAsync<AuthorizationException>();
        }

        [TestMethod]
        public async Task DeleteCourse()
        {
            var request = new AddCourseRequest()
            {
                Code = "CS-200",
                Name = "Intro to Computer Vision",
                Department = "Computer Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black",
            };

            var response = await apiClient.AddCourse(request);
            await apiClient.DeleteCourse(response.Course.Id);

            await new Func<Task>(async () =>
            {
                await apiClient.DeleteCourse(response.Course.Id);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task DeleteCourse_CourseDoesNotExists()
        {
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteCourse(10000);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task DeleteCourse_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("", "");
            
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteCourse(1);
            }).Should().ThrowAsync<AuthorizationException>();
        }

        [TestMethod]
        public async Task SetStudentIdentificationImage_GoodImage()
        {
            byte[] image = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RobusAPI.Tests.CoursesAndStudentsApiV2.hero.jpg"))
            {
                using (var  memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    image = memoryStream.ToArray();
                }

                await apiClient.SetStudentIdentificationImage(1, 1, image, "TestImage.jpg");
            }
        }

        [TestMethod]
        public async Task SetStudentIdentificationImage_CourseAndStudentDoNotExist()
        {
            byte[] image = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RobusAPI.Tests.CoursesAndStudentsApiV2.hero.jpg"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    image = memoryStream.ToArray();
                }
            }

            await new Func<Task>(async ()=>
            {
                await apiClient.SetStudentIdentificationImage(1000, 1000, image, "TestImage.jpg");
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task SetStudentIdentificationImage_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("", "");

            byte[] image = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RobusAPI.Tests.CoursesAndStudentsApiV2.hero.jpg"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    image = memoryStream.ToArray();
                }
            }

            await new Func<Task>(async () =>
            {
                await apiClient.SetStudentIdentificationImage(1, 1, image, "TestImage.jpg");
            }).Should().ThrowAsync<AuthorizationException>();
        }


        [TestMethod]
        public async Task DeleteStudentIdentificationImage()
        {
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteStudentIdentificationImage(1000, 1000);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }


        [TestMethod]
        public async Task DeleteStudentIdentificationImage_CourseAndStudentDoNotExist()
        {
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteStudentIdentificationImage(1000, 1000);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task DeleteStudentIdentificationImage_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("", "");

            await new Func<Task>(async () =>
            {
                await apiClient.DeleteStudentIdentificationImage(1, 1);
            }).Should().ThrowAsync<AuthorizationException>();
        }

    }
}
