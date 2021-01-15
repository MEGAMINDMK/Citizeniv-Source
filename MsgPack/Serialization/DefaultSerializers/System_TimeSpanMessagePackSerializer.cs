// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_TimeSpanMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_TimeSpanMessagePackSerializer : MessagePackSerializer<TimeSpan>
  {
    public System_TimeSpanMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, TimeSpan value)
    {
      packer.Pack(value.Ticks);
    }

    protected internal override sealed TimeSpan UnpackFromCore(Unpacker unpacker)
    {
      long ticks;
      try
      {
        ticks = unpacker.LastReadData.AsInt64();
      }
      catch (InvalidOperationException ex)
      {
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The unpacked value is not '{0}' type. {1}", (object) typeof (long), (object) ex.Message));
      }
      return new TimeSpan(ticks);
    }
  }
}
