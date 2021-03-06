﻿// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_StringMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_StringMessagePackSerializer : MessagePackSerializer<string>
  {
    public System_StringMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, string value)
    {
      packer.PackString(value);
    }

    protected internal override string UnpackFromCore(Unpacker unpacker)
    {
      MessagePackObject lastReadData = unpacker.LastReadData;
      return !lastReadData.IsNil ? lastReadData.DeserializeAsString() : (string) null;
    }
  }
}
