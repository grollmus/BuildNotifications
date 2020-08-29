using System;
using BuildNotifications.Services;
using Xunit;

namespace BuildNotifications.Tests.Services
{
    public class UpdateUrlsTests
    {
        public static TheoryData<Uri, string> BaseAddressTestCases => new TheoryData<Uri, string>
        {
            {new Uri("http://example.com"), "http://example.com/"},
            {new Uri("https://example.com/file.name"), "https://example.com/"}
        };

        public static TheoryData<string, string, string> DownloadFileTestCases => new TheoryData<string, string, string>
        {
            {"https://example.com/", "file.name", "https://example.com/file.name"},
            {"http://example.com", "file", "http://example.com/file"},
            {"https://example.com/", "/file", "https://example.com/file"},
            {"https://example.com", "/file", "https://example.com/file"},
        };

        public static TheoryData<Uri, string> RelativeFileDownloadTestCases => new TheoryData<Uri, string>
        {
            {new Uri("https://example.com/file"), "file"},
            {new Uri("http://example.com/file.name"), "file.name"},
            {new Uri("http://example.com/file/name"), "file/name"}
        };

        [Fact]
        public void BaseAddressForApiRequestsShouldBeValidUrl()
        {
            // Arrange
            var sut = new UpdateUrls();

            // Act
            var actual = sut.BaseAddressForApiRequests();

            // Assert
            Assert.True(actual.IsAbsoluteUri);
        }

        [Theory]
        [MemberData(nameof(BaseAddressTestCases))]
        public void BaseAddressOfShouldConstructCorrectAddresses(Uri address, string expectedAbsolute)
        {
            // Arrange
            var sut = new UpdateUrls();

            // Act
            var actual = sut.BaseAddressOf(address);

            // Assert
            Assert.Equal(expectedAbsolute, actual.AbsoluteUri);
        }

        [Theory]
        [MemberData(nameof(DownloadFileTestCases))]
        public void DownloadFileFromReleasesPackageShouldConstructCorrectAddress(string update, string fileName, string expectedAbsolute)
        {
            // Arrange
            var sut = new UpdateUrls();

            // Act
            var actual = sut.DownloadFileFromReleasePackage(update, fileName);

            // Assert
            Assert.Equal(expectedAbsolute, actual.AbsoluteUri);
        }

        [Fact]
        public void ListReleasesShouldBeValidAbsoluteUrl()
        {
            // Arrange
            var sut = new UpdateUrls();

            // Act
            var actual = sut.ListReleases();

            // Assert
            Assert.False(actual.IsAbsoluteUri);
        }

        [Theory]
        [MemberData(nameof(RelativeFileDownloadTestCases))]
        public void RelativeFileDownloadUrlShouldBeValidRelativeUrl(Uri url, string expectedRelative)
        {
            // Arrange
            var sut = new UpdateUrls();

            // Act
            var actual = sut.RelativeFileDownloadUrl(url);

            // Assert
            Assert.False(actual.IsAbsoluteUri);
            Assert.Equal(expectedRelative, actual.ToString());
        }
    }
}