using System;

namespace BuildNotifications.Core.Utilities
{
    internal class TypeMatcher : ITypeMatcher
    {
        /// <inheritdoc />
        public bool MatchesType(Type type, string typeName)
        {
            var typeToMatch = new TypeName(type.AssemblyQualifiedName.Split(','));
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

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj) || obj is TypeName other && Equals(other);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_assemblyName != null ? _assemblyName.GetHashCode() : 0) * 397)
                           ^ (_fullTypeName != null ? _fullTypeName.GetHashCode() : 0);
                }
            }

            private bool Equals(TypeName other)
            {
                if (!_fullTypeName.Equals(other._fullTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(_assemblyName))
                {
                    return true;
                }

                return _assemblyName.Equals(other._assemblyName, StringComparison.OrdinalIgnoreCase);
            }

            private readonly string _assemblyName;
            private readonly string _fullTypeName;
        }
    }
}