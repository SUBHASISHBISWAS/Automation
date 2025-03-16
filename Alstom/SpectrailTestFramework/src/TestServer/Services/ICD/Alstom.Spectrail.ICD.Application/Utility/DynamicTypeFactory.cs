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
// FileName: DynamicTypeFactory.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-16
// Updated by SUBHASISH BISWAS On: 2025-03-16
//  ******************************************************************************/

#endregion

#region

using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;

#endregion

public static class DynamicTypeFactory
{
    private static readonly ConcurrentDictionary<string, Type> _typeCache = new();

    /// <summary>
    ///     ✅ Creates a dynamic type for a database (e.g., trdp_icd_generated) and adds entity properties
    /// </summary>
    public static Type CreateDatabaseType(string dbName, List<string> entityNames)
    {
        if (_typeCache.TryGetValue(dbName, out var existingType))
            return existingType;

        var assemblyName = new AssemblyName("DynamicMongoEntities");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType(dbName, TypeAttributes.Public | TypeAttributes.Class);

        // ✅ Add entity properties dynamically
        foreach (var entityName in entityNames)
        {
            var entityType = CreateEntityType(entityName);
            AddPropertyToType(typeBuilder, entityName, entityType);
        }

        var newType = typeBuilder.CreateType();
        _typeCache[dbName] = newType;
        return newType;
    }

    /// <summary>
    ///     ✅ Creates a dynamic entity type (e.g., DCUEntity) with an `Entities` list
    /// </summary>
    public static Type CreateEntityType(string entityName)
    {
        if (_typeCache.TryGetValue(entityName, out var existingType))
            return existingType;

        var assemblyName = new AssemblyName("DynamicMongoEntities");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType(entityName, TypeAttributes.Public | TypeAttributes.Class);

        // ✅ Define the `Entities` property as a list of the corresponding entity
        var entityListType =
            typeof(List<>).MakeGenericType(typeof(DCUEntity)); // You can replace DCUEntity with a generic type

        AddPropertyToType(typeBuilder, "Entities", entityListType);

        var newType = typeBuilder.CreateType();
        _typeCache[entityName] = newType;
        return newType;
    }

    /// <summary>
    ///     ✅ Helper method to add a property dynamically to a type
    /// </summary>
    private static void AddPropertyToType(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        var fieldBuilder = typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);

        var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

        var getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            propertyType, Type.EmptyTypes);

        var ilGenerator = getMethodBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
        ilGenerator.Emit(OpCodes.Ret);

        var setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            null, new[] { propertyType });

        ilGenerator = setMethodBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
        ilGenerator.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getMethodBuilder);
        propertyBuilder.SetSetMethod(setMethodBuilder);
    }
}