// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.DateTimeOffsetMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class DateTimeOffsetMessagePackSerializer : MessagePackSerializer<DateTimeOffset>
  {
    private readonly DateTimeConversionMethod _conversion;

    public DateTimeOffsetMessagePackSerializer(
      SerializationContext ownerContext,
      DateTimeConversionMethod conversion)
      : base(ownerContext)
    {
      this._conversion = conversion;
    }

    protected internal override void PackToCore(Packer packer, DateTimeOffset objectTree)
    {
      if (this._conversion == DateTimeConversionMethod.Native)
      {
        packer.PackArrayHeader(2);
        packer.Pack(objectTree.DateTime.ToBinary());
        packer.Pack((short) (objectTree.Offset.Hours * 60 + objectTree.Offset.Minutes));
      }
      else
        packer.Pack(MessagePackConvert.FromDateTimeOffset(objectTree));
    }

    protected internal override DateTimeOffset UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        return MessagePackConvert.ToDateTimeOffset(unpacker.LastReadData.AsInt64());
      if (UnpackHelpers.GetItemsCount(unpacker) != 2)
        throw new SerializationException("Invalid DateTimeOffset serialization.");
      long result1;
      if (!unpacker.ReadInt64(out result1))
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      short result2;
      if (!unpacker.ReadInt16(out result2))
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      return new DateTimeOffset(DateTime.FromBinary(result1), TimeSpan.FromMinutes((double) result2));
    }
  }
}
