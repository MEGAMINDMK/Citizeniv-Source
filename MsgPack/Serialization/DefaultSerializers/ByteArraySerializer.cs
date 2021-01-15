// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.ByteArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class ByteArraySerializer : MessagePackSerializer<byte[]>
  {
    public ByteArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, byte[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (byte num in objectTree)
        packer.Pack(num);
    }

    protected internal override byte[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      byte[] collection = new byte[itemsCount];
      ByteArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, byte[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      ByteArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, byte[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        byte result;
        if (!unpacker.ReadByte(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
