using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace BuildNotifications.Generators
{
    [Generator]
    internal class StringLocalizerGenerator : ISourceGenerator
    {
        private List<string> FindTextResourceNames(Compilation compilation)
        {
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var fileName = Path.GetFileNameWithoutExtension(syntaxTree.FilePath);
                if (fileName != "StringLocalizer")
                    continue;

                var directory = Path.GetDirectoryName(syntaxTree.FilePath);
                if (string.IsNullOrEmpty(directory))
                    continue;

                var inputFile = Path.Combine(directory, "Texts.en.resx");

                var xml = XDocument.Load(inputFile);

                var result = new List<string>();
                foreach (var name in xml.Descendants("data").Select(x => x.Attribute(XName.Get("name"))?.Value))
                {
                    if (!string.IsNullOrEmpty(name))
                        result.Add(name);
                }

                return result;
            }

            return new List<string>();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // Not needed
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var names = FindTextResourceNames(context.Compilation);

            var sb = new StringBuilder();

            sb.AppendLine("#pragma warning disable all");
            sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("namespace BuildNotifications.Core.Text");
            sb.AppendLine("{");
            {
                sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034\")]");
                sb.AppendLine("[SuppressMessage(\"SonarSource\", \"S3218\")]");
                sb.AppendLine("public partial class StringLocalizer");
                sb.AppendLine("{");
                {
                    foreach (var name in names)
                    {
                        sb.AppendLine($"public static string {name} => Instance[\"{name}\"];");
                    }

                    sb.AppendLine("public static class Keys");
                    sb.AppendLine("{");
                    {
                        foreach (var name in names)
                        {
                            sb.AppendLine($"public const string {name} = \"{name}\";");
                        }

                        sb.AppendLine("public static System.Collections.Generic.IEnumerable<string> All()");
                        sb.AppendLine("{");

                        foreach (var name in names)
                        {
                            sb.AppendLine($"yield return \"{name}\";");
                        }

                        sb.AppendLine("}");
                    }
                    sb.AppendLine("}");
                }
                sb.AppendLine("}");
            }
            sb.AppendLine("}");

            context.AddSource("StringLocalizer_Strings__", sb.ToString());
        }
    }
}