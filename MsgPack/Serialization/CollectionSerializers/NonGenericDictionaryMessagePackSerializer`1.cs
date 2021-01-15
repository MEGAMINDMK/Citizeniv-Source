// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.NonGenericDictionaryMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class NonGenericDictionaryMessagePackSerializer<TDictionary> : MessagePackSerializer<TDictionary>, ICollectionInstanceFactory
    where TDictionary : IDictionary
  {
    private readonly IMessagePackSingleObjectSerializer _keySerializer;
    private readonly IMessagePackSingleObjectSerializer _valueSerializer;

    protected NonGenericDictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext)
    {
      PolymorphismSchema polymorphismSchema = schema ?? PolymorphismSchema.Default;
      this._keySerializer = ownerContext.GetSerializer(typeof (object), (object) polymorphismSchema.KeySchema);
      this._valueSerializer = ownerContext.GetSerializer(typeof (object), (object) polymorphismSchema.ItemSchema);
    }

    protected internal override sealed void PackToCore(Packer packer, TDictionary objectTree)
    {
      packer.PackMapHeader(objectTree.Count);
      IDictionaryEnumerator enumerator = objectTree.GetEnumerator();
      try
      {
        while (enumerator.MoveNext())
        {
          DictionaryEntry current = (DictionaryEntry) enumerator.Current;
          this._keySerializer.PackTo(packer, current.Key);
          this._valueSerializer.PackTo(packer, current.Value);
        }
      }
      finally
      {
        if (enumerator is IDisposable disposable)
          disposable.Dispose();
      }
    }

    protected internal override sealed TDictionary UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      return this.InternalUnpackFromCore(unpacker);
    }

    internal virtual TDictionary InternalUnpackFromCore(Unpacker unpacker)
    {
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      TDictionary instance = this.CreateInstance(itemsCount);
      this.UnpackToCore(unpacker, instance, itemsCount);
      return instance;
    }

    protected abstract TDictionary CreateInstance(int initialCapacity);

    object ICollectionInstanceFactory.CreateInstance(int initialCapacity)
    {
      return (object) this.CreateInstance(initialCapacity);
    }

    protected internal override sealed void UnpackToCore(Unpacker unpacker, TDictionary collection)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      this.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private void UnpackToCore(Unpacker unpacker, TDictionary collection, int itemsCount)
    {
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        object key;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          key = this._keySerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            key = this._keySerializer.UnpackFrom(unpacker1);
        }
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        object obj;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          obj = this._valueSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            obj = this._valueSerializer.UnpackFrom(unpacker1);
        }
        collection.Add(key, obj);
      }
    }
  }
}
