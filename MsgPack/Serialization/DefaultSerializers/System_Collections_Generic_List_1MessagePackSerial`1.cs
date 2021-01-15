// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Generic_List_1MessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class System_Collections_Generic_List_1MessagePackSerializer<T> : MessagePackSerializer<List<T>>, ICollectionInstanceFactory
  {
    private readonly MessagePackSerializer<T> _itemSerializer;

    public System_Collections_Generic_List_1MessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema itemsSchema)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<T>((object) itemsSchema);
    }

    protected internal override void PackToCore(Packer packer, List<T> objectTree)
    {
      PackerUnpackerExtensions.PackCollectionCore<T>(packer, (IEnumerable<T>) objectTree, this._itemSerializer);
    }

    protected internal override List<T> UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      List<T> collection = new List<T>(itemsCount);
      this.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, List<T> collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      this.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private void UnpackToCore(Unpacker unpacker, List<T> collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        if (unpacker.IsCollectionHeader)
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            collection.Add(this._itemSerializer.UnpackFromCore(unpacker1));
        }
        else
          collection.Add(this._itemSerializer.UnpackFromCore(unpacker));
      }
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new List<T>(initialCapacity);
    }
  }
}
