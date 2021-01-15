// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.FieldBasedEnumSerializerEmitter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Reflection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class FieldBasedEnumSerializerEmitter : EnumSerializerEmitter
  {
    private static readonly Type[] ContextConstructorParameterTypes = new Type[1]
    {
      typeof (SerializationContext)
    };
    private static readonly Type[] ContextAndEnumSerializationMethodConstructorParameterTypes = new Type[2]
    {
      typeof (SerializationContext),
      typeof (EnumSerializationMethod)
    };
    private readonly ConstructorBuilder _contextConstructorBuilder;
    private readonly ConstructorBuilder _contextAndEnumSerializationMethodConstructorBuilder;
    private readonly EnumSerializationMethod _defaultEnumSerializationMethod;
    private readonly TypeBuilder _typeBuilder;
    private readonly MethodBuilder _packUnderlyingValueToMethodBuilder;
    private readonly MethodBuilder _unpackFromUnderlyingValueMethodBuilder;
    private readonly bool _isDebuggable;

    public FieldBasedEnumSerializerEmitter(
      SerializationContext context,
      ModuleBuilder host,
      SerializerSpecification specification,
      bool isDebuggable)
    {
      Tracer.Emit.TraceEvent(TraceEventType.Verbose, 102, "Create {0}", (object) specification.SerializerTypeFullName);
      this._typeBuilder = host.DefineType(specification.SerializerTypeFullName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.UnicodeClass | TypeAttributes.BeforeFieldInit, typeof (EnumMessagePackSerializer<>).MakeGenericType(specification.TargetType));
      this._contextConstructorBuilder = this._typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, FieldBasedEnumSerializerEmitter.ContextConstructorParameterTypes);
      this._defaultEnumSerializationMethod = context.EnumSerializationMethod;
      this._contextAndEnumSerializationMethodConstructorBuilder = this._typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, FieldBasedEnumSerializerEmitter.ContextAndEnumSerializationMethodConstructorParameterTypes);
      this._packUnderlyingValueToMethodBuilder = this._typeBuilder.DefineMethod("PackUnderlyingValueTo", MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof (void), new Type[2]
      {
        typeof (Packer),
        specification.TargetType
      });
      this._unpackFromUnderlyingValueMethodBuilder = this._typeBuilder.DefineMethod("UnpackFromUnderlyingValue", MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, specification.TargetType, EnumSerializerEmitter.UnpackFromUnderlyingValueParameterTypes);
      Type baseType = this._typeBuilder.BaseType;
      this._typeBuilder.DefineMethodOverride((MethodInfo) this._packUnderlyingValueToMethodBuilder, baseType.GetMethod(this._packUnderlyingValueToMethodBuilder.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
      this._typeBuilder.DefineMethodOverride((MethodInfo) this._unpackFromUnderlyingValueMethodBuilder, baseType.GetMethod(this._unpackFromUnderlyingValueMethodBuilder.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
      this._isDebuggable = isDebuggable;
      if (!isDebuggable || !SerializerDebugging.DumpEnabled)
        return;
      SerializerDebugging.PrepareDump(host.Assembly as AssemblyBuilder);
    }

    public override TracingILGenerator GetPackUnderyingValueToMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) this._packUnderlyingValueToMethodBuilder);
      return new TracingILGenerator(this._packUnderlyingValueToMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override TracingILGenerator GetUnpackFromUnderlyingValueMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) this._unpackFromUnderlyingValueMethodBuilder);
      return new TracingILGenerator(this._unpackFromUnderlyingValueMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override Func<SerializationContext, EnumSerializationMethod, MessagePackSerializer<T>> CreateConstructor<T>()
    {
      if (!this._typeBuilder.IsCreated())
      {
        TracingILGenerator tracingIlGenerator1 = new TracingILGenerator(this._contextConstructorBuilder, TextWriter.Null, this._isDebuggable);
        tracingIlGenerator1.EmitLdarg_0();
        tracingIlGenerator1.EmitLdarg_1();
        tracingIlGenerator1.EmitAnyLdc_I4((int) this._defaultEnumSerializationMethod);
        tracingIlGenerator1.EmitCallConstructor((ConstructorInfo) this._contextAndEnumSerializationMethodConstructorBuilder);
        tracingIlGenerator1.EmitRet();
        TracingILGenerator tracingIlGenerator2 = new TracingILGenerator(this._contextAndEnumSerializationMethodConstructorBuilder, TextWriter.Null, this._isDebuggable);
        tracingIlGenerator2.EmitLdarg_0();
        tracingIlGenerator2.EmitLdarg_1();
        tracingIlGenerator2.EmitLdarg_2();
        tracingIlGenerator2.EmitCallConstructor(this._typeBuilder.BaseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, FieldBasedEnumSerializerEmitter.ContextAndEnumSerializationMethodConstructorParameterTypes, (ParameterModifier[]) null));
        tracingIlGenerator2.EmitRet();
      }
      ParameterExpression parameterExpression1;
      ParameterExpression parameterExpression2;
      return Expression.Lambda<Func<SerializationContext, EnumSerializationMethod, MessagePackSerializer<T>>>((Expression) Expression.New(this._typeBuilder.CreateType().GetConstructor(FieldBasedEnumSerializerEmitter.ContextAndEnumSerializationMethodConstructorParameterTypes), (Expression) parameterExpression1, (Expression) parameterExpression2), parameterExpression1, parameterExpression2).Compile();
    }
  }
}
