// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Generic_Stack_1MessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_Generic_Stack_1MessagePackSerializer<TItem> : MessagePackSerializer<Stack<TItem>>, ICollectionInstanceFactory
  {
    private readonly MessagePackSerializer<TItem> _itemSerializer;

    public System_Collections_Generic_Stack_1MessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<TItem>();
    }

    protected internal override void PackToCore(Packer packer, Stack<TItem> objectTree)
    {
      packer.PackArrayHeader(objectTree.Count);
      foreach (TItem objectTree1 in objectTree)
        this._itemSerializer.PackTo(packer, objectTree1);
    }

    protected internal override Stack<TItem> UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      return new Stack<TItem>(this.UnpackItemsInReverseOrder(unpacker, UnpackHelpers.GetItemsCount(unpacker)));
    }

    protected internal override void UnpackToCore(Unpacker unpacker, Stack<TItem> collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      foreach (TItem obj in this.UnpackItemsInReverseOrder(unpacker, UnpackHelpers.GetItemsCount(unpacker)))
        collection.Push(obj);
    }

    private IEnumerable<TItem> UnpackItemsInReverseOrder(Unpacker unpacker, int count)
    {
      TItem[] objArray = new TItem[count];
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        for (int index = objArray.Length - 1; index >= 0; --index)
        {
          if (!unpacker1.Read())
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          objArray[index] = this._itemSerializer.UnpackFrom(unpacker1);
        }
      }
      return (IEnumerable<TItem>) objArray;
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new Stack<TItem>(initialCapacity);
    }
  }
}
