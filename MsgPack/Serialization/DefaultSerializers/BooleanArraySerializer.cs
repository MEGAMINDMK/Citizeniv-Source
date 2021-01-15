// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.BooleanArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class BooleanArraySerializer : MessagePackSerializer<bool[]>
  {
    public BooleanArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, bool[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (bool flag in objectTree)
        packer.Pack(flag);
    }

    protected internal override bool[] UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      bool[] collection = new bool[itemsCount];
      BooleanArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, bool[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      BooleanArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, bool[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        bool result;
        if (!unpacker.ReadBoolean(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
