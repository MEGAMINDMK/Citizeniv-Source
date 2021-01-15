// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_ByteArrayMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_ByteArrayMessagePackSerializer : MessagePackSerializer<byte[]>
  {
    public System_ByteArrayMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, byte[] value)
    {
      packer.PackBinary(value);
    }

    protected internal override byte[] UnpackFromCore(Unpacker unpacker)
    {
      MessagePackObject lastReadData = unpacker.LastReadData;
      return !lastReadData.IsNil ? lastReadData.AsBinary() : (byte[]) null;
    }
  }
}
