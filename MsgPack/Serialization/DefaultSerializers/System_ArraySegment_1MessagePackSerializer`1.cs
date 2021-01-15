// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_ArraySegment_1MessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class System_ArraySegment_1MessagePackSerializer<T> : MessagePackSerializer<ArraySegment<T>>
  {
    private static readonly Action<Packer, ArraySegment<T>, MessagePackSerializer<T>> _packing = System_ArraySegment_1MessagePackSerializer<T>.InitializePacking();
    private static readonly Func<Unpacker, MessagePackSerializer<T>, ArraySegment<T>> _unpacking = System_ArraySegment_1MessagePackSerializer<T>.InitializeUnpacking();
    private readonly MessagePackSerializer<T> _itemSerializer;

    private static Action<Packer, ArraySegment<T>, MessagePackSerializer<T>> InitializePacking()
    {
      if (typeof (T) == typeof (byte))
        return new Action<Packer, ArraySegment<byte>, MessagePackSerializer<byte>>(ArraySegmentMessageSerializer.PackByteArraySegmentTo) as Action<Packer, ArraySegment<T>, MessagePackSerializer<T>>;
      return typeof (T) == typeof (char) ? new Action<Packer, ArraySegment<char>, MessagePackSerializer<char>>(ArraySegmentMessageSerializer.PackCharArraySegmentTo) as Action<Packer, ArraySegment<T>, MessagePackSerializer<T>> : new Action<Packer, ArraySegment<T>, MessagePackSerializer<T>>(ArraySegmentMessageSerializer.PackGenericArraySegmentTo<T>);
    }

    private static Func<Unpacker, MessagePackSerializer<T>, ArraySegment<T>> InitializeUnpacking()
    {
      if (typeof (T) == typeof (byte))
        return new Func<Unpacker, MessagePackSerializer<byte>, ArraySegment<byte>>(ArraySegmentMessageSerializer.UnpackByteArraySegmentFrom) as Func<Unpacker, MessagePackSerializer<T>, ArraySegment<T>>;
      return typeof (T) == typeof (char) ? new Func<Unpacker, MessagePackSerializer<char>, ArraySegment<char>>(ArraySegmentMessageSerializer.UnpackCharArraySegmentFrom) as Func<Unpacker, MessagePackSerializer<T>, ArraySegment<T>> : new Func<Unpacker, MessagePackSerializer<T>, ArraySegment<T>>(ArraySegmentMessageSerializer.UnpackGenericArraySegmentFrom<T>);
    }

    public System_ArraySegment_1MessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
      this._itemSerializer = ownerContext.GetSerializer<T>();
    }

    protected internal override sealed void PackToCore(Packer packer, ArraySegment<T> objectTree)
    {
      System_ArraySegment_1MessagePackSerializer<T>._packing(packer, objectTree, this._itemSerializer);
    }

    protected internal override sealed ArraySegment<T> UnpackFromCore(Unpacker unpacker)
    {
      return System_ArraySegment_1MessagePackSerializer<T>._unpacking(unpacker, this._itemSerializer);
    }
  }
}
