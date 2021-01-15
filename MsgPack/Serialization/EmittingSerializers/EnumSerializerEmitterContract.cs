// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.EnumSerializerEmitterContract
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class EnumSerializerEmitterContract : EnumSerializerEmitter
  {
    public override TracingILGenerator GetPackUnderyingValueToMethodILGenerator()
    {
      return (TracingILGenerator) null;
    }

    public override TracingILGenerator GetUnpackFromUnderlyingValueMethodILGenerator()
    {
      return (TracingILGenerator) null;
    }

    public override Func<SerializationContext, EnumSerializationMethod, MessagePackSerializer<T>> CreateConstructor<T>()
    {
      return (Func<SerializationContext, EnumSerializationMethod, MessagePackSerializer<T>>) null;
    }
  }
}
