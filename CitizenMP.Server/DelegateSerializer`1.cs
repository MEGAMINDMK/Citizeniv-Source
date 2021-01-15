// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.DelegateSerializer`1
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Resources;
using MsgPack;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CitizenMP.Server
{
  internal class DelegateSerializer<T> : MessagePackSerializer<T>
  {
    public DelegateSerializer()
      : base(SerializationContext.Default)
    {
    }

    protected override void PackToCore(Packer packer, T objectTree)
    {
      int num = InternalCallRefHandler.Get().AddCallback((object) objectTree as Delegate);
      byte[] bytes = Encoding.UTF8.GetBytes("__internal");
      byte[] body = new byte[8 + bytes.Length];
      Array.Copy((Array) ((IEnumerable<byte>) BitConverter.GetBytes(num)).Reverse<byte>().ToArray<byte>(), 0, (Array) body, 0, 4);
      Array.Copy((Array) ((IEnumerable<byte>) BitConverter.GetBytes(0)).Reverse<byte>().ToArray<byte>(), 0, (Array) body, 4, 4);
      Array.Copy((Array) bytes, 0, (Array) body, 8, bytes.Length);
      packer.PackExtendedTypeValue((byte) 1, body);
    }

    protected override T UnpackFromCore(Unpacker unpacker)
    {
      throw new NotImplementedException();
    }
  }
}
