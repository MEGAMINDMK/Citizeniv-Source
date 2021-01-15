// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_VersionMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_VersionMessagePackSerializer : MessagePackSerializer<Version>
  {
    public System_VersionMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, Version objectTree)
    {
      packer.PackArrayHeader(4);
      packer.Pack(objectTree.Major);
      packer.Pack(objectTree.Minor);
      packer.Pack(objectTree.Build);
      packer.Pack(objectTree.Revision);
    }

    protected internal override Version UnpackFromCore(Unpacker unpacker)
    {
      long num = unpacker.LastReadData.AsInt64();
      int[] numArray = new int[4];
      for (int index = 0; (long) index < num && index < 4; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        numArray[index] = unpacker.LastReadData.AsInt32();
      }
      return new Version(numArray[0], numArray[1], numArray[2], numArray[3]);
    }
  }
}
