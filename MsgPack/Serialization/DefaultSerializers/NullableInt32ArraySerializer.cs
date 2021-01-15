﻿// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.NullableInt32ArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class NullableInt32ArraySerializer : MessagePackSerializer<int?[]>
  {
    public NullableInt32ArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, int?[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (int? nullable in objectTree)
        packer.Pack(nullable);
    }

    protected internal override int?[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      int?[] collection = new int?[itemsCount];
      NullableInt32ArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, int?[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      NullableInt32ArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, int?[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        int? result;
        if (!unpacker.ReadNullableInt32(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
