// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.UnixEpocFileTimeMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Runtime.InteropServices.ComTypes;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class UnixEpocFileTimeMessagePackSerializer : MessagePackSerializer<FILETIME>
  {
    public UnixEpocFileTimeMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, FILETIME objectTree)
    {
      packer.Pack(MessagePackConvert.FromDateTime(objectTree.ToDateTime()));
    }

    protected internal override FILETIME UnpackFromCore(Unpacker unpacker)
    {
      return MessagePackConvert.ToDateTime(unpacker.LastReadData.AsInt64()).ToWin32FileTimeUtc();
    }
  }
}
