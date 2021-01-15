// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.UInt32ArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class UInt32ArraySerializer : MessagePackSerializer<uint[]>
  {
    public UInt32ArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, uint[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (uint num in objectTree)
        packer.Pack(num);
    }

    protected internal override uint[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      uint[] collection = new uint[itemsCount];
      UInt32ArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, uint[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      UInt32ArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, uint[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        uint result;
        if (!unpacker.ReadUInt32(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
