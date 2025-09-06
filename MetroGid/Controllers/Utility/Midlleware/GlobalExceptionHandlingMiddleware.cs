using System.Security;
using System.Text.Json;
using MetroGid.Core.Exceptions.Concrete;

namespace MetroGid.Controllers.Utility.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
           _logger.LogError(ex, "Необработанная ошибка в {Path}", context.Request.Path);

            int statusCode;
            object body;
            
            switch (ex)
            {
                case SecurityException secEx:
                {
                    statusCode = 403;
                    body = new
                    {
                        Error = "InvalidRole",
                        Message = "Указанная роль недопустима или запрещена",
                    };
                    break;
                }
                case BuilderProccessException bpEx:
                {
                    statusCode = 500;
                    body = new
                    {
                        Error = "SchemaBuildingFailed",
                        Message = "Ошибка создания схемы метро",
                    };
                    break;
                }
                case BuilderValidationException bvEx:
                {
                    statusCode = 500;
                    body = new
                    {
                        Error = "SchemaBuildingValidationFailed",
                        Message = "Раннее обнаружение ошибки в процессе создания метро",
                    };
                    break;
                }
                case DataBaseException dbEx when IsUniqueConstraint(dbEx):
                {
                    statusCode = 409;
                    body = new
                    {
                        Error = "Conflict",
                        Message = "Логин или почта уже заняты"
                    };
                    break;
                }
                case DataBaseException dbEx:
                {
                    statusCode = 500;
                    body = new
                    {
                        Error = "DatabaseError",
                        Message = "Ошибка базы данных"
                    };
                    break;
                }
                case DomainValidationException validationEx:
                {
                    statusCode = 400;
                    body = new
                    {
                        Error = "ValidationFailed",
                        Message = "Ошибка валидации данных",
                    };
                    break;
                }
                case JsonDeserializeException jdEx:
                {   
                    statusCode = 500;
                    body = new
                    {
                        Error = "JsonDeserializeFailed",
                        Message = "Ошибка дессериализации",
                    };
                    break;
                }
                case JsonValidationException jvEx:
                {
                    statusCode = 500;
                    body = new
                    {
                        Error = "JsonValidationFailed",
                        Message = "Ошибка валидации данных при дессериализации",
                    };
                    break;
                }
                default:
                {
                    statusCode = 520;
                    body = new
                    {
                        Error = "UnknownErroe",
                        Message = "Необработанная ошибка"
                    };
                    break;
                }
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }
    }

    private static bool IsUniqueConstraint(DataBaseException ex)
    {
        var msg = ex.Message.ToLower();
        return msg.Contains("unique") || msg.Contains("duplicate") || msg.Contains("already exists");
    }
}