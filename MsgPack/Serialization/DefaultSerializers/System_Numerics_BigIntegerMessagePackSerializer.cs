// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Numerics_BigIntegerMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Numerics_BigIntegerMessagePackSerializer : MessagePackSerializer<BigInteger>
  {
    public System_Numerics_BigIntegerMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, BigInteger value)
    {
      packer.PackRaw(value.ToByteArray());
    }

    protected internal override sealed BigInteger UnpackFromCore(Unpacker unpacker)
    {
      try
      {
        return new BigInteger(unpacker.LastReadData.AsBinary());
      }
      catch (ArgumentException ex)
      {
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The unpacked value is not expected type. {0}", (object) ex.Message), (Exception) ex);
      }
      catch (InvalidOperationException ex)
      {
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The unpacked value is not expected type. {0}", (object) ex.Message), (Exception) ex);
      }
    }
  }
}
