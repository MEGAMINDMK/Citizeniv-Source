// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.ContextBasedEnumSerializerEmitter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Reflection;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class ContextBasedEnumSerializerEmitter : EnumSerializerEmitter
  {
    private readonly Type _targetType;
    private readonly DynamicMethod _packUnderyingValueToMethod;
    private readonly DynamicMethod _unpackFromUnderlyingValueMethod;

    public ContextBasedEnumSerializerEmitter(SerializerSpecification specification)
    {
      this._targetType = specification.TargetType;
      this._packUnderyingValueToMethod = new DynamicMethod("PackUnderyingValue", (Type) null, new Type[3]
      {
        typeof (SerializationContext),
        typeof (Packer),
        this._targetType
      });
      this._unpackFromUnderlyingValueMethod = new DynamicMethod("UnpackFromUnderlyingValue", this._targetType, new Type[2]
      {
        typeof (SerializationContext),
        typeof (MessagePackObject)
      });
    }

    public override TracingILGenerator GetPackUnderyingValueToMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) this._packUnderyingValueToMethod);
      return new TracingILGenerator(this._packUnderyingValueToMethod, SerializerDebugging.ILTraceWriter);
    }

    public override TracingILGenerator GetUnpackFromUnderlyingValueMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) this._unpackFromUnderlyingValueMethod);
      return new TracingILGenerator(this._unpackFromUnderlyingValueMethod, SerializerDebugging.ILTraceWriter);
    }

    public override Func<SerializationContext, EnumSerializationMethod, MessagePackSerializer<T>> CreateConstructor<T>()
    {
      Action<SerializationContext, Packer, T> packUnderyingValueTo = this._packUnderyingValueToMethod.CreateDelegate(typeof (Action<SerializationContext, Packer, T>)) as Action<SerializationContext, Packer, T>;
      Func<SerializationContext, MessagePackObject, T> unpackFromUnderlyingValue = this._unpackFromUnderlyingValueMethod.CreateDelegate(typeof (Func<SerializationContext, MessagePackObject, T>)) as Func<SerializationContext, MessagePackObject, T>;
      Type targetType = typeof (CallbackEnumMessagePackSerializer<>).MakeGenericType(typeof (T));
      return (Func<SerializationContext, EnumSerializationMethod, MessagePackSerializer<T>>) ((context, method) => MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<MessagePackSerializer<T>>(targetType, (object) context, (object) method, (object) packUnderyingValueTo, (object) unpackFromUnderlyingValue));
    }
  }
}
