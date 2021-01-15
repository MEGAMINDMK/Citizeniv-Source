// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionTreeSerializerBuilderHelpers
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal static class ExpressionTreeSerializerBuilderHelpers
  {
    public static Type GetSerializerClass(Type targetType, CollectionTraits traits)
    {
      switch (traits.DetailedCollectionType)
      {
        case CollectionDetailedKind.GenericList:
        case CollectionDetailedKind.GenericSet:
        case CollectionDetailedKind.GenericCollection:
          return typeof (ExpressionCallbackCollectionMessagePackSerializer<,>).MakeGenericType(targetType, traits.ElementType);
        case CollectionDetailedKind.NonGenericList:
          return typeof (ExpressionCallbackNonGenericListMessagePackSerializer<>).MakeGenericType(targetType);
        case CollectionDetailedKind.GenericDictionary:
          Type[] genericArguments1 = traits.ElementType.GetGenericArguments();
          return typeof (ExpressionCallbackDictionaryMessagePackSerializer<,,>).MakeGenericType(targetType, genericArguments1[0], genericArguments1[1]);
        case CollectionDetailedKind.NonGenericDictionary:
          return typeof (ExpressionCallbackNonGenericDictionaryMessagePackSerializer<>).MakeGenericType(targetType);
        case CollectionDetailedKind.NonGenericCollection:
          return typeof (ExpressionCallbackNonGenericCollectionMessagePackSerializer<>).MakeGenericType(targetType);
        case CollectionDetailedKind.GenericEnumerable:
          return typeof (ExpressionCallbackEnumerableMessagePackSerializer<,>).MakeGenericType(targetType, traits.ElementType);
        case CollectionDetailedKind.NonGenericEnumerable:
          return typeof (ExpressionCallbackNonGenericEnumerableMessagePackSerializer<>).MakeGenericType(targetType);
        case CollectionDetailedKind.GenericReadOnlyList:
        case CollectionDetailedKind.GenericReadOnlyCollection:
          return typeof (ExpressionCallbackReadOnlyCollectionMessagePackSerializer<,>).MakeGenericType(targetType, traits.ElementType);
        case CollectionDetailedKind.GenericReadOnlyDictionary:
          Type[] genericArguments2 = traits.ElementType.GetGenericArguments();
          return typeof (ExpressionCallbackReadOnlyDictionaryMessagePackSerializer<,,>).MakeGenericType(targetType, genericArguments2[0], genericArguments2[1]);
        default:
          return !targetType.GetIsEnum() ? typeof (ExpressionCallbackMessagePackSerializer<>).MakeGenericType(targetType) : typeof (ExpressionCallbackEnumMessagePackSerializer<>).MakeGenericType(targetType);
      }
    }

    public static Func<SerializationContext, MessagePackSerializer<TObject>> CreateFactory<TObject>(
      ExpressionTreeContext codeGenerationContext,
      CollectionTraits traits,
      PolymorphismSchema schema)
    {
      Delegate packToCore = codeGenerationContext.GetPackToCore();
      Delegate unpackFromCore = codeGenerationContext.GetUnpackFromCore();
      Delegate unpackToCore = codeGenerationContext.GetUnpackToCore();
      Delegate createInstance = codeGenerationContext.GetCreateInstance();
      Delegate addItem = codeGenerationContext.GetAddItem();
      switch (traits.DetailedCollectionType)
      {
        case CollectionDetailedKind.GenericList:
        case CollectionDetailedKind.GenericSet:
        case CollectionDetailedKind.GenericCollection:
          ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory factory1 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.CollectionCallbackSerializerFactory<,>).MakeGenericType(typeof (TObject), traits.ElementType));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory1.Create(context, schema, createInstance, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.NonGenericList:
          ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory factory2 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.NonGenericListCallbackSerializerFactory<>).MakeGenericType(typeof (TObject)));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory2.Create(context, schema, createInstance, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.GenericDictionary:
          Type[] genericArguments1 = traits.ElementType.GetGenericArguments();
          ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory factory3 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.DictionaryCallbackSerializerFactory<,,>).MakeGenericType(typeof (TObject), genericArguments1[0], genericArguments1[1]));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory3.Create(context, schema, createInstance, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.NonGenericDictionary:
          ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory factory4 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.NonGenericDictionaryCallbackSerializerFactory<>).MakeGenericType(typeof (TObject)));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory4.Create(context, schema, createInstance, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.NonGenericCollection:
          ExpressionTreeSerializerBuilderHelpers.IEnumerableCallbackSerializerFactory factory5 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.IEnumerableCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.NonGenericCollectionCallbackSerializerFactory<>).MakeGenericType(typeof (TObject)));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory5.Create(context, schema, createInstance, unpackFromCore, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.GenericEnumerable:
          ExpressionTreeSerializerBuilderHelpers.IEnumerableCallbackSerializerFactory factory6 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.IEnumerableCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.EnumerableCallbackSerializerFactory<,>).MakeGenericType(typeof (TObject), traits.ElementType));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory6.Create(context, schema, createInstance, unpackFromCore, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.NonGenericEnumerable:
          ExpressionTreeSerializerBuilderHelpers.IEnumerableCallbackSerializerFactory factory7 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.IEnumerableCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.NonGenericEnumerableCallbackSerializerFactory<>).MakeGenericType(typeof (TObject)));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory7.Create(context, schema, createInstance, unpackFromCore, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.GenericReadOnlyList:
        case CollectionDetailedKind.GenericReadOnlyCollection:
          ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory factory8 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.ReadOnlyCollectionCallbackSerializerFactory<,>).MakeGenericType(typeof (TObject), traits.ElementType));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory8.Create(context, schema, createInstance, addItem) as MessagePackSerializer<TObject>);
        case CollectionDetailedKind.GenericReadOnlyDictionary:
          Type[] genericArguments2 = traits.ElementType.GetGenericArguments();
          ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory factory9 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.ReadOnlyDictionaryCallbackSerializerFactory<,,>).MakeGenericType(typeof (TObject), genericArguments2[0], genericArguments2[1]));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory9.Create(context, schema, createInstance, addItem) as MessagePackSerializer<TObject>);
        default:
          ExpressionTreeSerializerBuilderHelpers.ICallbackSerializerFactory factory10 = ReflectionExtensions.CreateInstancePreservingExceptionType<ExpressionTreeSerializerBuilderHelpers.ICallbackSerializerFactory>(typeof (ExpressionTreeSerializerBuilderHelpers.CallbackSerializerFactory<>).MakeGenericType(typeof (TObject)));
          return (Func<SerializationContext, MessagePackSerializer<TObject>>) (context => factory10.Create(context, packToCore, unpackFromCore, unpackToCore) as MessagePackSerializer<TObject>);
      }
    }

    private interface ICallbackSerializerFactory
    {
      object Create(
        SerializationContext context,
        Delegate packTo,
        Delegate unpackFrom,
        Delegate unpackTo);
    }

    private sealed class CallbackSerializerFactory<T> : ExpressionTreeSerializerBuilderHelpers.ICallbackSerializerFactory
    {
      private static object Create(
        SerializationContext context,
        Action<ExpressionCallbackMessagePackSerializer<T>, SerializationContext, Packer, T> packTo,
        Func<ExpressionCallbackMessagePackSerializer<T>, SerializationContext, Unpacker, T> unpackFrom,
        Action<ExpressionCallbackMessagePackSerializer<T>, SerializationContext, Unpacker, T> unpackTo)
      {
        return (object) new ExpressionCallbackMessagePackSerializer<T>(context, packTo, unpackFrom, unpackTo);
      }

      public object Create(
        SerializationContext context,
        Delegate packTo,
        Delegate unpackFrom,
        Delegate unpackTo)
      {
        return ExpressionTreeSerializerBuilderHelpers.CallbackSerializerFactory<T>.Create(context, (Action<ExpressionCallbackMessagePackSerializer<T>, SerializationContext, Packer, T>) packTo, (Func<ExpressionCallbackMessagePackSerializer<T>, SerializationContext, Unpacker, T>) unpackFrom, (Action<ExpressionCallbackMessagePackSerializer<T>, SerializationContext, Unpacker, T>) unpackTo);
      }
    }

    private interface IEnumerableCallbackSerializerFactory
    {
      object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Delegate createInstance,
        Delegate unpackFrom,
        Delegate addItem);
    }

    private abstract class EnumerableCallbackSerializerFactoryBase<TCollection, TItem, TSerializer> : ExpressionTreeSerializerBuilderHelpers.IEnumerableCallbackSerializerFactory
    {
      protected abstract object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<TSerializer, SerializationContext, int, TCollection> createInstance,
        Func<TSerializer, SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<TSerializer, SerializationContext, TCollection, TItem> addItem);

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Delegate createInstance,
        Delegate unpackFrom,
        Delegate addItem)
      {
        return this.Create(context, schema, (Func<TSerializer, SerializationContext, int, TCollection>) createInstance, (Func<TSerializer, SerializationContext, Unpacker, TCollection>) unpackFrom, (Action<TSerializer, SerializationContext, TCollection, TItem>) addItem);
      }
    }

    private sealed class EnumerableCallbackSerializerFactory<TCollection, TItem> : ExpressionTreeSerializerBuilderHelpers.EnumerableCallbackSerializerFactoryBase<TCollection, TItem, ExpressionCallbackEnumerableMessagePackSerializer<TCollection, TItem>>
      where TCollection : IEnumerable<TItem>
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackEnumerableMessagePackSerializer<TCollection, TItem>, SerializationContext, int, TCollection> createInstance,
        Func<ExpressionCallbackEnumerableMessagePackSerializer<TCollection, TItem>, SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<ExpressionCallbackEnumerableMessagePackSerializer<TCollection, TItem>, SerializationContext, TCollection, TItem> addItem)
      {
        return (object) new ExpressionCallbackEnumerableMessagePackSerializer<TCollection, TItem>(context, schema, createInstance, unpackFrom, addItem);
      }
    }

    private sealed class NonGenericEnumerableCallbackSerializerFactory<TCollection> : ExpressionTreeSerializerBuilderHelpers.EnumerableCallbackSerializerFactoryBase<TCollection, object, ExpressionCallbackNonGenericEnumerableMessagePackSerializer<TCollection>>
      where TCollection : IEnumerable
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackNonGenericEnumerableMessagePackSerializer<TCollection>, SerializationContext, int, TCollection> createInstance,
        Func<ExpressionCallbackNonGenericEnumerableMessagePackSerializer<TCollection>, SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<ExpressionCallbackNonGenericEnumerableMessagePackSerializer<TCollection>, SerializationContext, TCollection, object> addItem)
      {
        return (object) new ExpressionCallbackNonGenericEnumerableMessagePackSerializer<TCollection>(context, schema, createInstance, unpackFrom, addItem);
      }
    }

    private sealed class NonGenericCollectionCallbackSerializerFactory<TCollection> : ExpressionTreeSerializerBuilderHelpers.EnumerableCallbackSerializerFactoryBase<TCollection, object, ExpressionCallbackNonGenericCollectionMessagePackSerializer<TCollection>>
      where TCollection : ICollection
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackNonGenericCollectionMessagePackSerializer<TCollection>, SerializationContext, int, TCollection> createInstance,
        Func<ExpressionCallbackNonGenericCollectionMessagePackSerializer<TCollection>, SerializationContext, Unpacker, TCollection> unpackFrom,
        Action<ExpressionCallbackNonGenericCollectionMessagePackSerializer<TCollection>, SerializationContext, TCollection, object> addItem)
      {
        return (object) new ExpressionCallbackNonGenericCollectionMessagePackSerializer<TCollection>(context, schema, createInstance, unpackFrom, addItem);
      }
    }

    private interface ICollectionCallbackSerializerFactory
    {
      object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Delegate createInstance,
        Delegate addItem);
    }

    private abstract class CollectionCallbackSerializerFactoryBase<TCollection, TItem, TSerializer> : ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory
    {
      protected abstract object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<TSerializer, SerializationContext, int, TCollection> createInstance,
        Action<TSerializer, SerializationContext, TCollection, TItem> addItem);

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Delegate createInstance,
        Delegate addItem)
      {
        return this.Create(context, schema, (Func<TSerializer, SerializationContext, int, TCollection>) createInstance, (Action<TSerializer, SerializationContext, TCollection, TItem>) addItem);
      }
    }

    private sealed class CollectionCallbackSerializerFactory<TCollection, TItem> : ExpressionTreeSerializerBuilderHelpers.CollectionCallbackSerializerFactoryBase<TCollection, TItem, ExpressionCallbackCollectionMessagePackSerializer<TCollection, TItem>>
      where TCollection : ICollection<TItem>
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, int, TCollection> createInstance,
        Action<ExpressionCallbackCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, TCollection, TItem> addItem)
      {
        return (object) new ExpressionCallbackCollectionMessagePackSerializer<TCollection, TItem>(context, schema, createInstance);
      }
    }

    private sealed class ReadOnlyCollectionCallbackSerializerFactory<TCollection, TItem> : ExpressionTreeSerializerBuilderHelpers.CollectionCallbackSerializerFactoryBase<TCollection, TItem, ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>>
      where TCollection : IReadOnlyCollection<TItem>
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, int, TCollection> createInstance,
        Action<ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, TCollection, TItem> addItem)
      {
        return (object) new ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>(context, schema, createInstance, addItem);
      }
    }

    private sealed class NonGenericListCallbackSerializerFactory<TCollection> : ExpressionTreeSerializerBuilderHelpers.CollectionCallbackSerializerFactoryBase<TCollection, object, ExpressionCallbackNonGenericListMessagePackSerializer<TCollection>>
      where TCollection : IList
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackNonGenericListMessagePackSerializer<TCollection>, SerializationContext, int, TCollection> createInstance,
        Action<ExpressionCallbackNonGenericListMessagePackSerializer<TCollection>, SerializationContext, TCollection, object> addItem)
      {
        return (object) new ExpressionCallbackNonGenericListMessagePackSerializer<TCollection>(context, schema, createInstance);
      }
    }

    private sealed class NonGenericDictionaryCallbackSerializerFactory<TDictionary> : ExpressionTreeSerializerBuilderHelpers.CollectionCallbackSerializerFactoryBase<TDictionary, DictionaryEntry, ExpressionCallbackNonGenericDictionaryMessagePackSerializer<TDictionary>>
      where TDictionary : IDictionary
    {
      protected override object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackNonGenericDictionaryMessagePackSerializer<TDictionary>, SerializationContext, int, TDictionary> createInstance,
        Action<ExpressionCallbackNonGenericDictionaryMessagePackSerializer<TDictionary>, SerializationContext, TDictionary, DictionaryEntry> addItem)
      {
        return (object) new ExpressionCallbackNonGenericDictionaryMessagePackSerializer<TDictionary>(context, schema, createInstance);
      }
    }

    private sealed class DictionaryCallbackSerializerFactory<TDictionary, TKey, TValue> : ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory
      where TDictionary : IDictionary<TKey, TValue>
    {
      private static object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, int, TDictionary> createInstance)
      {
        return (object) new ExpressionCallbackDictionaryMessagePackSerializer<TDictionary, TKey, TValue>(context, schema, createInstance);
      }

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Delegate createInstance,
        Delegate addItem)
      {
        return ExpressionTreeSerializerBuilderHelpers.DictionaryCallbackSerializerFactory<TDictionary, TKey, TValue>.Create(context, schema, (Func<ExpressionCallbackDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, int, TDictionary>) createInstance);
      }
    }

    private sealed class ReadOnlyDictionaryCallbackSerializerFactory<TDictionary, TKey, TValue> : ExpressionTreeSerializerBuilderHelpers.ICollectionCallbackSerializerFactory
      where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
      private static object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Func<ExpressionCallbackReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, int, TDictionary> createInstance,
        Action<ExpressionCallbackReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, TDictionary, TKey, TValue> addItem)
      {
        return (object) new ExpressionCallbackReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>(context, schema, createInstance, addItem);
      }

      public object Create(
        SerializationContext context,
        PolymorphismSchema schema,
        Delegate createInstance,
        Delegate addItem)
      {
        return ExpressionTreeSerializerBuilderHelpers.ReadOnlyDictionaryCallbackSerializerFactory<TDictionary, TKey, TValue>.Create(context, schema, (Func<ExpressionCallbackReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, int, TDictionary>) createInstance, (Action<ExpressionCallbackReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>, SerializationContext, TDictionary, TKey, TValue>) addItem);
      }
    }
  }
}
