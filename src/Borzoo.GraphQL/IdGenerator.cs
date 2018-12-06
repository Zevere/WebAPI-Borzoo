using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Borzoo.GraphQL
{
    /// <summary>
    /// Contains functions for generating valid entity IDs
    /// </summary>
    public static class IdGenerator
    {
        /// <summary>
        /// Checks whether the entity ID is valid.
        /// </summary>
        public static bool IsValid(string id)
        {
            bool isValid;
            if (string.IsNullOrWhiteSpace(id))
            {
                isValid = false;
            }
            else
            {
                isValid = Regex.IsMatch(id, @"^[A-Z](?:[A-Z]|\d|-|_|\.)+$", RegexOptions.IgnoreCase);
            }

            return isValid;
        }

        /// <summary>
        /// Generate a valid ID from entity's title
        /// </summary>
        public static string GetIdFromTitle(string title)
        {
            string id;
            if (string.IsNullOrWhiteSpace(title))
            {
                id = GenerateRandomId();
            }
            else
            {
                var matches = Regex.Matches(title, @"(?:[A-Z]|\d|-|_|\.)+", RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    id = "";
                    foreach (Match match in matches)
                    {
                        id += match.Value + '-';
                    }

                    id = id.Remove(id.Length - 1); // remove last '-'
                }
                else
                {
                    id = GenerateRandomId();
                }
            }

            return id;
        }

        private static string GenerateRandomId()
        {
            var rnd = new Random(DateTime.UtcNow.Millisecond);
            var chars = Enumerable.Range(0, 15)
                .Select(_ =>
                {
                    char c = default;
                    int charType = rnd.Next() % 3;
                    switch (charType)
                    {
                        case 0: // Number
                            c = (char) rnd.Next(48, 50);
                            break;
                        case 1: // Upper-Case Letter
                            c = (char) rnd.Next(65, 91);
                            break;
                        case 2: // Lower-Case Letter
                            c = (char) rnd.Next(97, 123);
                            break;
                    }

                    return c;
                });
            return string.Join(string.Empty, chars);
        }
    }
}
