// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_ObjectMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_ObjectMessagePackSerializer : MessagePackSerializer<object>
  {
    public System_ObjectMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, object value)
    {
      if (value.GetType() == typeof (object))
        throw new SerializationException("System.Object cannot be serialized.");
      packer.PackObject(value, this.OwnerContext);
    }

    protected internal override object UnpackFromCore(Unpacker unpacker)
    {
      if (unpacker.IsArrayHeader)
      {
        MessagePackObject[] messagePackObjectArray = new MessagePackObject[UnpackHelpers.GetItemsCount(unpacker)];
        for (int index = 0; index < messagePackObjectArray.Length; ++index)
        {
          if (!unpacker.ReadObject(out messagePackObjectArray[index]))
            throw SerializationExceptions.NewUnexpectedEndOfStream();
        }
        return (object) new MessagePackObject((IList<MessagePackObject>) messagePackObjectArray);
      }
      if (unpacker.IsMapHeader)
      {
        int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
        MessagePackObjectDictionary objectDictionary = new MessagePackObjectDictionary(itemsCount);
        for (int index = 0; index < itemsCount; ++index)
        {
          MessagePackObject result1;
          if (!unpacker.ReadObject(out result1))
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          MessagePackObject result2;
          if (!unpacker.ReadObject(out result2))
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          objectDictionary.Add(result1, result2);
        }
        return (object) new MessagePackObject(objectDictionary);
      }
      MessagePackObject lastReadData = unpacker.LastReadData;
      return (object) (lastReadData.IsNil ? MessagePackObject.Nil : lastReadData);
    }
  }
}
