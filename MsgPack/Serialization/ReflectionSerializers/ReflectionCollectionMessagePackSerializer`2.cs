// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionCollectionMessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal sealed class ReflectionCollectionMessagePackSerializer<TCollection, TItem> : CollectionMessagePackSerializer<TCollection, TItem>
    where TCollection : ICollection<TItem>
  {
    private readonly Func<int, TCollection> _factory;

    public ReflectionCollectionMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema itemsSchema)
      : base(ownerContext, itemsSchema)
    {
      this._factory = ReflectionSerializerHelper.CreateCollectionInstanceFactory<TCollection, TItem>(targetType);
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return this._factory(initialCapacity);
    }
  }
}
