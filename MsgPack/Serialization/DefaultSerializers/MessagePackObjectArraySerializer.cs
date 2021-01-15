// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.MessagePackObjectArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class MessagePackObjectArraySerializer : MessagePackSerializer<MessagePackObject[]>
  {
    public MessagePackObjectArraySerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, MessagePackObject[] objectTree)
    {
      packer.PackArrayHeader(objectTree.Length);
      foreach (MessagePackObject messagePackObject in objectTree)
        messagePackObject.PackToMessage(packer, (PackingOptions) null);
    }

    protected internal override MessagePackObject[] UnpackFromCore(
      Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      MessagePackObject[] collection = new MessagePackObject[itemsCount];
      MessagePackObjectArraySerializer.UnpackToCore(unpacker, collection, itemsCount);
      return collection;
    }

    protected internal override void UnpackToCore(Unpacker unpacker, MessagePackObject[] collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      MessagePackObjectArraySerializer.UnpackToCore(unpacker, collection, UnpackHelpers.GetItemsCount(unpacker));
    }

    private static void UnpackToCore(Unpacker unpacker, MessagePackObject[] collection, int count)
    {
      for (int index = 0; index < count; ++index)
      {
        MessagePackObject result;
        if (!unpacker.ReadObject(out result))
          throw SerializationExceptions.NewMissingItem(index);
        collection[index] = result;
      }
    }
  }
}
