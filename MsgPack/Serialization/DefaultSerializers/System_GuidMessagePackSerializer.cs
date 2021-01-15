// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_GuidMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_GuidMessagePackSerializer : MessagePackSerializer<Guid>
  {
    public System_GuidMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, Guid value)
    {
      packer.PackRaw(value.ToByteArray());
    }

    protected internal override sealed Guid UnpackFromCore(Unpacker unpacker)
    {
      try
      {
        return new Guid(unpacker.LastReadData.AsBinary());
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
