// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.MsgPack_MessagePackObjectMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class MsgPack_MessagePackObjectMessagePackSerializer : MessagePackSerializer<MessagePackObject>
  {
    public MsgPack_MessagePackObjectMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, MessagePackObject value)
    {
      value.PackToMessage(packer, new PackingOptions());
    }

    protected internal override MessagePackObject UnpackFromCore(Unpacker unpacker)
    {
      return unpacker.IsArrayHeader || unpacker.IsMapHeader ? unpacker.UnpackSubtreeData() : unpacker.LastReadData;
    }
  }
}
