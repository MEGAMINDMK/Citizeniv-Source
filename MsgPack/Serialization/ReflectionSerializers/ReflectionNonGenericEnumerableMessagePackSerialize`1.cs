// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionNonGenericEnumerableMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal sealed class ReflectionNonGenericEnumerableMessagePackSerializer<TCollection> : NonGenericEnumerableMessagePackSerializer<TCollection>
    where TCollection : IEnumerable
  {
    private readonly Func<int, TCollection> _factory;
    private readonly Action<TCollection, object> _addItem;

    public ReflectionNonGenericEnumerableMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema itemsSchema)
      : base(ownerContext, itemsSchema)
    {
      this._factory = ReflectionSerializerHelper.CreateCollectionInstanceFactory<TCollection, object>(targetType);
      this._addItem = ReflectionSerializerHelper.GetAddItem<TCollection, object>(targetType);
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

    protected override void AddItem(TCollection collection, object item)
    {
      this._addItem(collection, item);
    }
  }
}
