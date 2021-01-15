// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.CallbackEnumMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class CallbackEnumMessagePackSerializer<TEnum> : EnumMessagePackSerializer<TEnum>
    where TEnum : struct
  {
    private readonly Action<SerializationContext, Packer, TEnum> _packUnderlyingValueTo;
    private readonly Func<SerializationContext, MessagePackObject, TEnum> _unpackFromUnderlyingValue;

    public CallbackEnumMessagePackSerializer(
      SerializationContext ownerContext,
      EnumSerializationMethod serializationMethod,
      Action<SerializationContext, Packer, TEnum> packUnderlyingValueTo,
      Func<SerializationContext, MessagePackObject, TEnum> unpackFromUnderlyingValue)
      : base(ownerContext, serializationMethod)
    {
      this._packUnderlyingValueTo = packUnderlyingValueTo;
      this._unpackFromUnderlyingValue = unpackFromUnderlyingValue;
    }

    protected internal override void PackUnderlyingValueTo(Packer packer, TEnum enumValue)
    {
      this._packUnderlyingValueTo(this.OwnerContext, packer, enumValue);
    }

    protected internal override TEnum UnpackFromUnderlyingValue(MessagePackObject messagePackObject)
    {
      return this._unpackFromUnderlyingValue(this.OwnerContext, messagePackObject);
    }
  }
}
