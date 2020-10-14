using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit.Sdk;

namespace BuildNotifications.TestMocks.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UseTestCultureAttribute : BeforeAfterTestAttribute
    {
        public UseTestCultureAttribute(string culture)
        {
            _culture = CultureInfo.CreateSpecificCulture(culture);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Thread.CurrentThread.CurrentCulture = _originalCulture!;
            Thread.CurrentThread.CurrentUICulture = _originalUiCulture!;
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            _originalCulture = Thread.CurrentThread.CurrentCulture;
            _originalUiCulture = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = _culture;
            Thread.CurrentThread.CurrentUICulture = _culture;
        }

        private readonly CultureInfo _culture;
        private CultureInfo? _originalCulture;
        private CultureInfo? _originalUiCulture;
    }
}