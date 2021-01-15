// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.NullableInt64ArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class NullableInt64ArraySerializer : MessagePackSerializer<long?[]>
  {
    public NullableInt64ArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, long?[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (long? nullable in objectTree)
        packer.Pack(nullable);
    }

    protected internal override long?[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      long?[] collection = new long?[itemsCount];
      NullableInt64ArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, long?[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      NullableInt64ArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, long?[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        long? result;
        if (!unpacker.ReadNullableInt64(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
