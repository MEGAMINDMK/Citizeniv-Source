// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.CollectionMessagePackSerializerBase`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class CollectionMessagePackSerializerBase<TCollection, TItem> : EnumerableMessagePackSerializerBase<TCollection, TItem>
    where TCollection : IEnumerable<TItem>
  {
    protected CollectionMessagePackSerializerBase(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, TCollection objectTree)
    {
      packer.PackArrayHeader(this.GetCount(objectTree));
      MessagePackSerializer<TItem> itemSerializer = this.ItemSerializer;
      using (IEnumerator<TItem> enumerator = objectTree.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          TItem current = enumerator.Current;
          itemSerializer.PackTo(packer, current);
        }
      }
    }

    protected abstract int GetCount(TCollection collection);

    protected internal override sealed TCollection UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      return this.InternalUnpackFromCore(unpacker);
    }

    internal virtual TCollection InternalUnpackFromCore(Unpacker unpacker)
    {
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      TCollection instance = this.CreateInstance(itemsCount);
      this.UnpackToCore(unpacker, instance, itemsCount);
      return instance;
    }
  }
}
