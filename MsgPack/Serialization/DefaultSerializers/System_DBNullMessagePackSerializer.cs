// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_DBNullMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class System_DBNullMessagePackSerializer : MessagePackSerializer<DBNull>
  {
    public System_DBNullMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, DBNull objectTree)
    {
      packer.PackNull();
    }

    protected internal override DBNull UnpackFromCore(Unpacker unpacker)
    {
      throw new SerializationException("DBNull value should be nil.");
    }

    protected internal override DBNull UnpackNil()
    {
      return DBNull.Value;
    }
  }
}
