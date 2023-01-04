using RobusAPI.Services.Courses;
using AutoMapper;
using System.Net;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using RobusAPI.WebApi.Handlers;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { });
builder.Services.AddAuthorization(options => {
    options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication")
        .RequireAuthenticatedUser().Build());
});

builder.Services.AddControllers(options =>
//ensures 406 when ask for something that is not supported 
    {
        options.ReturnHttpNotAcceptable = true;
    })
    .AddXmlDataContractSerializerFormatters()
    .ConfigureApiBehaviorOptions(setupAction =>
    {
        setupAction.InvalidModelStateResponseFactory = (context) =>
        {
            var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

            problemDetails.Detail = "See the error field for details.";
            problemDetails.Instance = context.HttpContext.Request.Path;

            //find out which status code to use

            var actionExecutingContext = context as ActionExecutingContext;

            //if there were any modelstate errors and all arguments were found 
            //then it is a validation error

            if (context.ModelState.ErrorCount > 0 && actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count)
            {
                problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Title = "One or more validation errors ocurred";
                return new UnprocessableEntityObjectResult(problemDetails)
                {
                    ContentTypes =
                    {
                        "application/problem+json"
                    }
                };
            }
            else
            {
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "One or more errors ocurred";
                return new UnprocessableEntityObjectResult(problemDetails)
                {
                    ContentTypes =
                    {
                        "application/problem+json"
                    }
                };
            }
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { 
        Version = "v1",
        Title = "Courses API V1",
        Description = "An ASP .NET core web API for managing courses",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name ="Julian Gallo",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Julian",
            Url = new Uri("https://example.com/contact")
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "Courses and Students API V2",
        Description = "An ASP .NET core web API for managing courses and students",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Julian Gallo",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Julian",
            Url = new Uri("https://example.com/contact")
        }
    });

    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme ="basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[]{}
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMvc().AddFluentValidation(fv =>
{
    fv.DisableDataAnnotationsValidation = true;
    fv.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Courses API V1");
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "Courses and Students API V2");
        });
}
else
{
    app.UseExceptionHandler(
        appBuilder =>
        {
            appBuilder.Run(async ContextBoundObject =>
            {
                ContextBoundObject.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await ContextBoundObject.Response.WriteAsync("An unexpected error ocurred. Please try again later");
            });
        });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
