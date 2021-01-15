// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.NonGenericListMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class NonGenericListMessagePackSerializer<TList> : NonGenericCollectionMessagePackSerializer<TList>
    where TList : IList
  {
    protected NonGenericListMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
    }

    protected internal override sealed TList UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      return this.InternalUnpackFromCore(unpacker);
    }

    internal virtual TList InternalUnpackFromCore(Unpacker unpacker)
    {
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      TList instance = this.CreateInstance(itemsCount);
      this.UnpackToCore(unpacker, instance, itemsCount);
      return instance;
    }

    protected override void AddItem(TList collection, object item)
    {
      collection.Add(item);
    }
  }
}
