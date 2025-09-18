using System.Security;
using System.Text.Json;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Truistic;

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
        var correlationId = GetCorrelationId(context);
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId });

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            LogExceptionWithContext(ex, context, correlationId);

            var (statusCode, body) = MapExceptionToResponse(ex);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(body));

            _logger.LogInformation(
                "Sent {StatusCode} response for {ExceptionType} (CorrelationId: {CorrelationId})",
                statusCode, ex.GetType().Name, correlationId
            );
        }
    }

    private void LogExceptionWithContext(Exception ex, HttpContext context, string correlationId)
    {
        LogLevel logLevel = ex switch
        {
            LoginInUseException or MailInUseException or RouteNotFoundException or UnknownClientCredentialsException => LogLevel.Warning,

            DomainValidationException or SavedRouteNotFoundException or
            UnknownChartCredentialsException or UnknownBranchCredentialsException or
            UnknownStationCredentialsException => LogLevel.Error,

            BuilderProccessException or JsonDeserializeException or JsonValidationException => LogLevel.Critical,

            _ => LogLevel.Error
        };

        _logger.Log(logLevel, ex,
            "Exception in {Method} {Path} from {RemoteIp} | CorrelationId: {CorrelationId} | UserAgent: {UserAgent}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            correlationId,
            context.Request.Headers["User-Agent"].ToString()
        );
    }

    private (int StatusCode, object Body) MapExceptionToResponse(Exception ex)
    {
        return ex switch
        {
            LoginInUseException => (StatusCodes.Status409Conflict, new { Error = "LoginInUse", Message = "Логин уже занят" }),
            MailInUseException => (StatusCodes.Status409Conflict, new { Error = "MailInUse", Message = "Почта уже занята" }),
            NotFoundByIdException => (StatusCodes.Status404NotFound, new { Error = "NotFound", Message = "Объект не найден" }),

            RouteNotFoundException => (StatusCodes.Status500InternalServerError, new { Error = "RouteNotFound", Message = "Найти маршрут не удалось" }),
            SavedRouteNotFoundException => (StatusCodes.Status404NotFound, new { Error = "NotFound",  Message = "Сохраненный маршрут не найден" }),

            UnknownChartCredentialsException => (StatusCodes.Status500InternalServerError, new { Error = "NotFound", Message = "Необходимая схема метрополитена не найдена"}),
            UnknownBranchCredentialsException => (StatusCodes.Status500InternalServerError, new { Error = "NotFound", Message = "Необходимая ветка схемы метрополитена не найдена"}),
            UnknownStationCredentialsException => (StatusCodes.Status500InternalServerError, new { Error = "NotFound", Message = "Необходимая станция схемы метрополитена не найдена"}),
            UnknownClientCredentialsException => (StatusCodes.Status500InternalServerError, new { Error = "NotFound", Message = "Неверный логин или пароль" }),

            // SecurityException => (StatusCodes.Status403Forbidden, new { Error = "InvalidRole", Message = "Указанная роль недопустима или запрещена" }),
            DomainValidationException => (StatusCodes.Status400BadRequest, new { Error = "ValidationFailed", Message = "Ошибка валидации данных" }),
            // DataBaseException dbEx when IsUniqueConstraint(dbEx) => (StatusCodes.Status409Conflict, new { Error = "Conflict", Message = "Логин или почта уже заняты" }),
            // DataBaseException => (StatusCodes.Status500InternalServerError, new { Error = "DatabaseError", Message = "Ошибка базы данных" }),
            BuilderProccessException => (StatusCodes.Status500InternalServerError, new { Error = "SchemaBuildingFailed", Message = "Ошибка создания схемы метро" }),

            JsonDeserializeException => (500, new {Error = "JsonDeserializeFailed", Message = "Ошибка дессериализации" }),
            JsonValidationException => (StatusCodes.Status500InternalServerError, new { Error = "JsonValidationFailed", Message = "Ошибка валидации данных при дессериализации" }),

            _ => (520, new { Error = "UnknownError", Message = "Необработанная ошибка" })
        };
    }

    private static string GetCorrelationId(HttpContext context)
    {
        return context.Request.Headers.TryGetValue("X-Correlation-ID", out var cid) && !string.IsNullOrEmpty(cid)
            ? cid.ToString()
            : Guid.NewGuid().ToString();
    }

    private static bool IsUniqueConstraint(DataBaseException ex)
    {
        var msg = ex.Message.ToLower();
        return msg.Contains("unique") || msg.Contains("duplicate") || msg.Contains("already exists");
    }
}