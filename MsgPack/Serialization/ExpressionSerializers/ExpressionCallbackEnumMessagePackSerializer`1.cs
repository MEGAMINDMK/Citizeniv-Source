// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionCallbackEnumMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal class ExpressionCallbackEnumMessagePackSerializer<TEnum> : EnumMessagePackSerializer<TEnum>
    where TEnum : struct
  {
    private readonly Action<ExpressionCallbackEnumMessagePackSerializer<TEnum>, Packer, TEnum> _packUnderlyingValueTo;
    private readonly Func<ExpressionCallbackEnumMessagePackSerializer<TEnum>, MessagePackObject, TEnum> _unpackFromUnderlyingValue;

    public ExpressionCallbackEnumMessagePackSerializer(
      SerializationContext ownerContext,
      EnumSerializationMethod serializationMethod,
      Action<ExpressionCallbackEnumMessagePackSerializer<TEnum>, Packer, TEnum> packUnderlyingValueTo,
      Func<ExpressionCallbackEnumMessagePackSerializer<TEnum>, MessagePackObject, TEnum> unpackFromUnderlyingValue)
      : base(ownerContext, serializationMethod)
    {
      this._packUnderlyingValueTo = packUnderlyingValueTo;
      this._unpackFromUnderlyingValue = unpackFromUnderlyingValue;
    }

    protected internal override void PackUnderlyingValueTo(Packer packer, TEnum enumValue)
    {
      this._packUnderlyingValueTo(this, packer, enumValue);
    }

    protected internal override TEnum UnpackFromUnderlyingValue(MessagePackObject messagePackObject)
    {
      return this._unpackFromUnderlyingValue(this, messagePackObject);
    }
  }
}
