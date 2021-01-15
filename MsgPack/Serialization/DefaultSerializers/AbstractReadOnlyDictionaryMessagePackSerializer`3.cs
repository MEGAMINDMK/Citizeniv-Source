// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.AbstractReadOnlyDictionaryMessagePackSerializer`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Polymorphic;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class AbstractReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue> : ReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>
    where TDictionary : IReadOnlyDictionary<TKey, TValue>
  {
    private readonly ICollectionInstanceFactory _concreteCollectionInstanceFactory;
    private readonly IPolymorphicDeserializer _polymorphicDeserializer;
    private readonly IMessagePackSingleObjectSerializer _concreteDeserializer;

    public AbstractReadOnlyDictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
      IMessagePackSingleObjectSerializer serializer;
      AbstractCollectionSerializerHelper.GetConcreteSerializer(ownerContext, schema, typeof (TDictionary), targetType, typeof (DictionaryMessagePackSerializer<,,>), out this._concreteCollectionInstanceFactory, out serializer);
      this._polymorphicDeserializer = serializer as IPolymorphicDeserializer;
      this._concreteDeserializer = serializer;
    }

    internal override TDictionary InternalUnpackFromCore(Unpacker unpacker)
    {
      if (this._polymorphicDeserializer != null)
        return (TDictionary) this._polymorphicDeserializer.PolymorphicUnpackFrom(unpacker);
      return this._concreteDeserializer != null ? (TDictionary) this._concreteDeserializer.UnpackFrom(unpacker) : base.InternalUnpackFromCore(unpacker);
    }

    protected override TDictionary CreateInstance(int initialCapacity)
    {
      return (TDictionary) this._concreteCollectionInstanceFactory.CreateInstance(initialCapacity);
    }
  }
}
