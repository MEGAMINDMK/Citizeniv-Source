// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.ArraySerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class ArraySerializer<T> : MessagePackSerializer<T[]>
  {
    private readonly MessagePackSerializer<T> _itemSerializer;

    public ArraySerializer(SerializationContext ownerContext, PolymorphismSchema itemsSchema)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<T>((object) itemsSchema);
    }

    protected internal override void PackToCore(Packer packer, T[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (T objectTree1 in objectTree)
        this._itemSerializer.PackTo(packer, objectTree1);
    }

    protected internal override T[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      T[] collection = new T[itemsCount];
      this.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, T[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      this.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private void UnpackToCore(Unpacker unpacker, T[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        T obj;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          obj = this._itemSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            obj = this._itemSerializer.UnpackFrom(unpacker1);
        }
        collection[index] = obj;
      }
    }
  }
}
