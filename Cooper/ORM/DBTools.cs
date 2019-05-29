using System;
using System.Data;
using System.Data.Common;
using Cooper.Configuration;

namespace Cooper
{
    public class DbTools
    {
        public static string SanitizeString(string value)
        {
            if (value == null) { 
                return null;
            }
            return value.Replace("'", "").Replace("\"", "");
        }
    }
}