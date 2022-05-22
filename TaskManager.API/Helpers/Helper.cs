using System;
using System.Globalization;

namespace TaskManager.API.Helpers
{
    public static class Helper
    {
        /// <summary>
        /// Validate Date Format
        /// </summary>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static bool IsValidDateFormat(string dateFormat)
        {
            try
            {
                DateTime validDate;
                if (DateTime.TryParseExact(dateFormat, new string[] { "MM-dd-yyyy", "MM/dd/yyyy", }, CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
