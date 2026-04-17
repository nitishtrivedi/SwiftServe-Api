using Serilog;
using System.Net;
using System.Text.Json;

namespace SwiftServe_API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled Exception");

                context.Response.StatusCode = 500;

                var response = ApiResponse<string>.Fail("Something went wrong");

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
