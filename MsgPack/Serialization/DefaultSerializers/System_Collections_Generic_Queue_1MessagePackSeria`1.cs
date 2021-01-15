// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Generic_Queue_1MessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_Generic_Queue_1MessagePackSerializer<TItem> : MessagePackSerializer<Queue<TItem>>, ICollectionInstanceFactory
  {
    private readonly MessagePackSerializer<TItem> _itemSerializer;

    public System_Collections_Generic_Queue_1MessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<TItem>();
    }

    protected internal override void PackToCore(Packer packer, Queue<TItem> objectTree)
    {
      packer.PackArrayHeader(objectTree.Count);
      foreach (TItem objectTree1 in objectTree)
        this._itemSerializer.PackTo(packer, objectTree1);
    }

    protected internal override Queue<TItem> UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      Queue<TItem> collection = new Queue<TItem>(UnpackHelpers.GetItemsCount(unpacker));
      this.UnpackToCore(unpacker, collection);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, Queue<TItem> collection)
    {
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
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
        collection.Enqueue(obj);
      }
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new Queue<TItem>(initialCapacity);
    }
  }
}
