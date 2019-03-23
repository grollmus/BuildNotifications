using BuildNotifications.Core.Plugin.Host;
using Xunit;

namespace BuildNotifications.Core.Tests.Plugin.Host
{
	public class PluginHostTests
	{
		[Fact]
		public void SchemaFactoryShouldNotBeNull()
		{
			// Arrange
			var sut = new PluginHost();

			// Act
			var actual = sut.SchemaFactory;

			// Assert
			Assert.NotNull(actual);
		}
	}
}