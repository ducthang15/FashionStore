using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using FashionStore.Models; 

namespace FashionStore.Middlewares 
{
    public class UrlRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public UrlRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, fashionDbContext dbContext) 
        {
            var path = context.Request.Path.Value?.Trim('/').ToLower();
            if (!string.IsNullOrEmpty(path))
            {
                var redirect = await dbContext.Redirects
                    .FirstOrDefaultAsync(r => r.OldUrl.ToLower() == path);

                if (redirect != null)
                {
                    context.Response.Clear();
                    int statusCode = redirect.IsPermanent
                        ? StatusCodes.Status301MovedPermanently
                        : StatusCodes.Status302Found;

                    context.Response.StatusCode = statusCode;
                    var destinationUrl = "/" + redirect.NewUrl.Trim('/');
                    context.Response.Headers["Location"] = destinationUrl;

                    return;
                }
            }
            await _next(context);
        }
    }
}