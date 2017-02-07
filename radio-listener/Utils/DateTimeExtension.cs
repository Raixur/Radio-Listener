using System;

namespace RadioListener.Utils
{
    public static class DateTimeExtension
    {
        public static string GetDateString(this DateTime date)
        {
            return $"{date.Day}-{date.Month}-{date.Year}";
        }

        public static string GetTimeString(this DateTime dateTime)
        {
            return $"{dateTime.Hour}-{dateTime.Minute}-{dateTime.Second}";
        }
    }
}