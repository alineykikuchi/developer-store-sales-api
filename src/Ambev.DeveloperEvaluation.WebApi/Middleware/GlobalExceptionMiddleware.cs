using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
            catch (InvalidOperationException ex)
            {
                await HandleConflictExceptionAsync(context, ex);
            }
            catch (ArgumentException ex)
            {
                await HandleBadRequestExceptionAsync(context, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleUnauthorizedExceptionAsync(context, ex);
            }
            catch (OperationCanceledException ex)
            {
                await HandleOperationCancelledExceptionAsync(context, ex);
            }
            catch (TimeoutException ex)
            {
                await HandleTimeoutExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            _logger.LogWarning(exception, "Validation error occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation Failed",
                Errors = exception.Errors.Select(error => new ValidationErrorDetail
                {
                    Error = error.PropertyName,
                    Detail = error.ErrorMessage
                }).ToList()
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private async Task HandleNotFoundExceptionAsync(HttpContext context, KeyNotFoundException exception)
        {
            _logger.LogWarning(exception, "Resource not found: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var response = new ApiResponse
            {
                Success = false,
                Message = exception.Message ?? "Resource not found"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private async Task HandleConflictExceptionAsync(HttpContext context, InvalidOperationException exception)
        {
            _logger.LogWarning(exception, "Conflict occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            var response = new ApiResponse
            {
                Success = false,
                Message = exception.Message ?? "Operation conflict"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private async Task HandleBadRequestExceptionAsync(HttpContext context, ArgumentException exception)
        {
            _logger.LogWarning(exception, "Bad request: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new ApiResponse
            {
                Success = false,
                Message = exception.Message ?? "Invalid argument provided"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private async Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
        {
            _logger.LogWarning(exception, "Unauthorized access attempt");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var response = new ApiResponse
            {
                Success = false,
                Message = "Unauthorized access"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private async Task HandleOperationCancelledExceptionAsync(HttpContext context, OperationCanceledException exception)
        {
            _logger.LogInformation(exception, "Operation was cancelled");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new ApiResponse
            {
                Success = false,
                Message = "Operation was cancelled"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private async Task HandleTimeoutExceptionAsync(HttpContext context, TimeoutException exception)
        {
            _logger.LogWarning(exception, "Request timeout occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;

            var response = new ApiResponse
            {
                Success = false,
                Message = "Request timeout"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private async Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unexpected error occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred. Please try again later.",
                // Em ambiente de desenvolvimento, você pode incluir mais detalhes:
#if DEBUG
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Error = "Exception",
                        Detail = exception.ToString()
                    }
                }
#endif
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}