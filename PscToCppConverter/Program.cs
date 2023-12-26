using System;
using System.Xml;
using System.IO;
using System.Text;

class PscToCppConverter
{
    public static void ConvertPscToCpp(string inputFilePath)
    {
        string outputFilePath = Path.ChangeExtension(inputFilePath, ".h");

        StringBuilder cppCode = new StringBuilder();

        XmlDocument doc = new XmlDocument();
        doc.Load(inputFilePath);

        XmlNodeList structDefs = doc.GetElementsByTagName("structdef");

        foreach (XmlNode structDef in structDefs)
        {
            string structName = structDef.Attributes["type"].Value;
            cppCode.AppendLine($"struct {structName}");
            cppCode.AppendLine("{");

            foreach (XmlNode child in structDef.ChildNodes)
            {
                string memberType = child.Name;
                string memberName = child.Attributes["name"]?.Value;

                if (child.Name == "array")
                {
                    if (child.Attributes["size"] != null)
                    {
                        string size = child.Attributes["size"].Value;
                        string arrayElementType = child.FirstChild.Name;
                        cppCode.AppendLine($"    {arrayElementType} {memberName}[{size}];");
                    }
                    else
                    {
                        string arrayType = child.Attributes["type"].Value;
                        string arrayElementTypeName = child.FirstChild.Attributes["type"].Value;
                        cppCode.AppendLine($"    {arrayType}<{arrayElementTypeName}> {memberName};");
                    }
                }
                else if (memberType == "struct" || memberType == "string")
                {
                    if (child.Attributes["type"] != null)
                    {
                        memberType = child.Attributes["type"].Value;
                    }
                    cppCode.AppendLine($"    {memberType} {memberName};");
                }
                else if (memberName != null)
                {
                    cppCode.AppendLine($"    {memberType} {memberName};");
                }
            }

            cppCode.AppendLine("};");
            cppCode.AppendLine();
        }

        File.WriteAllText(outputFilePath, cppCode.ToString());
    }

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: PscToCppConverter <inputFilePath>");
            return;
        }

        string inputFilePath = args[0];

        ConvertPscToCpp(inputFilePath);

        Console.WriteLine($"Conversion completed. Output file: {Path.ChangeExtension(inputFilePath, ".h")}");
    }
}
