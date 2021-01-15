// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EnumMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization
{
  public abstract class EnumMessagePackSerializer<TEnum> : MessagePackSerializer<TEnum>, ICustomizableEnumSerializer
    where TEnum : struct
  {
    private readonly Type _underlyingType;
    private EnumSerializationMethod _serializationMethod;

    protected EnumMessagePackSerializer(
      SerializationContext ownerContext,
      EnumSerializationMethod serializationMethod)
      : base(ownerContext)
    {
      if (!typeof (TEnum).GetIsEnum())
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not enum.", (object) typeof (TEnum)));
      this._serializationMethod = serializationMethod;
      this._underlyingType = Enum.GetUnderlyingType(typeof (TEnum));
    }

    protected internal override sealed void PackToCore(Packer packer, TEnum objectTree)
    {
      if (this._serializationMethod == EnumSerializationMethod.ByUnderlyingValue)
        this.PackUnderlyingValueTo(packer, objectTree);
      else
        packer.PackString(objectTree.ToString());
    }

    protected internal abstract void PackUnderlyingValueTo(Packer packer, TEnum enumValue);

    protected internal override sealed TEnum UnpackFromCore(Unpacker unpacker)
    {
      if (unpacker.LastReadData.IsRaw)
      {
        string str = unpacker.LastReadData.AsString();
        TEnum result;
        if (!Enum.TryParse<TEnum>(str, false, out result))
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Name '{0}' is not member of enum type '{1}'.", (object) str, (object) typeof (TEnum)));
        return result;
      }
      if (unpacker.LastReadData.IsTypeOf(this._underlyingType).GetValueOrDefault())
        return this.UnpackFromUnderlyingValue(unpacker.LastReadData);
      throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not underlying type of enum type '{1}'.", (object) unpacker.LastReadData.UnderlyingType, (object) typeof (TEnum)));
    }

    protected internal abstract TEnum UnpackFromUnderlyingValue(MessagePackObject messagePackObject);

    ICustomizableEnumSerializer ICustomizableEnumSerializer.GetCopyAs(
      EnumSerializationMethod method)
    {
      if (method == this._serializationMethod)
        return (ICustomizableEnumSerializer) this;
      EnumMessagePackSerializer<TEnum> messagePackSerializer = this.MemberwiseClone() as EnumMessagePackSerializer<TEnum>;
      messagePackSerializer._serializationMethod = method;
      return (ICustomizableEnumSerializer) messagePackSerializer;
    }
  }
}
