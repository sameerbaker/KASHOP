using KASHOP.DAL.DTO.Response;

namespace KASHOP.UI.Middleware
{
    public class GlobalExceptionHanadling
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHanadling(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var errorDetails = new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Internal Server Error from the GlobalExceptionHanadling middleware.",
                    InnerError = ex.InnerException.Message
                };
                
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(errorDetails);
            }
        }

       
    }
}