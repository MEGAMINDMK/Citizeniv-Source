// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.ContextBasedSerializerEmitter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class ContextBasedSerializerEmitter : SerializerEmitter
  {
    private static readonly Type[] UnpackFromCoreParameterTypes = new Type[2]
    {
      typeof (SerializationContext),
      typeof (Unpacker)
    };
    private static readonly Type[] CreateInstanceParameterTypes = new Type[2]
    {
      typeof (SerializationContext),
      typeof (int)
    };
    private readonly Type _targetType;
    private readonly CollectionTraits _traits;
    private DynamicMethod _packToMethod;
    private DynamicMethod _unpackFromMethod;
    private DynamicMethod _unpackToMethod;
    private DynamicMethod _createInstanceMethod;
    private DynamicMethod _addItemMethod;
    private DynamicMethod _restoreSchemaMethod;

    public ContextBasedSerializerEmitter(SerializerSpecification specification)
    {
      this._targetType = specification.TargetType;
      this._traits = specification.TargetCollectionTraits;
    }

    public override TracingILGenerator GetPackToMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) "PackToCore");
      if ((MethodInfo) this._packToMethod == (MethodInfo) null)
        this._packToMethod = new DynamicMethod("PackToCore", (Type) null, new Type[3]
        {
          typeof (SerializationContext),
          typeof (Packer),
          this._targetType
        });
      return new TracingILGenerator(this._packToMethod, SerializerDebugging.ILTraceWriter);
    }

    public override TracingILGenerator GetUnpackFromMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) "UnpackFromCore");
      if ((MethodInfo) this._unpackFromMethod == (MethodInfo) null)
        this._unpackFromMethod = new DynamicMethod("UnpackFromCore", this._targetType, ContextBasedSerializerEmitter.UnpackFromCoreParameterTypes);
      return new TracingILGenerator(this._unpackFromMethod, SerializerDebugging.ILTraceWriter);
    }

    public override TracingILGenerator GetUnpackToMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) "UnpackToCore");
      if ((MethodInfo) this._unpackToMethod == (MethodInfo) null)
        this._unpackToMethod = new DynamicMethod("UnpackToCore", (Type) null, new Type[3]
        {
          typeof (SerializationContext),
          typeof (Unpacker),
          this._targetType
        });
      return new TracingILGenerator(this._unpackToMethod, SerializerDebugging.ILTraceWriter);
    }

    public override TracingILGenerator GetAddItemMethodILGenerator(
      MethodInfo declaration)
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) "AddItem");
      if ((MethodInfo) this._addItemMethod == (MethodInfo) null)
      {
        Type[] parameterTypes;
        if (this._traits.DetailedCollectionType != CollectionDetailedKind.GenericDictionary && this._traits.DetailedCollectionType != CollectionDetailedKind.GenericReadOnlyDictionary)
          parameterTypes = new Type[3]
          {
            typeof (SerializationContext),
            this._targetType,
            this._traits.ElementType
          };
        else
          parameterTypes = new Type[4]
          {
            typeof (SerializationContext),
            this._targetType,
            this._traits.ElementType.GetGenericArguments()[0],
            this._traits.ElementType.GetGenericArguments()[1]
          };
        this._addItemMethod = new DynamicMethod("AddItem", (Type) null, parameterTypes);
      }
      return new TracingILGenerator(this._addItemMethod, SerializerDebugging.ILTraceWriter);
    }

    public override TracingILGenerator GetCreateInstanceMethodILGenerator(
      MethodInfo declaration)
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) "CreateInstance");
      if ((MethodInfo) this._createInstanceMethod == (MethodInfo) null)
        this._createInstanceMethod = new DynamicMethod("CreateInstance", this._targetType, ContextBasedSerializerEmitter.CreateInstanceParameterTypes);
      return new TracingILGenerator(this._createInstanceMethod, SerializerDebugging.ILTraceWriter);
    }

    public override TracingILGenerator GetRestoreSchemaMethodILGenerator()
    {
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("{0}::{1}", (object) MethodBase.GetCurrentMethod(), (object) "RestoreSchema");
      if ((MethodInfo) this._restoreSchemaMethod == (MethodInfo) null)
        this._restoreSchemaMethod = new DynamicMethod("RestoreSchema", typeof (PolymorphismSchema), ReflectionAbstractions.EmptyTypes);
      return new TracingILGenerator(this._restoreSchemaMethod, SerializerDebugging.ILTraceWriter);
    }

    public override Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>> CreateConstructor<T>()
    {
      CollectionTraits collectionTraits = typeof (T).GetCollectionTraits();
      DynamicMethod createInstanceMethod = this._createInstanceMethod;
      DynamicMethod unpackFromMethod = this._unpackFromMethod;
      DynamicMethod addItemMethod = this._addItemMethod;
      switch (collectionTraits.DetailedCollectionType)
      {
        case CollectionDetailedKind.GenericList:
        case CollectionDetailedKind.GenericSet:
        case CollectionDetailedKind.GenericCollection:
          ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory factory1 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.CollectionCallbackSerializerFactory<,>).MakeGenericType(typeof (T), collectionTraits.ElementType));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory1.Create(context, schema, createInstanceMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.NonGenericList:
          ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory factory2 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.NonGenericListCallbackSerializerFactory<>).MakeGenericType(typeof (T)));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory2.Create(context, schema, createInstanceMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.GenericDictionary:
          Type[] genericArguments1 = collectionTraits.ElementType.GetGenericArguments();
          ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory factory3 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.DictionaryCallbackSerializerFactory<,,>).MakeGenericType(typeof (T), genericArguments1[0], genericArguments1[1]));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory3.Create(context, schema, createInstanceMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.NonGenericDictionary:
          ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory factory4 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.NonGenericDictionaryCallbackSerializerFactory<>).MakeGenericType(typeof (T)));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory4.Create(context, schema, createInstanceMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.NonGenericCollection:
          ContextBasedSerializerEmitter.IEnumerableCallbackSerializerFactory factory5 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.IEnumerableCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.NonGenericCollectionCallbackSerializerFactory<>).MakeGenericType(typeof (T)));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory5.Create(context, schema, createInstanceMethod, unpackFromMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.GenericEnumerable:
          ContextBasedSerializerEmitter.IEnumerableCallbackSerializerFactory factory6 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.IEnumerableCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.EnumerableCallbackSerializerFactory<,>).MakeGenericType(typeof (T), collectionTraits.ElementType));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory6.Create(context, schema, createInstanceMethod, unpackFromMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.NonGenericEnumerable:
          ContextBasedSerializerEmitter.IEnumerableCallbackSerializerFactory factory7 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.IEnumerableCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.NonGenericEnumerableCallbackSerializerFactory<>).MakeGenericType(typeof (T)));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory7.Create(context, schema, createInstanceMethod, unpackFromMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.GenericReadOnlyList:
        case CollectionDetailedKind.GenericReadOnlyCollection:
          ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory factory8 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.ReadOnlyCollectionCallbackSerializerFactory<,>).MakeGenericType(typeof (T), collectionTraits.ElementType));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory8.Create(context, schema, createInstanceMethod, addItemMethod) as MessagePackSerializer<T>);
        case CollectionDetailedKind.GenericReadOnlyDictionary:
          Type[] genericArguments2 = collectionTraits.ElementType.GetGenericArguments();
          ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory factory9 = MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory>(typeof (ContextBasedSerializerEmitter.ReadOnlyDictionaryCallbackSerializerFactory<,,>).MakeGenericType(typeof (T), genericArguments2[0], genericArguments2[1]));
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => factory9.Create(context, schema, createInstanceMethod, addItemMethod) as MessagePackSerializer<T>);
        default:
          Action<SerializationContext, Packer, T> packTo = this._packToMethod.CreateDelegate(typeof (Action<SerializationContext, Packer, T>)) as Action<SerializationContext, Packer, T>;
          Func<SerializationContext, Unpacker, T> unpackFrom = unpackFromMethod.CreateDelegate(typeof (Func<SerializationContext, Unpacker, T>)) as Func<SerializationContext, Unpacker, T>;
          Action<SerializationContext, Unpacker, T> unpackTo = (Action<SerializationContext, Unpacker, T>) null;
          if ((MethodInfo) this._unpackToMethod != (MethodInfo) null)
            unpackTo = this._unpackToMethod.CreateDelegate(typeof (Action<SerializationContext, Unpacker, T>)) as Action<SerializationContext, Unpacker, T>;
          return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) ((context, schema) => (MessagePackSerializer<T>) new CallbackMessagePackSerializer<T>(context, packTo, unpackFrom, unpackTo));
      }
    }

    public override Action<TracingILGenerator, int> RegisterSerializer(
      Type targetType,
      EnumMemberSerializationMethod enumMemberSerializationMethod,
      DateTimeMemberConversionMethod dateTimeConversionMethod,
      PolymorphismSchema polymorphismSchema,
      Func<IEnumerable<ILConstruct>> schemaRegenerationCodeProvider)
    {
      return (Action<TracingILGenerator, int>) ((g, i) =>
      {
        throw new NotImplementedException();
      });
    }

    private interface IEnumerableCallbackSerializerFactory
    {
      object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        DynamicMethod createInstance,
        DynamicMethod unpackFrom,
        DynamicMethod addItem);
    }

    private abstract class EnumerableCallbackSerializerFactoryBase<TCollection, TItem> : ContextBasedSerializerEmitter.IEnumerableCallbackSerializerFactory
    {
      protected abstract object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Func<SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<SerializationContext, TCollection, TItem> addItem);

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        DynamicMethod createInstance,
        DynamicMethod unpackFrom,
        DynamicMethod addItem)
      {
        SerializationContext context1 = context;
        PolymorphismSchema schema1 = schema;
        Func<SerializationContext, int, TCollection> createInstance1 = (Func<SerializationContext, int, TCollection>) createInstance.CreateDelegate(typeof (Func<,,>).MakeGenericType(typeof (SerializationContext), typeof (int), typeof (TCollection)));
        Func<SerializationContext, Unpacker, TCollection> unpackFrom1 = (Func<SerializationContext, Unpacker, TCollection>) unpackFrom.CreateDelegate(typeof (Func<SerializationContext, Unpacker, TCollection>));
        Action<SerializationContext, TCollection, TItem> addItem1;
        if (!((MethodInfo) addItem == (MethodInfo) null))
          addItem1 = (Action<SerializationContext, TCollection, TItem>) addItem.CreateDelegate(typeof (Action<,,>).MakeGenericType(typeof (SerializationContext), typeof (TCollection), typeof (TItem)));
        else
          addItem1 = (Action<SerializationContext, TCollection, TItem>) null;
        return this.Create(context1, schema1, createInstance1, unpackFrom1, addItem1);
      }
    }

    private sealed class EnumerableCallbackSerializerFactory<TCollection, TItem> : ContextBasedSerializerEmitter.EnumerableCallbackSerializerFactoryBase<TCollection, TItem>
      where TCollection : IEnumerable<TItem>
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Func<SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<SerializationContext, TCollection, TItem> addItem)
      {
        return (object) new CallbackEnumerableMessagePackSerializer<TCollection, TItem>(context, schema, createInstance, unpackFrom, addItem);
      }
    }

    private sealed class NonGenericEnumerableCallbackSerializerFactory<TCollection> : ContextBasedSerializerEmitter.EnumerableCallbackSerializerFactoryBase<TCollection, object>
      where TCollection : IEnumerable
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Func<SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<SerializationContext, TCollection, object> addItem)
      {
        return (object) new CallbackNonGenericEnumerableMessagePackSerializer<TCollection>(context, schema, createInstance, unpackFrom, addItem);
      }
    }

    private sealed class NonGenericCollectionCallbackSerializerFactory<TCollection> : ContextBasedSerializerEmitter.EnumerableCallbackSerializerFactoryBase<TCollection, object>
      where TCollection : ICollection
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Func<SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<SerializationContext, TCollection, object> addItem)
      {
        return (object) new CallbackNonGenericCollectionMessagePackSerializer<TCollection>(context, schema, createInstance, unpackFrom, addItem);
      }
    }

    private interface ICollectionCallbackSerializerFactory
    {
      object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        DynamicMethod createInstance,
        DynamicMethod addItem);
    }

    private abstract class CollectionCallbackSerializerFactoryBase<TCollection, TItem> : ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory
    {
      protected abstract object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Action<SerializationContext, TCollection, TItem> addItem);

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        DynamicMethod createInstance,
        DynamicMethod addItem)
      {
        SerializationContext context1 = context;
        PolymorphismSchema schema1 = schema;
        Func<SerializationContext, int, TCollection> createInstance1 = (Func<SerializationContext, int, TCollection>) createInstance.CreateDelegate(typeof (Func<,,>).MakeGenericType(typeof (SerializationContext), typeof (int), typeof (TCollection)));
        Action<SerializationContext, TCollection, TItem> addItem1;
        if (!((MethodInfo) addItem == (MethodInfo) null))
          addItem1 = (Action<SerializationContext, TCollection, TItem>) addItem.CreateDelegate(typeof (Action<,,>).MakeGenericType(typeof (SerializationContext), typeof (TCollection), typeof (TItem)));
        else
          addItem1 = (Action<SerializationContext, TCollection, TItem>) null;
        return this.Create(context1, schema1, createInstance1, addItem1);
      }
    }

    private sealed class CollectionCallbackSerializerFactory<TCollection, TItem> : ContextBasedSerializerEmitter.CollectionCallbackSerializerFactoryBase<TCollection, TItem>
      where TCollection : ICollection<TItem>
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Action<SerializationContext, TCollection, TItem> addItem)
      {
        return (object) new CallbackCollectionMessagePackSerializer<TCollection, TItem>(context, schema, createInstance);
      }
    }

    private sealed class ReadOnlyCollectionCallbackSerializerFactory<TCollection, TItem> : ContextBasedSerializerEmitter.CollectionCallbackSerializerFactoryBase<TCollection, TItem>
      where TCollection : IReadOnlyCollection<TItem>
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Action<SerializationContext, TCollection, TItem> addItem)
      {
        return (object) new CallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>(context, schema, createInstance, addItem);
      }
    }

    private sealed class NonGenericListCallbackSerializerFactory<TCollection> : ContextBasedSerializerEmitter.CollectionCallbackSerializerFactoryBase<TCollection, object>
      where TCollection : IList
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TCollection> createInstance,
        Action<SerializationContext, TCollection, object> addItem)
      {
        return (object) new CallbackNonGenericListMessagePackSerializer<TCollection>(context, schema, createInstance);
      }
    }

    private sealed class NonGenericDictionaryCallbackSerializerFactory<TDictionary> : ContextBasedSerializerEmitter.CollectionCallbackSerializerFactoryBase<TDictionary, DictionaryEntry>
      where TDictionary : IDictionary
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TDictionary> createInstance,
        Action<SerializationContext, TDictionary, DictionaryEntry> addItem)
      {
        return (object) new CallbackNonGenericDictionaryMessagePackSerializer<TDictionary>(context, schema, createInstance);
      }
    }

    private sealed class DictionaryCallbackSerializerFactory<TDictionary, TKey, TValue> : ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory
      where TDictionary : IDictionary<TKey, TValue>
    {
      private static object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TDictionary> createInstance)
      {
        return (object) new CallbackDictionaryMessagePackSerializer<TDictionary, TKey, TValue>(context, schema, createInstance);
      }

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        DynamicMethod createInstance,
        DynamicMethod addItem)
      {
        return ContextBasedSerializerEmitter.DictionaryCallbackSerializerFactory<TDictionary, TKey, TValue>.Create(context, schema, (Func<SerializationContext, int, TDictionary>) createInstance.CreateDelegate(typeof (Func<,,>).MakeGenericType(typeof (SerializationContext), typeof (int), typeof (TDictionary))));
      }
    }

    private sealed class ReadOnlyDictionaryCallbackSerializerFactory<TDictionary, TKey, TValue> : ContextBasedSerializerEmitter.ICollectionCallbackSerializerFactory
      where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
      private static object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<SerializationContext, int, TDictionary> createInstance,
        Action<SerializationContext, TDictionary, TKey, TValue> addItem)
      {
        return (object) new CallbackReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>(context, schema, createInstance, addItem);
      }

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        DynamicMethod createInstance,
        DynamicMethod addItem)
      {
        SerializationContext context1 = context;
        PolymorphismSchema schema1 = schema;
        Func<SerializationContext, int, TDictionary> createInstance1 = (Func<SerializationContext, int, TDictionary>) createInstance.CreateDelegate(typeof (Func<,,>).MakeGenericType(typeof (SerializationContext), typeof (int), typeof (TDictionary)));
        Action<SerializationContext, TDictionary, TKey, TValue> addItem1;
        if (!((MethodInfo) addItem == (MethodInfo) null))
          addItem1 = (Action<SerializationContext, TDictionary, TKey, TValue>) addItem.CreateDelegate(typeof (Action<,,,>).MakeGenericType(typeof (SerializationContext), typeof (TDictionary), typeof (TKey), typeof (TValue)));
        else
          addItem1 = (Action<SerializationContext, TDictionary, TKey, TValue>) null;
        return ContextBasedSerializerEmitter.ReadOnlyDictionaryCallbackSerializerFactory<TDictionary, TKey, TValue>.Create(context1, schema1, createInstance1, addItem1);
      }
    }
  }
}
