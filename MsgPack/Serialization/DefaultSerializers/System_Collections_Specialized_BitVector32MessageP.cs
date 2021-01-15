// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Specialized_BitVector32MessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_Specialized_BitVector32MessagePackSerializer : MessagePackSerializer<BitVector32>
  {
    public System_Collections_Specialized_BitVector32MessagePackSerializer(
      SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, BitVector32 value)
    {
      packer.Pack(value.Data);
    }

    protected internal override sealed BitVector32 UnpackFromCore(Unpacker unpacker)
    {
      int data;
      try
      {
        data = unpacker.LastReadData.AsInt32();
      }
      catch (InvalidOperationException ex)
      {
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The unpacked value is not '{0}' type. {1}", (object) typeof (int), (object) ex.Message));
      }
      return new BitVector32(data);
    }
  }
}
