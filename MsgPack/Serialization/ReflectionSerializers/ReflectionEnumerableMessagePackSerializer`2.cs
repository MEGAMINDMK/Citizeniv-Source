// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionEnumerableMessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal sealed class ReflectionEnumerableMessagePackSerializer<TCollection, TItem> : EnumerableMessagePackSerializer<TCollection, TItem>
    where TCollection : IEnumerable<TItem>
  {
    private readonly Func<int, TCollection> _factory;
    private readonly Action<TCollection, TItem> _addItem;

    public ReflectionEnumerableMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema itemsSchema)
      : base(ownerContext, itemsSchema)
    {
      this._factory = ReflectionSerializerHelper.CreateCollectionInstanceFactory<TCollection, TItem>(targetType);
      this._addItem = ReflectionSerializerHelper.GetAddItem<TCollection, TItem>(targetType);
    }

    protected internal override TCollection UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      TCollection instance = this.CreateInstance(itemsCount);
      this.UnpackToCore(unpacker, instance, itemsCount);
      return instance;
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return this._factory(initialCapacity);
    }

    protected override void AddItem(TCollection collection, TItem item)
    {
      this._addItem(collection, item);
    }
  }
}
