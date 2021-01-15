// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.CallbackMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class CallbackMessagePackSerializer<T> : MessagePackSerializer<T>
  {
    private readonly Action<SerializationContext, Packer, T> _packToCore;
    private readonly Func<SerializationContext, Unpacker, T> _unpackFromCore;
    private readonly Action<SerializationContext, Unpacker, T> _unpackToCore;

    public CallbackMessagePackSerializer(
      SerializationContext ownerContext,
      Action<SerializationContext, Packer, T> packToCore,
      Func<SerializationContext, Unpacker, T> unpackFromCore,
      Action<SerializationContext, Unpacker, T> unpackToCore)
      : base(ownerContext)
    {
      this._packToCore = packToCore;
      this._unpackFromCore = unpackFromCore;
      this._unpackToCore = unpackToCore;
    }

    protected internal override void PackToCore(Packer packer, T objectTree)
    {
      this._packToCore(this.OwnerContext, packer, objectTree);
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      return this._unpackFromCore(this.OwnerContext, unpacker);
    }

    protected internal override void UnpackToCore(Unpacker unpacker, T collection)
    {
      if (this._unpackToCore != null)
        this._unpackToCore(this.OwnerContext, unpacker, collection);
      else
        base.UnpackToCore(unpacker, collection);
    }
  }
}
