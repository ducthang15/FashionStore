using Microsoft.EntityFrameworkCore;
using FashionStore.Repository; 

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
                    // Nếu tìm thấy, lập tức xóa dữ liệu phản hồi cũ để chuẩn bị chuyển hướng
                    context.Response.Clear();

                    // Trả về mã 301 (Vĩnh viễn) hoặc 302 (Tạm thời) tùy theo cấu hình dưới database
                    int statusCode = redirect.IsPermanent
                        ? StatusCodes.Status301MovedPermanently
                        : StatusCodes.Status302Found;

                    context.Response.StatusCode = statusCode;
                    var destinationUrl = "/" + redirect.NewUrl.Trim('/');
                    context.Response.Headers["Location"] = destinationUrl;

                    return; // Kết thúc luồng tại đây, không chạy xuống các Controller bên dưới nữa
                }
            }
            await _next(context);
        }
    }
}