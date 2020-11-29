using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Text;
using Xunit;

namespace BuildNotifications.Core.Tests.Text
{
    public partial class StringLocalizerTests
    {
        private static IEnumerable<CultureInfo> AllCultures => CultureInfo.GetCultures(CultureTypes.AllCultures);

        private const string NativeName = nameof(NativeName);

        public static TheoryData<CultureInfo> SupportedCultures
        {
            get
            {
                var data = new TheoryData<CultureInfo>();
                foreach (var supportedCulture in StringLocalizer.SupportedCultures())
                {
                    data.Add(supportedCulture);
                }

                return data;
            }
        }

        [Theory]
        [MemberData(nameof(SupportedCultures))]
        public void AllSupportedCulturesShouldHaveNativeNameLocalized(CultureInfo culture)
        {
            var translation = StringLocalizer.Instance.GetText(NativeName, culture);

            Assert.NotEqual(translation, NativeName);
        }

        [Theory]
        [MemberData(nameof(SupportedCultures))]
        public void AllTextIdsShouldBeLocalized(CultureInfo culture)
        {
            var localizer = StringLocalizer.Instance;

            var keys = StringLocalizer.Keys.All().ToList();

            Assert.NotEmpty(keys);
            Assert.All(keys, key => Assert.NotEmpty(localizer.GetText(key, culture)));
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

        [Theory]
        [MemberData(nameof(SupportedCultures))]
        public void NotTranslatedTextReturnsFallbackLanguageTranslation(CultureInfo culture)
        {
            var localizer = StringLocalizer.Instance;
            const string textOnlyInEnglishId = nameof(textOnlyInEnglishId);
            const string textOnlyInEnglish = nameof(textOnlyInEnglish);
            localizer.Cache[StringLocalizer.DefaultCulture][textOnlyInEnglishId] = textOnlyInEnglish;

            Assert.Equal(StringLocalizer.Instance.GetText(textOnlyInEnglishId, culture), textOnlyInEnglish);
        }

        [Fact]
        public void NullValueShouldResultInEmptyString()
        {
            var result = StringLocalizer.Instance[null!];

            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result));
        }
    }
}