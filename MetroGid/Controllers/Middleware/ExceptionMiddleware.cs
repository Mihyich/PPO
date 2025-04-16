// using System.Net;
// using MetroGid.Services.Exceptions;
// using Microsoft.Extensions.Logging;

// namespace MetroGid.Services.Middleware
// {    
//     public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
//     {
//         private readonly ILogger<ExceptionMiddleware> _logger = logger;

//         public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//         {
//             try
//             {
//                 await next(context);
//             }
//             catch (DomainException ex)
//             {
//                 context.Response.StatusCode = (int)ex.StatusCode;
//                 await context.Response.WriteAsJsonAsync(new 
//                 {
//                     ex.Message,
//                     ex.StatusCode,
//                     Errors = ex is ValidationException valEx ? valEx.Errors : null
//                 });
//             }
//             catch (Exception ex)
//             {
//                 context.Response.StatusCode = 500;
//                 await context.Response.WriteAsJsonAsync(new
//                 {
//                     Message = "Internal server error",
//                     StatusCode = HttpStatusCode.InternalServerError
//                 });
                
//                 _logger.LogError(ex, "Unhandled exception");
//             }
//         }
//     }
// }