// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.ImmutableStackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class ImmutableStackSerializer<T, TItem> : ImmutableCollectionSerializer<T, TItem>
    where T : IEnumerable<TItem>
  {
    private readonly MessagePackSerializer<TItem> _itemSerializer;

    public ImmutableStackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema itemsSchema)
      : base(ownerContext, itemsSchema)
    {
      this._itemSerializer = ownerContext.GetSerializer<TItem>((object) itemsSchema);
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      TItem[] objArray = new TItem[UnpackHelpers.GetItemsCount(unpacker)];
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        for (int index = objArray.Length - 1; index >= 0; --index)
        {
          if (!unpacker1.Read())
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          objArray[index] = this._itemSerializer.UnpackFrom(unpacker1);
        }
      }
      return ImmutableCollectionSerializer<T, TItem>.factory(objArray);
    }
  }
}
