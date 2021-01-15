// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Text_StringBuilderMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Text;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Text_StringBuilderMessagePackSerializer : MessagePackSerializer<StringBuilder>
  {
    public System_Text_StringBuilderMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, StringBuilder value)
    {
      packer.PackString(value.ToString());
    }

    protected internal override StringBuilder UnpackFromCore(Unpacker unpacker)
    {
      MessagePackObject lastReadData = unpacker.LastReadData;
      return !lastReadData.IsNil ? new StringBuilder(lastReadData.DeserializeAsString()) : (StringBuilder) null;
    }
  }
}
