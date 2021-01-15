// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.FieldBasedSerializerEmitter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Metadata;
using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class FieldBasedSerializerEmitter : SerializerEmitter
  {
    private static readonly Type[] ConstructorParameterTypes = new Type[1]
    {
      typeof (SerializationContext)
    };
    private static readonly Type[] CollectionConstructorParameterTypes = new Type[2]
    {
      typeof (SerializationContext),
      typeof (PolymorphismSchema)
    };
    private readonly Dictionary<SerializerFieldKey, FieldBasedSerializerEmitter.SerializerFieldInfo> _serializers;
    private readonly Dictionary<RuntimeFieldHandle, FieldBasedSerializerEmitter.CachedFieldInfo> _cachedFieldInfos;
    private readonly Dictionary<RuntimeMethodHandle, FieldBasedSerializerEmitter.CachedMethodBase> _cachedMethodBases;
    private readonly ConstructorBuilder _defaultConstructorBuilder;
    private readonly ConstructorBuilder _contextConstructorBuilder;
    private readonly TypeBuilder _typeBuilder;
    private MethodBuilder _packMethodBuilder;
    private MethodBuilder _unpackFromMethodBuilder;
    private MethodBuilder _unpackToMethodBuilder;
    private MethodBuilder _addItemMethodBuilder;
    private MethodBuilder _createInstanceMethodBuilder;
    private MethodBuilder _restoreSchemaMethodBuilder;
    private readonly CollectionTraits _traits;
    private readonly bool _isDebuggable;

    public FieldBasedSerializerEmitter(
      ModuleBuilder host,
      SerializerSpecification specification,
      Type baseClass,
      bool isDebuggable)
    {
      Tracer.Emit.TraceEvent(TraceEventType.Verbose, 102, "Create {0}", (object) specification.SerializerTypeFullName);
      this._typeBuilder = host.DefineType(specification.SerializerTypeFullName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.UnicodeClass | TypeAttributes.BeforeFieldInit, baseClass);
      this._defaultConstructorBuilder = this._typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
      this._contextConstructorBuilder = this._typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, FieldBasedSerializerEmitter.ConstructorParameterTypes);
      this._traits = specification.TargetCollectionTraits;
      Type baseType = this._typeBuilder.BaseType;
      this._serializers = new Dictionary<SerializerFieldKey, FieldBasedSerializerEmitter.SerializerFieldInfo>();
      this._cachedFieldInfos = new Dictionary<RuntimeFieldHandle, FieldBasedSerializerEmitter.CachedFieldInfo>();
      this._cachedMethodBases = new Dictionary<RuntimeMethodHandle, FieldBasedSerializerEmitter.CachedMethodBase>();
      this._isDebuggable = isDebuggable;
      if (!isDebuggable || !SerializerDebugging.DumpEnabled)
        return;
      SerializerDebugging.PrepareDump(host.Assembly as AssemblyBuilder);
    }

    public override TracingILGenerator GetPackToMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) "PackToCore");
      if ((MethodInfo) this._packMethodBuilder == (MethodInfo) null)
      {
        MethodInfo method = this._typeBuilder.BaseType.GetMethod("PackToCore", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        this._packMethodBuilder = this._typeBuilder.DefineMethod(method.Name, (method.Attributes | MethodAttributes.Final) & ~MethodAttributes.Abstract, method.CallingConvention, method.ReturnType, ((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>());
        this._typeBuilder.DefineMethodOverride((MethodInfo) this._packMethodBuilder, method);
      }
      return new TracingILGenerator(this._packMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override TracingILGenerator GetUnpackFromMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) "UnpackFromCore");
      if ((MethodInfo) this._unpackFromMethodBuilder == (MethodInfo) null)
      {
        MethodInfo method = this._typeBuilder.BaseType.GetMethod("UnpackFromCore", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        this._unpackFromMethodBuilder = this._typeBuilder.DefineMethod(method.Name, (method.Attributes | MethodAttributes.Final) & ~MethodAttributes.Abstract, method.CallingConvention, method.ReturnType, ((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>());
        this._typeBuilder.DefineMethodOverride((MethodInfo) this._unpackFromMethodBuilder, method);
      }
      return new TracingILGenerator(this._unpackFromMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override TracingILGenerator GetCreateInstanceMethodILGenerator(
      MethodInfo declaration)
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) "CreateInstance");
      if ((MethodInfo) this._createInstanceMethodBuilder == (MethodInfo) null)
      {
        MethodInfo method = this._typeBuilder.BaseType.GetMethod("CreateInstance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        this._createInstanceMethodBuilder = this._typeBuilder.DefineMethod(method.Name, (method.Attributes | MethodAttributes.Final) & ~MethodAttributes.Abstract, method.CallingConvention, method.ReturnType, ((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>());
        this._typeBuilder.DefineMethodOverride((MethodInfo) this._createInstanceMethodBuilder, method);
      }
      return new TracingILGenerator(this._createInstanceMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override TracingILGenerator GetAddItemMethodILGenerator(
      MethodInfo declaration)
    {
      if ((MethodInfo) this._addItemMethodBuilder == (MethodInfo) null)
      {
        MethodInfo method = this._typeBuilder.BaseType.GetMethod("AddItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        this._addItemMethodBuilder = this._typeBuilder.DefineMethod(method.Name, (method.Attributes | MethodAttributes.Final) & ~MethodAttributes.Abstract, method.CallingConvention, method.ReturnType, ((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>());
        this._typeBuilder.DefineMethodOverride((MethodInfo) this._addItemMethodBuilder, method);
      }
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) this._addItemMethodBuilder);
      return new TracingILGenerator(this._addItemMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override TracingILGenerator GetRestoreSchemaMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) "RestoreSchema");
      if ((MethodInfo) this._restoreSchemaMethodBuilder == (MethodInfo) null)
        this._restoreSchemaMethodBuilder = this._typeBuilder.DefineMethod("RestoreSchema", MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig, CallingConventions.Standard, typeof (PolymorphismSchema), ReflectionAbstractions.EmptyTypes);
      return new TracingILGenerator(this._restoreSchemaMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override TracingILGenerator GetUnpackToMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}->{1}::{2}", (object) MethodBase.GetCurrentMethod(), (object) this._typeBuilder.Name, (object) "UnpackToCore");
      if ((MethodInfo) this._unpackToMethodBuilder == (MethodInfo) null)
      {
        MethodInfo method = this._typeBuilder.BaseType.GetMethod("UnpackToCore", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        this._unpackToMethodBuilder = this._typeBuilder.DefineMethod(method.Name, (method.Attributes | MethodAttributes.Final) & ~MethodAttributes.Abstract, method.CallingConvention, method.ReturnType, ((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>());
        this._typeBuilder.DefineMethodOverride((MethodInfo) this._unpackToMethodBuilder, method);
      }
      return new TracingILGenerator(this._unpackToMethodBuilder, SerializerDebugging.ILTraceWriter, this._isDebuggable);
    }

    public override Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>> CreateConstructor<T>()
    {
      if (!this._typeBuilder.IsCreated())
      {
        ILGenerator ilGenerator = this._defaultConstructorBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldnull);
        ilGenerator.Emit(OpCodes.Call, (ConstructorInfo) this._contextConstructorBuilder);
        ilGenerator.Emit(OpCodes.Ret);
        TracingILGenerator tracingIlGenerator = new TracingILGenerator(this._contextConstructorBuilder, TextWriter.Null, this._isDebuggable);
        tracingIlGenerator.EmitLdarg_0();
        tracingIlGenerator.EmitLdarg_1();
        if (this._traits.CollectionType == CollectionKind.NotCollection)
        {
          tracingIlGenerator.EmitCallConstructor(this._typeBuilder.BaseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, FieldBasedSerializerEmitter.ConstructorParameterTypes, (ParameterModifier[]) null));
        }
        else
        {
          tracingIlGenerator.EmitCall((MethodInfo) this._restoreSchemaMethodBuilder);
          tracingIlGenerator.EmitCallConstructor(this._typeBuilder.BaseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, FieldBasedSerializerEmitter.CollectionConstructorParameterTypes, (ParameterModifier[]) null));
        }
        foreach (KeyValuePair<SerializerFieldKey, FieldBasedSerializerEmitter.SerializerFieldInfo> serializer in this._serializers)
        {
          Type typeFromHandle = Type.GetTypeFromHandle(serializer.Key.TypeHandle);
          MethodInfo target = _SerializationContext.GetSerializer1_Parameter_Method.MakeGenericMethod(typeFromHandle);
          tracingIlGenerator.EmitLdarg_0();
          tracingIlGenerator.EmitLdarg_1();
          if (typeFromHandle.GetIsEnum())
          {
            tracingIlGenerator.EmitLdarg_1();
            tracingIlGenerator.EmitTypeOf(typeFromHandle);
            tracingIlGenerator.EmitAnyLdc_I4((int) serializer.Key.EnumSerializationMethod);
            tracingIlGenerator.EmitCall(_EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethodMethod);
            tracingIlGenerator.EmitBox(typeof (EnumSerializationMethod));
          }
          else if (DateTimeMessagePackSerializerHelpers.IsDateTime(typeFromHandle))
          {
            tracingIlGenerator.EmitLdarg_1();
            tracingIlGenerator.EmitAnyLdc_I4((int) serializer.Key.DateTimeConversionMethod);
            tracingIlGenerator.EmitCall(_DateTimeMessagePackSerializerHelpers.DetermineDateTimeConversionMethodMethod);
            tracingIlGenerator.EmitBox(typeof (DateTimeConversionMethod));
          }
          else if (serializer.Key.PolymorphismSchema == null)
            tracingIlGenerator.EmitLdnull();
          else
            serializer.Value.SchemaProvider(tracingIlGenerator);
          tracingIlGenerator.EmitCallvirt(target);
          tracingIlGenerator.EmitStfld((FieldInfo) serializer.Value.Field);
        }
        foreach (KeyValuePair<RuntimeFieldHandle, FieldBasedSerializerEmitter.CachedFieldInfo> cachedFieldInfo in this._cachedFieldInfos)
        {
          tracingIlGenerator.EmitLdarg_0();
          tracingIlGenerator.EmitLdtoken(cachedFieldInfo.Value.Target);
          tracingIlGenerator.EmitLdtoken(cachedFieldInfo.Value.Target.DeclaringType);
          tracingIlGenerator.EmitCall(_FieldInfo.GetFieldFromHandle);
          tracingIlGenerator.EmitStfld((FieldInfo) cachedFieldInfo.Value.StorageFieldBuilder);
        }
        foreach (KeyValuePair<RuntimeMethodHandle, FieldBasedSerializerEmitter.CachedMethodBase> cachedMethodBase in this._cachedMethodBases)
        {
          tracingIlGenerator.EmitLdarg_0();
          tracingIlGenerator.EmitLdtoken(cachedMethodBase.Value.Target);
          tracingIlGenerator.EmitLdtoken(cachedMethodBase.Value.Target.DeclaringType);
          tracingIlGenerator.EmitCall(_MethodBase.GetMethodFromHandle);
          tracingIlGenerator.EmitStfld((FieldInfo) cachedMethodBase.Value.StorageFieldBuilder);
        }
        tracingIlGenerator.EmitRet();
      }
      ParameterExpression parameterExpression1;
      ParameterExpression parameterExpression2;
      return Expression.Lambda<Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>>((Expression) Expression.New(this._typeBuilder.CreateType().GetConstructor(FieldBasedSerializerEmitter.ConstructorParameterTypes), (Expression) parameterExpression1), parameterExpression1, parameterExpression2).Compile();
    }

    public override Action<TracingILGenerator, int> RegisterSerializer(
      Type targetType,
      EnumMemberSerializationMethod enumMemberSerializationMethod,
      DateTimeMemberConversionMethod dateTimeConversionMethod,
      PolymorphismSchema polymorphismSchema,
      Func<IEnumerable<ILConstruct>> schemaRegenerationCodeProvider)
    {
      if (this._typeBuilder.IsCreated())
        throw new InvalidOperationException("Type is already built.");
      SerializerFieldKey key = new SerializerFieldKey(targetType, enumMemberSerializationMethod, dateTimeConversionMethod, polymorphismSchema);
      FieldBasedSerializerEmitter.SerializerFieldInfo result;
      if (!this._serializers.TryGetValue(key, out result))
      {
        result = new FieldBasedSerializerEmitter.SerializerFieldInfo(this._typeBuilder.DefineField("_serializer" + (object) this._serializers.Count, typeof (MessagePackSerializer<>).MakeGenericType(targetType), FieldAttributes.Private | FieldAttributes.InitOnly), (Action<TracingILGenerator>) (il =>
        {
          foreach (ILConstruct ilConstruct in schemaRegenerationCodeProvider())
            ilConstruct.Evaluate(il);
        }));
        this._serializers.Add(key, result);
      }
      return (Action<TracingILGenerator, int>) ((il, thisIndex) =>
      {
        il.EmitAnyLdarg(thisIndex);
        il.EmitLdfld((FieldInfo) result.Field);
      });
    }

    public override Action<TracingILGenerator, int> RegisterField(
      FieldInfo field)
    {
      if (this._typeBuilder.IsCreated())
        throw new InvalidOperationException("Type is already built.");
      RuntimeFieldHandle fieldHandle = field.FieldHandle;
      FieldBasedSerializerEmitter.CachedFieldInfo result;
      if (!this._cachedFieldInfos.TryGetValue(fieldHandle, out result))
      {
        result = new FieldBasedSerializerEmitter.CachedFieldInfo(field, this._typeBuilder.DefineField("_field" + field.DeclaringType.Name + "_" + field.Name + (object) this._cachedFieldInfos.Count, typeof (FieldInfo), FieldAttributes.Private | FieldAttributes.InitOnly));
        this._cachedFieldInfos.Add(fieldHandle, result);
      }
      return (Action<TracingILGenerator, int>) ((il, thisIndex) =>
      {
        il.EmitAnyLdarg(thisIndex);
        il.EmitLdfld((FieldInfo) result.StorageFieldBuilder);
      });
    }

    public override Action<TracingILGenerator, int> RegisterMethod(
      MethodBase method)
    {
      if (this._typeBuilder.IsCreated())
        throw new InvalidOperationException("Type is already built.");
      RuntimeMethodHandle methodHandle = method.MethodHandle;
      FieldBasedSerializerEmitter.CachedMethodBase result;
      if (!this._cachedMethodBases.TryGetValue(methodHandle, out result))
      {
        result = new FieldBasedSerializerEmitter.CachedMethodBase(method, this._typeBuilder.DefineField("_function" + method.DeclaringType.Name + "_" + method.Name + (object) this._cachedMethodBases.Count, typeof (FieldInfo), FieldAttributes.Private | FieldAttributes.InitOnly));
        this._cachedMethodBases.Add(methodHandle, result);
      }
      return (Action<TracingILGenerator, int>) ((il, thisIndex) =>
      {
        il.EmitAnyLdarg(thisIndex);
        il.EmitLdfld((FieldInfo) result.StorageFieldBuilder);
      });
    }

    private struct SerializerFieldInfo
    {
      public readonly FieldBuilder Field;
      public readonly Action<TracingILGenerator> SchemaProvider;

      public SerializerFieldInfo(FieldBuilder field, Action<TracingILGenerator> schemaProvider)
      {
        this.Field = field;
        this.SchemaProvider = schemaProvider;
      }
    }

    private struct CachedFieldInfo
    {
      public readonly FieldBuilder StorageFieldBuilder;
      public readonly FieldInfo Target;

      public CachedFieldInfo(FieldInfo target, FieldBuilder storageFieldBuilder)
      {
        this.Target = target;
        this.StorageFieldBuilder = storageFieldBuilder;
      }
    }

    private struct CachedMethodBase
    {
      public readonly FieldBuilder StorageFieldBuilder;
      public readonly MethodBase Target;

      public CachedMethodBase(MethodBase target, FieldBuilder storageFieldBuilder)
      {
        this.Target = target;
        this.StorageFieldBuilder = storageFieldBuilder;
      }
    }
  }
}
