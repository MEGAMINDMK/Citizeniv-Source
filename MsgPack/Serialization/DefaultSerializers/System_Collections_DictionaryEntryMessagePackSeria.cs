// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_DictionaryEntryMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_DictionaryEntryMessagePackSerializer : MessagePackSerializer<DictionaryEntry>
  {
    public System_Collections_DictionaryEntryMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, DictionaryEntry objectTree)
    {
      packer.PackArrayHeader(2);
      System_Collections_DictionaryEntryMessagePackSerializer.EnsureMessagePackObject(objectTree.Key).PackToMessage(packer, (PackingOptions) null);
      System_Collections_DictionaryEntryMessagePackSerializer.EnsureMessagePackObject(objectTree.Value).PackToMessage(packer, (PackingOptions) null);
    }

    private static MessagePackObject EnsureMessagePackObject(object obj)
    {
      if (obj == null)
        return MessagePackObject.Nil;
      if (!(obj is MessagePackObject messagePackObject))
        throw new NotSupportedException("Only MessagePackObject Key/Value is supported.");
      return messagePackObject;
    }

    protected internal override DictionaryEntry UnpackFromCore(Unpacker unpacker)
    {
      if (unpacker.IsArrayHeader)
      {
        MessagePackObject result1;
        if (!unpacker.ReadObject(out result1))
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        MessagePackObject result2;
        if (!unpacker.ReadObject(out result2))
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        return new DictionaryEntry((object) result1, (object) result2);
      }
      MessagePackObject result3 = new MessagePackObject();
      MessagePackObject result4 = new MessagePackObject();
      bool flag1 = false;
      bool flag2 = false;
      string result5;
      while ((!flag1 || !flag2) && unpacker.ReadString(out result5))
      {
        switch (result5)
        {
          case "Key":
            if (!unpacker.ReadObject(out result3))
              throw SerializationExceptions.NewUnexpectedEndOfStream();
            flag1 = true;
            continue;
          case "Value":
            if (!unpacker.ReadObject(out result4))
              throw SerializationExceptions.NewUnexpectedEndOfStream();
            flag2 = true;
            continue;
          default:
            continue;
        }
      }
      if (!flag1)
        throw SerializationExceptions.NewMissingProperty("Key");
      if (!flag2)
        throw SerializationExceptions.NewMissingProperty("Value");
      return new DictionaryEntry((object) result3, (object) result4);
    }
  }
}
