using System.Data;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;

namespace ErrorHanlding
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string ERROR_MESSAGE = $"An error has occured while processing your request. Trace Code : {context.TraceIdentifier}";
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case FileNotFoundException:
                    case KeyNotFoundException:
                    case EntryPointNotFoundException:
                    case VersionNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ArgumentOutOfRangeException:
                        response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
                        break;
                    case DirectoryNotFoundException:
                    case ArgumentNullException:
                    case ArgumentException:
                    case BadHttpRequestException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case AccessViolationException:
                    case MemberAccessException:
                    case TypeAccessException:
                    case UnauthorizedAccessException:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    case DllNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.FailedDependency;
                        break;
                    case TimeoutException:
                        response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new StandardResponse(false,ERROR_MESSAGE/*error?.Message*/,null)) ;
                await response.WriteAsync(result);
            }
        }
    }
}

    

