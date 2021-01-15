// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_UriMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_UriMessagePackSerializer : MessagePackSerializer<Uri>
  {
    public System_UriMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, Uri objectTree)
    {
      packer.PackString(objectTree.ToString());
    }

    protected internal override Uri UnpackFromCore(Unpacker unpacker)
    {
      return new Uri(unpacker.LastReadData.DeserializeAsString());
    }
  }
}
