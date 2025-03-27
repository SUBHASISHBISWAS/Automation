#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: DynamicEntityCompiler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-25
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using System.Reflection;
using System.Runtime;
using System.Text;
using Alstom.Spectrail.Server.Common.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MongoDB.Bson;

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class DynamicEntityCompiler
{
    private static string GetFriendlyTypeName(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return GetFriendlyTypeName(type.GetGenericArguments()[0]) + "?";

        var map = new Dictionary<Type, string>
        {
            [typeof(string)] = "string",
            [typeof(int)] = "int",
            [typeof(double)] = "double",
            [typeof(float)] = "float",
            [typeof(bool)] = "bool",
            [typeof(DateTime)] = "DateTime",
            [typeof(object)] = "object",
            [typeof(decimal)] = "decimal",
            [typeof(long)] = "long",
            [typeof(short)] = "short",
            [typeof(byte)] = "byte"
        };

        if (map.TryGetValue(type, out var alias))
            return alias;

        if (type == typeof(ObjectId))
            return "ObjectId";

        return type.Name;
    }

    public static void CompileAndSaveEntitiesToAssembly(IEnumerable<Type> entityTypes, string fileName,
        Action<List<Type>>
            compiledEntities)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using MongoDB.Bson;");
        sb.AppendLine("using Alstom.Spectrail.Server.Common.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {SpectrailConstants.DynamicAssemblyName}");
        sb.AppendLine("{");

        var baseProps = typeof(EntityBase)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var entityType in entityTypes)
        {
            var entityName = entityType.Name;
            var ownProps = entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !baseProps.Contains(p.Name))
                .Select(p => new
                {
                    p.Name,
                    Type = GetFriendlyTypeName(p.PropertyType)
                });

            sb.AppendLine($"    public class {entityName} : EntityBase");
            sb.AppendLine("    {");

            foreach (var prop in ownProps)
                sb.AppendLine($"        public {prop.Type} {prop.Name} {{ get; set; }}");

            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("}");

        var syntaxTree = CSharpSyntaxTree.ParseText(sb.ToString(), encoding: Encoding.UTF8);
        var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var refs = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(EntityBase).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(BsonObjectId).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(GCSettings).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!,
                "System.Runtime.dll"))
        };

        var outputFile = Path.Combine(AppContext.BaseDirectory, "DynamicEntities",
            $"{SpectrailConstants.DynamicAssemblyName}.{fileName}.DynamicEntities.dll");
        Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);

        var compilation = CSharpCompilation.Create(
            Path.GetFileNameWithoutExtension(outputFile),
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var fs = new FileStream(outputFile, FileMode.Create);
        var result = compilation.Emit(fs);
        fs.Flush(); // Ensure all bytes are written
        fs.Close(); // Important: avoid locked/incomplete files
        if (result.Success)
            compiledEntities?.Invoke(Assembly.LoadFrom(outputFile)
                .GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(EntityBase).IsAssignableFrom(t))
                .ToList());

        var errors = string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString()));
        throw new Exception($"❌ Roslyn compilation failed:\n{errors}");
    }
}