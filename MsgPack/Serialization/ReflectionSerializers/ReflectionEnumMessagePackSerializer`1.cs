// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionEnumMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal class ReflectionEnumMessagePackSerializer<T> : EnumMessagePackSerializer<T>
    where T : struct
  {
    public ReflectionEnumMessagePackSerializer(SerializationContext context)
      : base(context, EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context, typeof (T), EnumMemberSerializationMethod.Default))
    {
    }

    protected internal override void PackUnderlyingValueTo(Packer packer, T enumValue)
    {
      packer.Pack(ulong.Parse(((IFormattable) enumValue).ToString("D", (IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture));
    }

    protected internal override T UnpackFromUnderlyingValue(MessagePackObject messagePackObject)
    {
      return (T) Enum.Parse(typeof (T), messagePackObject.ToString(), false);
    }
  }
}
