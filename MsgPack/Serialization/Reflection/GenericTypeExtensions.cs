// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Reflection.GenericTypeExtensions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace MsgPack.Serialization.Reflection
{
  internal static class GenericTypeExtensions
  {
    public static bool Implements(this Type source, Type genericType)
    {
      return GenericTypeExtensions.EnumerateGenericIntefaces(source, genericType, false).Any<Type>();
    }

    private static IEnumerable<Type> EnumerateGenericIntefaces(
      Type source,
      Type genericType,
      bool includesOwn)
    {
      object obj;
      if (!includesOwn)
        obj = (object) source.GetInterfaces();
      else
        obj = (object) ((IEnumerable<Type>) new Type[1]
        {
          source
        }).Concat<Type>((IEnumerable<Type>) source.GetInterfaces());
      Func<Type, bool> predicate = (Func<Type, bool>) (@interface =>
      {
        if (!@interface.GetIsGenericType())
          return false;
        return !genericType.GetIsGenericTypeDefinition() ? @interface == genericType : @interface.GetGenericTypeDefinition() == genericType;
      });
      return ((IEnumerable<Type>) obj).Where<Type>(predicate).Select<Type, Type>((Func<Type, Type>) (@interface => !source.GetIsGenericTypeDefinition() ? @interface : @interface.GetGenericTypeDefinition()));
    }

    public static string GetName(this Type source)
    {
      if (!source.GetIsGenericType())
        return source.Name;
      return source.Name + (object) '[' + string.Join(", ", ((IEnumerable<Type>) source.GetGenericArguments()).Select<Type, string>((Func<Type, string>) (t => t.GetName()))) + (object) ']';
    }

    public static string GetFullName(this Type source)
    {
      if (source.IsArray)
      {
        Type elementType = source.GetElementType();
        if (!elementType.GetIsGenericType())
          return source.FullName;
        return 1 < source.GetArrayRank() ? elementType.GetFullName() + "[*]" : elementType.GetFullName() + "[]";
      }
      if (!source.GetIsGenericType())
        return source.FullName;
      return source.Namespace + (object) ReflectionAbstractions.TypeDelimiter + source.Name + (object) '[' + string.Join(", ", ((IEnumerable<Type>) source.GetGenericArguments()).Select<Type, string>((Func<Type, string>) (t => t.GetFullName()))) + (object) ']';
    }
  }
}
