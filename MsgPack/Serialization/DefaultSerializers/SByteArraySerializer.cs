// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.SByteArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class SByteArraySerializer : MessagePackSerializer<sbyte[]>
  {
    public SByteArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, sbyte[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (sbyte num in objectTree)
        packer.Pack(num);
    }

    protected internal override sbyte[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      sbyte[] collection = new sbyte[itemsCount];
      SByteArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, sbyte[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      SByteArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, sbyte[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        sbyte result;
        if (!unpacker.ReadSByte(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
