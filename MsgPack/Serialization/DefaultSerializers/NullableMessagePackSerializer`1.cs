// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.NullableMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class NullableMessagePackSerializer<T> : MessagePackSerializer<T?> where T : struct
  {
    private readonly MessagePackSerializer<T> _valueSerializer;

    public NullableMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
      this._valueSerializer = ownerContext.GetSerializer<T>();
    }

    public NullableMessagePackSerializer(
      SerializationContext ownerContext,
      MessagePackSerializer<T> valueSerializer)
      : base(ownerContext)
    {
      this._valueSerializer = valueSerializer;
    }

    protected internal override void PackToCore(Packer packer, T? objectTree)
    {
      this._valueSerializer.PackToCore(packer, objectTree.Value);
    }

    protected internal override T? UnpackFromCore(Unpacker unpacker)
    {
      return new T?(this._valueSerializer.UnpackFromCore(unpacker));
    }
  }
}
