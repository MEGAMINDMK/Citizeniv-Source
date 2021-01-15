// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.CollectionSerializerHelpers
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.CollectionSerializers
{
  internal static class CollectionSerializerHelpers
  {
    public static readonly Type[] CollectionConstructorTypes = new Type[2]
    {
      typeof (SerializationContext),
      typeof (PolymorphismSchema)
    };
  }
}
