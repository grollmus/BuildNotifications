using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Text;
using Xunit;

namespace BuildNotifications.Core.Tests.Text
{
    public class StringLocalizerTests
    {
        private IEnumerable<CultureInfo> AllCultures => CultureInfo.GetCultures(CultureTypes.AllCultures);

        private const string NativeName = nameof(NativeName);

        [Fact]
        public void AllSupportedCulturesShouldHaveNativeNameLocalized()
        {
            Assert.All(StringLocalizer.SupportedCultures(), c =>
            {
                var translation = StringLocalizer.Instance.GetText(NativeName, c);

                Assert.NotEqual(translation, NativeName);
            });
        }

        [Fact]
        public void NoneOfTheUnsupportedCulturesShouldProvideATextDifferentThanDefaultCulture()
        {
            var unsupportedCultures = AllCultures.Except(StringLocalizer.SupportedCultures());

            Assert.All(unsupportedCultures, c =>
            {
                var cultureSpecificText = StringLocalizer.Instance.GetText(NativeName, c);
                var defaultTranslation = StringLocalizer.Instance.GetText(NativeName, StringLocalizer.DefaultCulture);

                Assert.Equal(cultureSpecificText, defaultTranslation);
            });
        }

        [Fact]
        public void NullValueShouldResultInEmptyString()
        {
            var result = StringLocalizer.Instance[null!];

            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void NotTranslatedTextReturnsFallbackLanguageTranslation()
        {
            var localizer = StringLocalizer.Instance;
            const string textOnlyInEnglishId = nameof(textOnlyInEnglishId);
            const string textOnlyInEnglish = nameof(textOnlyInEnglish);
            localizer.Cache[StringLocalizer.DefaultCulture].Add(textOnlyInEnglishId, textOnlyInEnglish);

            foreach (var supportedCulture in StringLocalizer.SupportedCultures())
            {
                Assert.Equal(StringLocalizer.Instance.GetText(textOnlyInEnglishId, supportedCulture), textOnlyInEnglish);
            }
        }
    }
}