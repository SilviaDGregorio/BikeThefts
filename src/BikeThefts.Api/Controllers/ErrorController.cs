using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace BikeThefts.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ErrorController : ControllerBase
    {
        [Route("/error-local-development")]

        public IActionResult ErrorLocalDevelopment(
        [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return context.Error switch
            {

                HttpRequestException => CreateProblem(context, (int)((HttpRequestException)context.Error).StatusCode),
                ArgumentException => CreateProblem(context, StatusCodes.Status400BadRequest),
                _ => CreateProblem(context, null)
            };

        }



        [Route("/error")]
        public IActionResult Error() => Problem();

        private ObjectResult CreateProblem(IExceptionHandlerFeature context, int? statusCode)
        {
            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message,
                statusCode: statusCode ?? 500);
        }
    }

}
