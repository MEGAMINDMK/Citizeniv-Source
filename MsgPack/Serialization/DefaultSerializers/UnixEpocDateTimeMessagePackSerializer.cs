// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.UnixEpocDateTimeMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class UnixEpocDateTimeMessagePackSerializer : MessagePackSerializer<DateTime>
  {
    public UnixEpocDateTimeMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, DateTime objectTree)
    {
      packer.Pack(MessagePackConvert.FromDateTime(objectTree));
    }

    protected internal override DateTime UnpackFromCore(Unpacker unpacker)
    {
      return MessagePackConvert.ToDateTime(unpacker.LastReadData.AsInt64());
    }
  }
}
