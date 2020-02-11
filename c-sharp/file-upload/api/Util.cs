using System;
using System.Collections.Generic;
using System.Linq;

namespace DocUploadApi
{
    public class Util
    {
        private static readonly Random random = new Random();
        
        public static string RandomString(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}