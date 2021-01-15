// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Specialized_NameValueCollectionMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_Specialized_NameValueCollectionMessagePackSerializer : MessagePackSerializer<NameValueCollection>, ICollectionInstanceFactory
  {
    public System_Collections_Specialized_NameValueCollectionMessagePackSerializer(
      SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, NameValueCollection objectTree)
    {
      if (objectTree == null)
      {
        packer.PackNull();
      }
      else
      {
        packer.PackMapHeader(objectTree.Count);
        foreach (string name in (NameObjectCollectionBase) objectTree)
        {
          if (name == null)
            throw new NotSupportedException("null key is not supported.");
          packer.PackString(name);
          string[] values = objectTree.GetValues(name);
          if (values == null)
          {
            packer.PackArrayHeader(0);
          }
          else
          {
            packer.PackArrayHeader(values.Length);
            foreach (string str in values)
              packer.PackString(str);
          }
        }
      }
    }

    protected internal override NameValueCollection UnpackFromCore(
      Unpacker unpacker)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      NameValueCollection collection = new NameValueCollection(itemsCount);
      System_Collections_Specialized_NameValueCollectionMessagePackSerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, NameValueCollection collection)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      System_Collections_Specialized_NameValueCollectionMessagePackSerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(
      Unpacker unpacker,
      NameValueCollection collection,
      int keyCount)
    {
      for (int index1 = 0; index1 < keyCount; ++index1)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        string name = unpacker.LastReadData.DeserializeAsString();
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        if (!unpacker.IsArrayHeader)
          throw new SerializationException("Invalid NameValueCollection value.");
        int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
        using (Unpacker unpacker1 = unpacker.ReadSubtree())
        {
          for (int index2 = 0; index2 < itemsCount; ++index2)
          {
            if (!unpacker1.Read())
              throw SerializationExceptions.NewUnexpectedEndOfStream();
            collection.Add(name, unpacker1.LastReadData.DeserializeAsString());
          }
        }
      }
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new NameValueCollection(initialCapacity);
    }
  }
}
