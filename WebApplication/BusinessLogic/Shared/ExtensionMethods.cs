using System;

namespace WebApplication.BusinessLogic.Shared
{
    public static class IntExtensions
    {
        public static bool IsValidId(this int? @this)
        {
            if (@this == null || @this < 0)
                return false;
            return true;
        }

        public static bool IsNotValidId(this int? @this)
        {
            if (@this == null || @this < 0)
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
                throw new BusinessException("Error", $"{@this.GetType().Name} object is null.");
        }
    }

    public static class StringExtensions
    {
        public static bool IsNullEmptyWhitespace(this string @this)
        {
            if (String.IsNullOrEmpty(@this) && String.IsNullOrWhiteSpace(@this))
                return true;

            return false;
        }
    }
}
