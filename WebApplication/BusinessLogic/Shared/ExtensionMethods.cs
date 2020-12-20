using System;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApplication.BusinessLogic.Shared
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);
            if (httpContext.Response.StatusCode == 404)
            { }
        }
    }
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSPortsExceptionHandler(this IApplicationBuilder @this)
        {
            return @this.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }

    public static class IntExtensions
    {
        public static bool IsValidId(this int? @this)
        {
            if (@this == null || @this < 1)
                return false;
            return true;
        }

        public static bool IsNotValidId(this int? @this)
        {
            if (@this == null || @this < 1)
                return true;
            return false;
        }

        public static void ThrowIfInvalidId(this int? @this)
        {
            if (@this.IsNotValidId())
                throw new BusinessException("Error", "The id is invalid.");
        }

        public static void ThrowIfNegativeId(this int @this)
        {
            if (@this < 0)
                throw new BusinessException("Error", "The id is negative.");
        }
    }

    public static class ObjectExtensions
    {
        public static void ThrowIfNull<T>(this T @this)
        {
            if (@this == null)
                throw new BusinessException("Error", $"{typeof(T).Name} object is null.");
        }
    }

    public static class StringExtensions
    {
        public static bool IsNullEmptyWhitespace(this string @this)
        {
            if (String.IsNullOrEmpty(@this) || String.IsNullOrWhiteSpace(@this))
                return true;

            return false;
        }

        public static bool IsNotNullEmptyWhitespace(this string @this)
        {
            if (!String.IsNullOrEmpty(@this) && !String.IsNullOrWhiteSpace(@this))
                return true;

            return false;
        }
    }
}
