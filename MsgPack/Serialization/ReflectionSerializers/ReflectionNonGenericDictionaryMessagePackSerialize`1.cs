// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionNonGenericDictionaryMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal sealed class ReflectionNonGenericDictionaryMessagePackSerializer<TDictionary> : NonGenericDictionaryMessagePackSerializer<TDictionary>
    where TDictionary : IDictionary
  {
    private readonly Func<int, TDictionary> _factory;

    public ReflectionNonGenericDictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema itemsSchema)
      : base(ownerContext, itemsSchema)
    {
      this._factory = ReflectionSerializerHelper.CreateCollectionInstanceFactory<TDictionary, object>(targetType);
    }

    protected override TDictionary CreateInstance(int initialCapacity)
    {
      return this._factory(initialCapacity);
    }
  }
}
