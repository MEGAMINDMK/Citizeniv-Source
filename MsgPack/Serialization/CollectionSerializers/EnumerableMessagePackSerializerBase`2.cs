// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.EnumerableMessagePackSerializerBase`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class EnumerableMessagePackSerializerBase<TCollection, TItem> : MessagePackSerializer<TCollection>, ICollectionInstanceFactory
    where TCollection : IEnumerable<TItem>
  {
    private readonly MessagePackSerializer<TItem> _itemSerializer;

    internal MessagePackSerializer<TItem> ItemSerializer
    {
      get
      {
        return this._itemSerializer;
      }
    }

    protected EnumerableMessagePackSerializerBase(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<TItem>((object) (schema ?? PolymorphismSchema.Default).ItemSchema);
    }

    protected abstract TCollection CreateInstance(int initialCapacity);

    object ICollectionInstanceFactory.CreateInstance(int initialCapacity)
    {
      return (object) this.CreateInstance(initialCapacity);
    }

    protected internal override sealed void UnpackToCore(Unpacker unpacker, TCollection collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      this.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    internal void UnpackToCore(Unpacker unpacker, TCollection collection, int itemsCount)
    {
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        TItem obj;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          obj = this._itemSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            obj = this._itemSerializer.UnpackFrom(unpacker1);
        }
        this.AddItem(collection, obj);
      }
    }

    protected virtual void AddItem(TCollection collection, TItem item)
    {
      throw SerializationExceptions.NewUnpackToIsNotSupported(typeof (TCollection), (Exception) null);
    }
  }
}
