﻿// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.NativeFileTimeMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.InteropServices.ComTypes;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class NativeFileTimeMessagePackSerializer : MessagePackSerializer<FILETIME>
  {
    public NativeFileTimeMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, FILETIME objectTree)
    {
      packer.Pack(objectTree.ToDateTime().ToBinary());
    }

    protected internal override FILETIME UnpackFromCore(Unpacker unpacker)
    {
      return DateTime.FromBinary(unpacker.LastReadData.AsInt64()).ToWin32FileTimeUtc();
    }
  }
}
