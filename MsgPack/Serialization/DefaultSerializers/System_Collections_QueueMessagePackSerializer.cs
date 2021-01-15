// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_QueueMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_QueueMessagePackSerializer : MessagePackSerializer<Queue>, ICollectionInstanceFactory
  {
    public System_Collections_QueueMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, Queue objectTree)
    {
      packer.PackArrayHeader(objectTree.Count);
      foreach (object obj in objectTree)
        packer.PackObject(obj);
    }

    protected internal override Queue UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      Queue collection = new Queue(UnpackHelpers.GetItemsCount(unpacker));
      this.UnpackToCore(unpacker, collection);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, Queue collection)
    {
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        collection.Enqueue((object) unpacker.LastReadData);
      }
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new Queue(initialCapacity);
    }
  }
}
