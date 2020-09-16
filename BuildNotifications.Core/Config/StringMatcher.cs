using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Implements search pattern to match strings. Supports wildcards * and explicit match is implied by default.
    /// Quotation marks express case sensitivity. = expresses exact match
    /// </summary>
    public class StringMatcher
    {
        private bool _implyWildcards = true;
        private static readonly HashSet<char> SpecialCharacters = new HashSet<char> {'*', '"'};

        private string _searchPattern = string.Empty;

        public const char ForceExplicitMatchCharacter = '=';

        public string SearchPattern
        {
            get => _searchPattern;
            set
            {
                if (value.Length > 0 && value[0] == ForceExplicitMatchCharacter)
                {
                    _implyWildcards = false;
                    _searchPattern = value.Substring(1);
                }
                else
                {
                    _implyWildcards = true;
                    _searchPattern = value;
                }

                _splitBySpecialCharacter = SplitBySpecialCharacters(SearchPattern).ToList();
            }
        }

        private IEnumerable<string> _splitBySpecialCharacter = new List<string>();

        public StringMatcher(string searchPattern = "*")
        {
            SearchPattern = searchPattern;
        }

        public bool IsMatch(string input)
        {
            var matchCase = false;
            var useWildcard = _implyWildcards;
            var currentIndex = -1;

            if (SearchPattern.Length == 0)
                return _implyWildcards;

            foreach (var split in _splitBySpecialCharacter)
            {
                if (split == null)
                    continue;

                switch (split)
                {
                    case "\"":
                        matchCase = !matchCase;
                        continue;
                    case "*":
                        useWildcard = true;
                        continue;
                    default:

                        var newIndex = input.IndexOf(split, Math.Max(0, currentIndex), matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                        if (!useWildcard && newIndex != currentIndex + 1)
                            return false;

                        useWildcard = false;

                        if (newIndex < 0)
                            return false;

                        currentIndex = newIndex + split.Length;
                        break;
                }
            }

            if (!useWildcard && !_implyWildcards)
            {
                var lastSearchTerm = _splitBySpecialCharacter.LastOrDefault(IsNonSpecialCharacter);
                if (lastSearchTerm == null)
                    return currentIndex >= 0;

                return currentIndex == input.Length;
            }

            return currentIndex >= 0 || SearchPattern.All(c => c.Equals('*'));
        }

        private bool IsNonSpecialCharacter(string split)
        {
            return split.Length != 1 || !SpecialCharacters.Contains(split[0]);
        }

        private IEnumerable<string> SplitBySpecialCharacters(string input)
        {
            var sb = new StringBuilder();
            foreach (var c in input)
            {
                if (SpecialCharacters.Contains(c))
                {
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }

                    yield return c.ToString(CultureInfo.InvariantCulture);
                }
                else
                    sb.Append(c);
            }

            if (sb.Length > 0 || input.Length == 0)
                yield return sb.ToString();
        }
    }
}