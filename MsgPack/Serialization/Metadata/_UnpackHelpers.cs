// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._UnpackHelpers
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _UnpackHelpers
  {
    private static readonly Dictionary<Type, MethodInfo> _directUnpackMethods = _UnpackHelpers.GetDirectUnpackMethods();
    public static readonly MethodInfo GetItemsCount = FromExpression.ToMethod<Unpacker, int>((Expression<Func<Unpacker, int>>) (unpacker => UnpackHelpers.GetItemsCount(unpacker)));
    public static readonly MethodInfo GetEqualityComparer_1Method = typeof (UnpackHelpers).GetMethod("GetEqualityComparer");

    private static Dictionary<Type, MethodInfo> GetDirectUnpackMethods()
    {
      return new Dictionary<Type, MethodInfo>(14)
      {
        {
          typeof (sbyte),
          typeof (UnpackHelpers).GetMethod("UnpackSByteValue")
        },
        {
          typeof (sbyte?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableSByteValue")
        },
        {
          typeof (short),
          typeof (UnpackHelpers).GetMethod("UnpackInt16Value")
        },
        {
          typeof (short?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableInt16Value")
        },
        {
          typeof (int),
          typeof (UnpackHelpers).GetMethod("UnpackInt32Value")
        },
        {
          typeof (int?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableInt32Value")
        },
        {
          typeof (long),
          typeof (UnpackHelpers).GetMethod("UnpackInt64Value")
        },
        {
          typeof (long?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableInt64Value")
        },
        {
          typeof (byte),
          typeof (UnpackHelpers).GetMethod("UnpackByteValue")
        },
        {
          typeof (byte?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableByteValue")
        },
        {
          typeof (ushort),
          typeof (UnpackHelpers).GetMethod("UnpackUInt16Value")
        },
        {
          typeof (ushort?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableUInt16Value")
        },
        {
          typeof (uint),
          typeof (UnpackHelpers).GetMethod("UnpackUInt32Value")
        },
        {
          typeof (uint?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableUInt32Value")
        },
        {
          typeof (ulong),
          typeof (UnpackHelpers).GetMethod("UnpackUInt64Value")
        },
        {
          typeof (ulong?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableUInt64Value")
        },
        {
          typeof (float),
          typeof (UnpackHelpers).GetMethod("UnpackSingleValue")
        },
        {
          typeof (float?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableSingleValue")
        },
        {
          typeof (double),
          typeof (UnpackHelpers).GetMethod("UnpackDoubleValue")
        },
        {
          typeof (double?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableDoubleValue")
        },
        {
          typeof (bool),
          typeof (UnpackHelpers).GetMethod("UnpackBooleanValue")
        },
        {
          typeof (bool?),
          typeof (UnpackHelpers).GetMethod("UnpackNullableBooleanValue")
        },
        {
          typeof (string),
          typeof (UnpackHelpers).GetMethod("UnpackStringValue")
        },
        {
          typeof (byte[]),
          typeof (UnpackHelpers).GetMethod("UnpackBinaryValue")
        }
      };
    }

    public static MethodInfo GetDirectUnpackMethod(Type type)
    {
      MethodInfo methodInfo;
      _UnpackHelpers._directUnpackMethods.TryGetValue(type, out methodInfo);
      return methodInfo;
    }
  }
}
