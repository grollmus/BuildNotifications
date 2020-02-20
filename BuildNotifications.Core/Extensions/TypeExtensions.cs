using System;

namespace BuildNotifications.Core
{
    public static class TypeExtensions
    {
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType) => IsAssignableToGenericTypeInternal(givenType, genericType);

        public static bool IsGenericAssignableFrom(this Type genericType, Type givenType) => IsAssignableToGenericTypeInternal(givenType, genericType);

        private static bool IsAssignableToGenericTypeInternal(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null)
                return false;

            return IsAssignableToGenericTypeInternal(baseType, genericType);
        }
    }
}