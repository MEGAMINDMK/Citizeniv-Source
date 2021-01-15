// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.NullableInt16ArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class NullableInt16ArraySerializer : MessagePackSerializer<short?[]>
  {
    public NullableInt16ArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, short?[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (short? nullable in objectTree)
        packer.Pack(nullable);
    }

    protected internal override short?[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      short?[] collection = new short?[itemsCount];
      NullableInt16ArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, short?[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      NullableInt16ArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, short?[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        short? result;
        if (!unpacker.ReadNullableInt16(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
