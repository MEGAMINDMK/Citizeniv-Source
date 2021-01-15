// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionDictionaryMessagePackSerializer`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal sealed class ReflectionDictionaryMessagePackSerializer<TDictionary, TKey, TValue> : DictionaryMessagePackSerializer<TDictionary, TKey, TValue>
    where TDictionary : IDictionary<TKey, TValue>
  {
    private readonly Func<int, TDictionary> _factory;

    public ReflectionDictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema itemsSchema)
      : base(ownerContext, itemsSchema)
    {
      this._factory = ReflectionSerializerHelper.CreateCollectionInstanceFactory<TDictionary, TKey>(targetType);
    }

    protected override TDictionary CreateInstance(int initialCapacity)
    {
      return this._factory(initialCapacity);
    }
  }
}
