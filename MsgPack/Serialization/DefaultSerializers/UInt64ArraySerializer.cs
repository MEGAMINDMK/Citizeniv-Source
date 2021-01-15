// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.UInt64ArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class UInt64ArraySerializer : MessagePackSerializer<ulong[]>
  {
    public UInt64ArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, ulong[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (ulong num in objectTree)
        packer.Pack(num);
    }

    protected internal override ulong[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      ulong[] collection = new ulong[itemsCount];
      UInt64ArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, ulong[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      UInt64ArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, ulong[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        ulong result;
        if (!unpacker.ReadUInt64(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
