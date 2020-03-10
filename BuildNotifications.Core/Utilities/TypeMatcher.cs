using System;

namespace BuildNotifications.Core.Utilities
{
    internal class TypeMatcher : ITypeMatcher
    {
        public bool MatchesType(Type type, string? typeName)
        {
            if (typeName == null)
                return false;

            var typeAssemblyQualifiedName = type.AssemblyQualifiedName ?? string.Empty;
            var typeToMatch = new TypeName(typeAssemblyQualifiedName.Split(','));
            var typeToCheck = new TypeName(typeName.Split(','));

            return typeToCheck.Equals(typeToMatch);
        }

        private sealed class TypeName
        {
            public TypeName(string[] typeParts)
            {
                _assemblyName = typeParts.Length > 1 ? typeParts[1].Trim() : "";
                _fullTypeName = typeParts[0].Trim();
            }

            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj) || obj is TypeName other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_assemblyName != null ? _assemblyName.GetHashCode(StringComparison.InvariantCulture) : 0) * 397)
                           ^ (_fullTypeName != null ? _fullTypeName.GetHashCode(StringComparison.InvariantCulture) : 0);
                }
            }

            private bool Equals(TypeName other)
            {
                if (!_fullTypeName.Equals(other._fullTypeName, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (string.IsNullOrEmpty(_assemblyName))
                    return true;

                return _assemblyName.Equals(other._assemblyName, StringComparison.OrdinalIgnoreCase);
            }

            private readonly string _assemblyName;
            private readonly string _fullTypeName;
        }
    }
}