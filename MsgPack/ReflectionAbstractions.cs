// Decompiled with JetBrains decompiler
// Type: MsgPack.ReflectionAbstractions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MsgPack
{
  internal static class ReflectionAbstractions
  {
    public static readonly char TypeDelimiter = '.';
    public static readonly Type[] EmptyTypes = new Type[0];

    public static bool GetIsValueType(this Type source)
    {
      return source.IsValueType;
    }

    public static bool GetIsEnum(this Type source)
    {
      return source.IsEnum;
    }

    public static bool GetIsInterface(this Type source)
    {
      return source.IsInterface;
    }

    public static bool GetIsAbstract(this Type source)
    {
      return source.IsAbstract;
    }

    public static bool GetIsGenericType(this Type source)
    {
      return source.IsGenericType;
    }

    public static bool GetIsGenericTypeDefinition(this Type source)
    {
      return source.IsGenericTypeDefinition;
    }

    public static Assembly GetAssembly(this Type source)
    {
      return source.Assembly;
    }

    public static bool GetIsPublic(this Type source)
    {
      return source.IsPublic;
    }

    public static Type GetBaseType(this Type source)
    {
      return source.BaseType;
    }

    public static Type[] GetGenericTypeParameters(this Type source)
    {
      return ((IEnumerable<Type>) source.GetGenericArguments()).Where<Type>((Func<Type, bool>) (t => t.IsGenericParameter)).ToArray<Type>();
    }

    public static T GetCustomAttribute<T>(this MemberInfo source) where T : Attribute
    {
      return Attribute.GetCustomAttribute(source, typeof (T)) as T;
    }

    public static Type GetAttributeType(this CustomAttributeData source)
    {
      return source.Constructor.DeclaringType;
    }

    public static string GetMemberName(this CustomAttributeNamedArgument source)
    {
      return source.MemberInfo.Name;
    }

    public static string GetCultureName(this AssemblyName source)
    {
      return source.CultureName;
    }

    public static IEnumerable<CustomAttributeNamedArgument> GetNamedArguments(
      this CustomAttributeData source)
    {
      return (IEnumerable<CustomAttributeNamedArgument>) source.NamedArguments;
    }

    public static CustomAttributeTypedArgument GetTypedValue(
      this CustomAttributeNamedArgument source)
    {
      return source.TypedValue;
    }

    public static bool GetHasDefaultValue(this ParameterInfo source)
    {
      return source.HasDefaultValue;
    }
  }
}
