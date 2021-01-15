// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_ByteMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_ByteMessagePackSerializer : MessagePackSerializer<byte>
  {
    public System_ByteMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, byte value)
    {
      packer.Pack(value);
    }

    protected internal override sealed byte UnpackFromCore(Unpacker unpacker)
    {
      try
      {
        return unpacker.LastReadData.AsByte();
      }
      catch (InvalidOperationException ex)
      {
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The unpacked value is not '{0}' type. {1}", (object) typeof (byte), (object) ex.Message));
      }
    }
  }
}
