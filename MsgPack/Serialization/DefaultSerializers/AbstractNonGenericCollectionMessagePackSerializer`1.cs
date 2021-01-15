// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.AbstractNonGenericCollectionMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Polymorphic;
using System;
using System.Collections;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class AbstractNonGenericCollectionMessagePackSerializer<TCollection> : NonGenericCollectionMessagePackSerializer<TCollection>
    where TCollection : ICollection
  {
    private readonly ICollectionInstanceFactory _concreteCollectionInstanceFactory;
    private readonly IMessagePackSingleObjectSerializer _concreteSerializer;

    public AbstractNonGenericCollectionMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
      AbstractCollectionSerializerHelper.GetConcreteSerializer(ownerContext, schema, typeof (TCollection), targetType, typeof (EnumerableMessagePackSerializerBase<,>), out this._concreteCollectionInstanceFactory, out this._concreteSerializer);
    }

    protected internal override TCollection UnpackFromCore(Unpacker unpacker)
    {
      return this._concreteSerializer is IPolymorphicDeserializer concreteSerializer ? (TCollection) concreteSerializer.PolymorphicUnpackFrom(unpacker) : (TCollection) this._concreteSerializer.UnpackFrom(unpacker);
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return (TCollection) this._concreteCollectionInstanceFactory.CreateInstance(initialCapacity);
    }
  }
}
