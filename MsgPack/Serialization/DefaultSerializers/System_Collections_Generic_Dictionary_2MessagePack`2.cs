// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_Generic_Dictionary_2MessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class System_Collections_Generic_Dictionary_2MessagePackSerializer<TKey, TValue> : MessagePackSerializer<Dictionary<TKey, TValue>>, ICollectionInstanceFactory
  {
    private readonly MessagePackSerializer<TKey> _keySerializer;
    private readonly MessagePackSerializer<TValue> _valueSerializer;

    public System_Collections_Generic_Dictionary_2MessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema keysSchema,
      PolymorphismSchema valuesSchema)
      : base(ownerContext)
    {
      this._keySerializer = ownerContext.GetSerializer<TKey>((object) keysSchema);
      this._valueSerializer = ownerContext.GetSerializer<TValue>((object) valuesSchema);
    }

    protected internal override void PackToCore(Packer packer, Dictionary<TKey, TValue> objectTree)
    {
      PackerUnpackerExtensions.PackDictionaryCore<TKey, TValue>(packer, (IDictionary<TKey, TValue>) objectTree, this._keySerializer, this._valueSerializer);
    }

    protected internal override Dictionary<TKey, TValue> UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      Dictionary<TKey, TValue> collection = new Dictionary<TKey, TValue>(itemsCount);
      this.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(
      Unpacker unpacker,
      Dictionary<TKey, TValue> collection)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      this.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private void UnpackToCore(Unpacker unpacker, Dictionary<TKey, TValue> collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        TKey key;
        if (unpacker.IsCollectionHeader)
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            key = this._keySerializer.UnpackFromCore(unpacker1);
        }
        else
          key = this._keySerializer.UnpackFromCore(unpacker);
        if (!unpacker.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        if (unpacker.IsCollectionHeader)
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            collection.Add(key, this._valueSerializer.UnpackFromCore(unpacker1));
        }
        else
          collection.Add(key, this._valueSerializer.UnpackFromCore(unpacker));
      }
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new Dictionary<TKey, TValue>(initialCapacity);
    }
  }
}
