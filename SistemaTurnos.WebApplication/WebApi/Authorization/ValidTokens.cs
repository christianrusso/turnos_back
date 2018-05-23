using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Authorization
{
    public static class ValidTokens
    {
        private static ConcurrentDictionary<string, int> tokenPerUser = new ConcurrentDictionary<string, int>();

        public static void Add(string token, int userId)
        {
            if (!tokenPerUser.TryAdd(token, userId))
            {
                throw new ApplicationException();
            }
        }

        public static void Remove(string token)
        {
            if (!tokenPerUser.TryRemove(token, out int userId))
            {
                throw new ApplicationException();
            }
        }

        public static bool IsValid(string token) => tokenPerUser.Keys.Contains(token);

        public static int? GetUserId(string token)
        {
            var value = tokenPerUser.GetValueOrDefault(token, -1);

            return value != -1 ? value : (int?)null;
        }
    }
}
