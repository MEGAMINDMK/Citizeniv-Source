// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionCallbackDictionaryMessagePackSerializer`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal class ExpressionCallbackDictionaryMessagePackSerializer<TDictionary, TKey, TValue> : DictionaryMessagePackSerializer<TDictionary, TKey, TValue>
    where TDictionary : IDictionary<TKey, TValue>
  {
    private readonly Func<ExpressionCallbackDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, int, TDictionary> _createInstance;

    public ExpressionCallbackDictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<ExpressionCallbackDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, int, TDictionary> createInstance)
      : base(ownerContext, schema)
    {
      this._createInstance = createInstance;
    }

    protected override TDictionary CreateInstance(int initialCapacity)
    {
      return this._createInstance(this, this.OwnerContext, initialCapacity);
    }
  }
}
