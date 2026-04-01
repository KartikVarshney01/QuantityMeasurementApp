using System.Net;
using System.Text.Json;
using QuantityMeasurementAppBusinessLayer.Exceptions;
using QuantityMeasurementAppModelLayer.DTOs;

namespace QuantityMeasurementAPI.Middleware;

/// <summary>
/// UC17 global exception handler middleware.
/// Catches every unhandled exception that propagates out of the MVC pipeline and
/// returns a standardised <see cref="ErrorResponse"/> JSON payload with the
/// correct HTTP status code — equivalent to Spring's <c>@ControllerAdvice</c>.
/// Register before <c>UseAuthorization</c> and <c>MapControllers</c> in Program.cs.
/// </summary>
// This catches errors globally so the app doesn't crash unexpectedly
// and returns a nice error message to the user.
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate                               _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware>    _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next   = next;
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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Timestamp     = DateTime.UtcNow,
            ExceptionType = exception.GetType().Name
        };

        switch (exception)
        {
            case QuantityMeasurementException qEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode         = (int)HttpStatusCode.BadRequest;
                response.Message            = qEx.Message;
                break;

            case ArgumentNullException anEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode         = (int)HttpStatusCode.BadRequest;
                response.Message            = "Invalid input: " + anEx.Message;
                break;

            case ArgumentException aEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode         = (int)HttpStatusCode.BadRequest;
                response.Message            = "Invalid argument: " + aEx.Message;
                break;

            case NotSupportedException nsEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode         = (int)HttpStatusCode.BadRequest;
                response.Message            = nsEx.Message;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode         = (int)HttpStatusCode.InternalServerError;
                response.Message            = "An unexpected error occurred. Please try again later.";
                break;
        }

        var json = JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return context.Response.WriteAsync(json);
    }
}
