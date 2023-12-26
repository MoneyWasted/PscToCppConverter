using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Numerics;

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
            string structName = structDef.Attributes["name"].Value;
            cppCode.AppendLine($"struct {structName}");
            cppCode.AppendLine("{");

            foreach (XmlNode child in structDef.ChildNodes)
            {
                if (child.Name == "array")
                {
                    string arrayType = child.Attributes["type"].Value;
                    string memberName = child.Attributes["name"].Value;

                    XmlNode structNode = child.ChildNodes[0];
                    string arrayElementTypeName = structNode.Attributes["type"].Value;
                    
                    cppCode.AppendLine($"    {arrayType}<{arrayElementTypeName}> {memberName};");
                }
                else
                {
                    string memberType = child.Name;
                    string memberName = child.Attributes["name"].Value;
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