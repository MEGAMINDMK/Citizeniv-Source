// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializerNameResolver
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Threading;

namespace MsgPack.Serialization
{
  internal static class DefaultSerializerNameResolver
  {
    private const string Delimiter = ".";
    private static int _serializerNameSequence;

    public static void ResolveTypeName(
      bool useSequence,
      Type targetType,
      string callerNamespace,
      out string serializerTypeName,
      out string serializerTypeNamespace)
    {
      int? nullable = useSequence ? new int?(Interlocked.Increment(ref DefaultSerializerNameResolver._serializerNameSequence)) : new int?();
      serializerTypeName = IdentifierUtility.EscapeTypeName(targetType) + "Serializer" + (object) nullable;
      serializerTypeNamespace = callerNamespace + ".Generated";
    }
  }
}
