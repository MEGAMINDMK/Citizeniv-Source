// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.CallbackReadOnlyDictionaryMessagePackSerializer`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class CallbackReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue> : ReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>
    where TDictionary : IReadOnlyDictionary<TKey, TValue>
  {
    private readonly Func<SerializationContext, int, TDictionary> _createInstance;
    private readonly Action<SerializationContext, TDictionary, TKey, TValue> _addItem;

    public CallbackReadOnlyDictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<SerializationContext, int, TDictionary> createInstance,
      Action<SerializationContext, TDictionary, TKey, TValue> addItem)
      : base(ownerContext, schema)
    {
      this._createInstance = createInstance;
      this._addItem = addItem;
    }

    protected override TDictionary CreateInstance(int initialCapacity)
    {
      return this._createInstance(this.OwnerContext, initialCapacity);
    }

    protected override void AddItem(TDictionary dictionary, TKey key, TValue value)
    {
      if (this._addItem != null)
        this._addItem(this.OwnerContext, dictionary, key, value);
      else
        base.AddItem(dictionary, key, value);
    }
  }
}
