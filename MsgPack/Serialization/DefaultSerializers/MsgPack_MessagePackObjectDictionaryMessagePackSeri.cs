// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.MsgPack_MessagePackObjectDictionaryMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class MsgPack_MessagePackObjectDictionaryMessagePackSerializer : MessagePackSerializer<MessagePackObjectDictionary>, ICollectionInstanceFactory
  {
    public MsgPack_MessagePackObjectDictionaryMessagePackSerializer(
      SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(
      Packer packer,
      MessagePackObjectDictionary objectTree)
    {
      packer.PackMapHeader(objectTree.Count);
      foreach (KeyValuePair<MessagePackObject, MessagePackObject> keyValuePair in objectTree)
      {
        keyValuePair.Key.PackToMessage(packer, (PackingOptions) null);
        keyValuePair.Value.PackToMessage(packer, (PackingOptions) null);
      }
    }

    protected internal override MessagePackObjectDictionary UnpackFromCore(
      Unpacker unpacker)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      MessagePackObjectDictionary collection = new MessagePackObjectDictionary(itemsCount);
      MsgPack_MessagePackObjectDictionaryMessagePackSerializer.UnpackToCore(unpacker, itemsCount, collection);
      return collection;
    }

    protected internal override void UnpackToCore(
      Unpacker unpacker,
      MessagePackObjectDictionary collection)
    {
      MsgPack_MessagePackObjectDictionaryMessagePackSerializer.UnpackToCore(unpacker, UnpackHelpers.GetItemsCount(unpacker), collection);
    }

    private static void UnpackToCore(
      Unpacker unpacker,
      int count,
      MessagePackObjectDictionary collection)
    {
      for (int index = 0; index < count; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        MessagePackObject lastReadData = unpacker.LastReadData;
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        if (unpacker.IsCollectionHeader)
        {
          MessagePackObject result;
          if (!unpacker.UnpackSubtreeDataCore(out result))
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          collection.Add(lastReadData, result);
        }
        else
          collection.Add(lastReadData, unpacker.LastReadData);
      }
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new MessagePackObjectDictionary(initialCapacity);
    }
  }
}
