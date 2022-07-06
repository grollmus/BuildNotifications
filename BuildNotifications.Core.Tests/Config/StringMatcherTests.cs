using BuildNotifications.Core.Config;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace BuildNotifications.Core.Tests.Config;

public class StringMatcherTests
{
    [Theory]
    [InlineData("", "", true)]
    [InlineData("", "abc", true)]
    [InlineData("*", "abc", true)]
    [InlineData("****", "abc", true)]
    [InlineData("abc", "abc", true)]
    [InlineData("aBc", "abc", true)]
    [InlineData("\"aBc\"", "abc", false)]
    [InlineData("BC", "ABCDE", true)]
    [InlineData("\"BC\"", "ABCDE", true)]
    [InlineData("*\"BC\"*", "ABCDE", true)]
    [InlineData("*\"bC\"*", "ABCDE", false)]
    [InlineData("\"ABC", "ABCDE", true)]
    [InlineData("\"ABC*", "ABCDE", true)]
    [InlineData("\"AbC*", "ABCDE", false)]
    [InlineData("\"AB\"*E", "ABCDE", true)]
    [InlineData("AB*E", "ABCDE", true)]
    [InlineData("AB*CE", "ABCDE", false)]
    [InlineData("\"ab\"*", "ABCDE", false)]
    [InlineData("\"AB\"*", "ABCDE", true)]
    [InlineData("*DE", "ABCDE", true)]
    [InlineData("*D", "ABCDE", true)]
    [InlineData("*D*", "ABCDE", true)]
    [InlineData("\"C\"", "ABCDE", true)]
    [InlineData("BC", "bC", true)]
    [InlineData("\"BC\"", "bC", false)]
    [InlineData("ABC", "bC", false)]
    [InlineData("*C", "bC", true)]
    [InlineData("C", "bC", true)]
    [InlineData("\"Abc\"", "Abc", true)]
    [InlineData("\"Ab", "Abc", true)]
    public void MatcherWithImpliedWildcardsShouldMatch(string pattern, string input, bool shouldBeMatch)
    {
        var matcher = new StringMatcher(pattern);

        var isMatch = matcher.IsMatch(input);

        Assert.Equal(shouldBeMatch, isMatch);
    }

    [Theory]
    [InlineData("", "", false)]
    [InlineData("", "abc", false)]
    [InlineData("abc", "abc", true)]
    [InlineData("aBc", "abc", true)]
    [InlineData("\"aBc\"", "abc", false)]
    [InlineData("BC", "ABCDE", false)]
    [InlineData("\"BC\"", "ABCDE", false)]
    [InlineData("*\"BC\"*", "ABCDE", true)]
    [InlineData("*\"bC\"*", "ABCDE", false)]
    [InlineData("\"ABC", "ABCDE", false)]
    [InlineData("\"ABC*", "ABCDE", true)]
    [InlineData("\"AbC*", "ABCDE", false)]
    [InlineData("\"AB\"*E", "ABCDE", true)]
    [InlineData("AB*E", "ABCDE", true)]
    [InlineData("AB*CE", "ABCDE", false)]
    [InlineData("\"ab\"*", "ABCDE", false)]
    [InlineData("\"AB\"*", "ABCDE", true)]
    [InlineData("*DE", "ABCDE", true)]
    [InlineData("*D", "ABCDE", false)]
    [InlineData("*D*", "ABCDE", true)]
    [InlineData("\"C\"", "ABCDE", false)]
    [InlineData("BC", "bC", true)]
    [InlineData("\"BC\"", "bC", false)]
    [InlineData("ABC", "bC", false)]
    [InlineData("*C", "bC", true)]
    [InlineData("C", "bC", false)]
    [InlineData("\"Abc\"", "Abc", true)]
    [InlineData("\"Abc", "Abc", true)]
    [InlineData("Bra*A", "BranchA", true)]
    [InlineData("Bra*A", "BranchB", false)]
    public void MatcherWithNoImpliedWildcardsShouldMatch(string pattern, string input, bool shouldBeMatch)
    {
        var matcher = new StringMatcher("=" + pattern);

        var isMatch = matcher.IsMatch(input);

        Assert.Equal(shouldBeMatch, isMatch);
    }
}