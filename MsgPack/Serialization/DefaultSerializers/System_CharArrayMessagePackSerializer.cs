// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_CharArrayMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_CharArrayMessagePackSerializer : MessagePackSerializer<char[]>
  {
    public System_CharArrayMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, char[] value)
    {
      if (value == null)
        packer.PackNull();
      else
        packer.PackString((IEnumerable<char>) value);
    }

    protected internal override char[] UnpackFromCore(Unpacker unpacker)
    {
      MessagePackObject lastReadData = unpacker.LastReadData;
      return !lastReadData.IsNil ? lastReadData.AsCharArray() : (char[]) null;
    }
  }
}
