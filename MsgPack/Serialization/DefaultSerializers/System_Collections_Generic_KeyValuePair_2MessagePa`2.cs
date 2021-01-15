// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Generic_KeyValuePair_2MessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_Generic_KeyValuePair_2MessagePackSerializer<TKey, TValue> : MessagePackSerializer<KeyValuePair<TKey, TValue>>
  {
    private readonly MessagePackSerializer<TKey> _keySerializer;
    private readonly MessagePackSerializer<TValue> _valueSerializer;

    public System_Collections_Generic_KeyValuePair_2MessagePackSerializer(
      SerializationContext ownerContext)
      : base(ownerContext)
    {
      this._keySerializer = ownerContext.GetSerializer<TKey>();
      this._valueSerializer = ownerContext.GetSerializer<TValue>();
    }

    protected internal override void PackToCore(
      Packer packer,
      KeyValuePair<TKey, TValue> objectTree)
    {
      packer.PackArrayHeader(2);
      this._keySerializer.PackTo(packer, objectTree.Key);
      this._valueSerializer.PackTo(packer, objectTree.Value);
    }

    protected internal override KeyValuePair<TKey, TValue> UnpackFromCore(
      Unpacker unpacker)
    {
      if (!unpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      TKey key = unpacker.LastReadData.IsNil ? default (TKey) : this._keySerializer.UnpackFrom(unpacker);
      if (!unpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      TValue obj = unpacker.LastReadData.IsNil ? default (TValue) : this._valueSerializer.UnpackFrom(unpacker);
      return new KeyValuePair<TKey, TValue>(key, obj);
    }
  }
}
