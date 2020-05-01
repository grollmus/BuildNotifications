using System;

namespace BuildNotifications.Core
{
    public static class TypeExtensions
    {
        public static bool CompareWithoutGenericTypes(this Type type, Type otherType)
        {
            var typeToCompare = type;

            if (type.IsGenericType)
                typeToCompare = type.GetGenericTypeDefinition();

            return typeToCompare == otherType;
        }

        public static Type? FindBaseType(this Type type, Type baseType)
        {
            var result = type;
            while (result?.CompareWithoutGenericTypes(baseType) == false)
            {
                if (result.BaseType == null)
                    return null;

                result = result.BaseType;
            }

            return result;
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType) => IsAssignableToGenericTypeInternal(givenType, genericType);

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
