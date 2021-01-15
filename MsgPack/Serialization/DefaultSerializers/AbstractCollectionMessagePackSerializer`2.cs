// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.AbstractCollectionMessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Polymorphic;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class AbstractCollectionMessagePackSerializer<TCollection, TItem> : CollectionMessagePackSerializer<TCollection, TItem>
    where TCollection : ICollection<TItem>
  {
    private readonly ICollectionInstanceFactory _concreteCollectionInstanceFactory;
    private readonly IPolymorphicDeserializer _polymorphicDeserializer;

    public AbstractCollectionMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
      IMessagePackSingleObjectSerializer serializer;
      AbstractCollectionSerializerHelper.GetConcreteSerializer(ownerContext, schema, typeof (TCollection), targetType, typeof (EnumerableMessagePackSerializerBase<,>), out this._concreteCollectionInstanceFactory, out serializer);
      this._polymorphicDeserializer = serializer as IPolymorphicDeserializer;
    }

    internal override TCollection InternalUnpackFromCore(Unpacker unpacker)
    {
      return this._polymorphicDeserializer != null ? (TCollection) this._polymorphicDeserializer.PolymorphicUnpackFrom(unpacker) : base.InternalUnpackFromCore(unpacker);
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return (TCollection) this._concreteCollectionInstanceFactory.CreateInstance(initialCapacity);
    }
  }
}
