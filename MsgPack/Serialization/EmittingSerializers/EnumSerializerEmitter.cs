// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.EnumSerializerEmitter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal abstract class EnumSerializerEmitter : IDisposable
  {
    protected static readonly Type[] UnpackFromUnderlyingValueParameterTypes = new Type[1]
    {
      typeof (MessagePackObject)
    };

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public abstract TracingILGenerator GetPackUnderyingValueToMethodILGenerator();

    public abstract TracingILGenerator GetUnpackFromUnderlyingValueMethodILGenerator();

    public MessagePackSerializer<T> CreateInstance<T>(
      SerializationContext context,
      EnumSerializationMethod serializationMethod)
    {
      return this.CreateConstructor<T>()(context, serializationMethod);
    }

    public abstract Func<SerializationContext, EnumSerializationMethod, MessagePackSerializer<T>> CreateConstructor<T>();
  }
}
