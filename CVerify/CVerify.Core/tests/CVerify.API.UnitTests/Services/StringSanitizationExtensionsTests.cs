using System;
using Xunit;
using FluentAssertions;
using CVerify.API.Modules.Shared.System.Extensions;

namespace CVerify.API.UnitTests.Services
{
    public class StringSanitizationExtensionsTests
    {
        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("<p>Hello World</p>", "Hello World")]
        [InlineData("<div><span>Hello</span> <b>World</b></div>", "Hello World")]
        [InlineData("<a href=\"https://google.com\">Click Here</a>", "Click Here")]
        [InlineData("No html tags here", "No html tags here")]
        [InlineData("Test <br/> break", "Test  break")]
        public void StripHtml_ShouldStripAllTags(string? input, string expectedResult)
        {
            // Act
            string result = input.StripHtml();

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("Normal search", "Normal search")]
        [InlineData("Find 50% discount", "Find 50[%] discount")]
        [InlineData("user_name", "user[_]name")]
        [InlineData("bracket[test", "bracket[[]test")]
        [InlineData("mixed%_brackets[test", "mixed[%][_]brackets[[]test")]
        public void EscapeSqlLikeWildcards_ShouldEscapeSpecialCharacters(string? input, string expectedResult)
        {
            // Act
            string result = input.EscapeSqlLikeWildcards();

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("CamelCaseString", "camel_case_string")]
        [InlineData("camelCaseString", "camel_case_string")]
        [InlineData("already_snake_case", "already_snake_case")]
        [InlineData("PascalCase", "pascal_case")]
        [InlineData("snake_Case_With_Pascal", "snake_case_with_pascal")] // handles existing snake case + capitals
        [InlineData("Simple", "simple")]
        public void ToSnakeCase_ShouldFormatCorrectly(string? input, string expectedResult)
        {
            // Act
            string result = input.ToSnakeCase();

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("snake_case_string", "snakeCaseString")]
        [InlineData("PascalCaseString", "pascalCaseString")]
        [InlineData("camelCaseString", "camelCaseString")]
        [InlineData("multiple_words_snake_case", "multipleWordsSnakeCase")]
        [InlineData("Singleword", "singleword")]
        public void ToCamelCase_ShouldFormatCorrectly(string? input, string expectedResult)
        {
            // Act
            string result = input.ToCamelCase();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
