﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Mahamudra.Tapper.Tests.Common; 
public static class SqlExtensions
{ 
    public static string Add(this string sql, string? schema)
    {
        return sql.Replace("/*schema*/ ", schema);
    }
    public static string GetColumn<T>(this string propertyName)
    {
        var propertyInfo = typeof(T)!.GetProperty(propertyName)!;
        var customAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ColumnAttribute))!;
        if (customAttribute != null)
            return $"{customAttribute.Name!}";
        throw new ArgumentException($"Error mapping column {propertyName}");
    }

    public static string GetAsColumn<T>(this string propertyName)
    {
        var fieldName = GetColumn<T>(propertyName);
        return $"{fieldName} as {propertyName}";
    }

    public static string GetTable<T>(this string className)
    {
        var customAttribute = typeof(T)!.GetCustomAttribute<TableAttribute>();
        if (customAttribute != null)
            return $"{customAttribute.Name!}";
        throw new ArgumentException($"Error mapping table {className}");
    }
}