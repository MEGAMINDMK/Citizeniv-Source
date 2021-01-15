// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._SerializationContext
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _SerializationContext
  {
    public static readonly MethodInfo GetSerializer1_Method = typeof (SerializationContext).GetMethod("GetSerializer", ReflectionAbstractions.EmptyTypes);
    public static readonly MethodInfo GetSerializer1_Parameter_Method = typeof (SerializationContext).GetMethod("GetSerializer", new Type[1]
    {
      typeof (object)
    });
  }
}
