// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Generic_ListOfMessagePackObjectMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class System_Collections_Generic_ListOfMessagePackObjectMessagePackSerializer : MessagePackSerializer<List<MessagePackObject>>, ICollectionInstanceFactory
  {
    public System_Collections_Generic_ListOfMessagePackObjectMessagePackSerializer(
      SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, List<MessagePackObject> objectTree)
    {
      packer.PackArrayHeader(objectTree.Count);
      foreach (MessagePackObject messagePackObject in objectTree)
        messagePackObject.PackToMessage(packer, (PackingOptions) null);
    }

    protected internal override List<MessagePackObject> UnpackFromCore(
      Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      List<MessagePackObject> collection = new List<MessagePackObject>(itemsCount);
      System_Collections_Generic_ListOfMessagePackObjectMessagePackSerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(
      Unpacker unpacker,
      List<MessagePackObject> collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      System_Collections_Generic_ListOfMessagePackObjectMessagePackSerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(
      Unpacker unpacker,
      List<MessagePackObject> collection,
      int count)
    {
      for (int index = 0; index < count; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        collection.Add(unpacker.LastReadData);
      }
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new List<MessagePackObject>(initialCapacity);
    }
  }
}
