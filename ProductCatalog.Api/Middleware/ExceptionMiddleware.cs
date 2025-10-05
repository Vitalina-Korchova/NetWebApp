using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Detail = _env.IsDevelopment() ? exception.StackTrace : null
        };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Validation error";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Extensions["errors"] = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                break;

            case KeyNotFoundException _:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Resource not found";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                break;

            case ArgumentException _:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid argument";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal server error";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                break;
        }

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}