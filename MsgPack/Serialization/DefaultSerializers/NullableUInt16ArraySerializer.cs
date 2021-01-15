// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.NullableUInt16ArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class NullableUInt16ArraySerializer : MessagePackSerializer<ushort?[]>
  {
    public NullableUInt16ArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, ushort?[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (ushort? nullable in objectTree)
        packer.Pack(nullable);
    }

    protected internal override ushort?[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      ushort?[] collection = new ushort?[itemsCount];
      NullableUInt16ArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, ushort?[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      NullableUInt16ArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, ushort?[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        ushort? result;
        if (!unpacker.ReadNullableUInt16(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
