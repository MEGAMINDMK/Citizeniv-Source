// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.GenericSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal static class GenericSerializer
  {
    public static MessagePackSerializer<T> Create<T>(
      SerializationContext context,
      PolymorphismSchema schema)
    {
      return GenericSerializer.Create(context, typeof (T), schema) as MessagePackSerializer<T>;
    }

    public static IMessagePackSingleObjectSerializer Create(
      SerializationContext context,
      Type targetType,
      PolymorphismSchema schema)
    {
      if (targetType.IsArray)
        return GenericSerializer.CreateArraySerializer(context, targetType, (schema ?? PolymorphismSchema.Default).ItemSchema);
      Type underlyingType;
      if ((underlyingType = Nullable.GetUnderlyingType(targetType)) != (Type) null)
        return GenericSerializer.CreateNullableSerializer(context, underlyingType, schema);
      if (targetType.GetIsGenericType())
      {
        Type genericTypeDefinition = targetType.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (List<>))
          return GenericSerializer.CreateListSerializer(context, targetType.GetGenericArguments()[0], schema);
        if (genericTypeDefinition == typeof (Dictionary<,>))
        {
          Type[] genericArguments = targetType.GetGenericArguments();
          return GenericSerializer.CreateDictionarySerializer(context, genericArguments[0], genericArguments[1], schema);
        }
      }
      return GenericSerializer.TryCreateImmutableCollectionSerializer(context, targetType, schema);
    }

    private static IMessagePackSingleObjectSerializer CreateArraySerializer(
      SerializationContext context,
      Type targetType,
      PolymorphismSchema itemsSchema)
    {
      return ArraySerializer.Create(context, targetType, itemsSchema);
    }

    private static IMessagePackSingleObjectSerializer CreateNullableSerializer(
      SerializationContext context,
      Type underlyingType,
      PolymorphismSchema schema)
    {
      return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IGenericBuiltInSerializerFactory>(typeof (GenericSerializer.NullableInstanceFactory<>).MakeGenericType(underlyingType)).Create(context, schema);
    }

    private static IMessagePackSingleObjectSerializer CreateListSerializer(
      SerializationContext context,
      Type itemType,
      PolymorphismSchema schema)
    {
      return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IGenericBuiltInSerializerFactory>(typeof (GenericSerializer.ListInstanceFactory<>).MakeGenericType(itemType)).Create(context, schema);
    }

    private static IMessagePackSingleObjectSerializer CreateDictionarySerializer(
      SerializationContext context,
      Type keyType,
      Type valueType,
      PolymorphismSchema schema)
    {
      return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IGenericBuiltInSerializerFactory>(typeof (GenericSerializer.DictionaryInstanceFactory<,>).MakeGenericType(keyType, valueType)).Create(context, schema);
    }

    private static IMessagePackSingleObjectSerializer TryCreateImmutableCollectionSerializer(
      SerializationContext context,
      Type targetType,
      PolymorphismSchema schema)
    {
      if (targetType.Namespace != "System.Collections.Immutable")
        return (IMessagePackSingleObjectSerializer) null;
      if (!targetType.GetIsGenericType())
        return (IMessagePackSingleObjectSerializer) null;
      PolymorphismSchema schema1 = schema ?? PolymorphismSchema.Default;
      switch (GenericSerializer.DetermineImmutableCollectionType(targetType))
      {
        case GenericSerializer.ImmutableCollectionType.ImmutableArray:
        case GenericSerializer.ImmutableCollectionType.ImmutableHashSet:
        case GenericSerializer.ImmutableCollectionType.ImmutableList:
        case GenericSerializer.ImmutableCollectionType.ImmutableQueue:
        case GenericSerializer.ImmutableCollectionType.ImmutableSortedSet:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IGenericBuiltInSerializerFactory>(typeof (GenericSerializer.ImmutableCollectionSerializerFactory<,>).MakeGenericType(targetType, targetType.GetGenericArguments()[0])).Create(context, schema1);
        case GenericSerializer.ImmutableCollectionType.ImmutableDictionary:
        case GenericSerializer.ImmutableCollectionType.ImmutableSortedDictionary:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IGenericBuiltInSerializerFactory>(typeof (GenericSerializer.ImmutableDictionarySerializerFactory<,,>).MakeGenericType(targetType, targetType.GetGenericArguments()[0], targetType.GetGenericArguments()[1])).Create(context, schema1);
        case GenericSerializer.ImmutableCollectionType.ImmutableStack:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IGenericBuiltInSerializerFactory>(typeof (GenericSerializer.ImmutableStackSerializerFactory<,>).MakeGenericType(targetType, targetType.GetGenericArguments()[0])).Create(context, schema1);
        default:
          return (IMessagePackSingleObjectSerializer) null;
      }
    }

    private static GenericSerializer.ImmutableCollectionType DetermineImmutableCollectionType(
      Type targetType)
    {
      if (targetType.Namespace != "System.Collections.Immutable" || !targetType.GetIsGenericType())
        return GenericSerializer.ImmutableCollectionType.Unknown;
      switch (targetType.GetGenericTypeDefinition().Name)
      {
        case "ImmutableArray`1":
          return GenericSerializer.ImmutableCollectionType.ImmutableArray;
        case "ImmutableList`1":
          return GenericSerializer.ImmutableCollectionType.ImmutableList;
        case "ImmutableHashSet`1":
          return GenericSerializer.ImmutableCollectionType.ImmutableHashSet;
        case "ImmutableSortedSet`1":
          return GenericSerializer.ImmutableCollectionType.ImmutableSortedSet;
        case "ImmutableQueue`1":
          return GenericSerializer.ImmutableCollectionType.ImmutableQueue;
        case "ImmutableStack`1":
          return GenericSerializer.ImmutableCollectionType.ImmutableStack;
        case "ImmutableDictionary`2":
          return GenericSerializer.ImmutableCollectionType.ImmutableDictionary;
        case "ImmutableSortedDictionary`2":
          return GenericSerializer.ImmutableCollectionType.ImmutableSortedDictionary;
        default:
          return GenericSerializer.ImmutableCollectionType.Unknown;
      }
    }

    public static IMessagePackSingleObjectSerializer TryCreateAbstractCollectionSerializer(
      SerializationContext context,
      Type abstractType,
      Type concreteType,
      PolymorphismSchema schema)
    {
      return concreteType == (Type) null ? (IMessagePackSingleObjectSerializer) null : GenericSerializer.TryCreateAbstractCollectionSerializer(context, abstractType, concreteType, schema, abstractType.GetCollectionTraits());
    }

    internal static IMessagePackSingleObjectSerializer TryCreateAbstractCollectionSerializer(
      SerializationContext context,
      Type abstractType,
      Type concreteType,
      PolymorphismSchema schema,
      CollectionTraits traits)
    {
      switch (traits.DetailedCollectionType)
      {
        case CollectionDetailedKind.GenericList:
        case CollectionDetailedKind.GenericSet:
        case CollectionDetailedKind.GenericCollection:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.CollectionSerializerFactory<,>).MakeGenericType(abstractType, traits.ElementType)).Create(context, concreteType, schema);
        case CollectionDetailedKind.NonGenericList:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.NonGenericListSerializerFactory<>).MakeGenericType(abstractType)).Create(context, concreteType, schema);
        case CollectionDetailedKind.GenericDictionary:
          Type[] genericArguments1 = traits.ElementType.GetGenericArguments();
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.DictionarySerializerFactory<,,>).MakeGenericType(abstractType, genericArguments1[0], genericArguments1[1])).Create(context, concreteType, schema);
        case CollectionDetailedKind.NonGenericDictionary:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.NonGenericDictionarySerializerFactory<>).MakeGenericType(abstractType)).Create(context, concreteType, schema);
        case CollectionDetailedKind.NonGenericCollection:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.NonGenericCollectionSerializerFactory<>).MakeGenericType(abstractType)).Create(context, concreteType, schema);
        case CollectionDetailedKind.GenericEnumerable:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.EnumerableSerializerFactory<,>).MakeGenericType(abstractType, traits.ElementType)).Create(context, concreteType, schema);
        case CollectionDetailedKind.NonGenericEnumerable:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.NonGenericEnumerableSerializerFactory<>).MakeGenericType(abstractType)).Create(context, concreteType, schema);
        case CollectionDetailedKind.GenericReadOnlyList:
        case CollectionDetailedKind.GenericReadOnlyCollection:
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.ReadOnlyCollectionSerializerFactory<,>).MakeGenericType(abstractType, traits.ElementType)).Create(context, concreteType, schema);
        case CollectionDetailedKind.GenericReadOnlyDictionary:
          Type[] genericArguments2 = traits.ElementType.GetGenericArguments();
          return ReflectionExtensions.CreateInstancePreservingExceptionType<GenericSerializer.IVariantSerializerFactory>(typeof (GenericSerializer.ReadOnlyDictionarySerializerFactory<,,>).MakeGenericType(abstractType, genericArguments2[0], genericArguments2[1])).Create(context, concreteType, schema);
        default:
          return (IMessagePackSingleObjectSerializer) null;
      }
    }

    internal static bool IsSupported(
      Type type,
      CollectionTraits traits,
      bool preferReflectionBasedSerializer)
    {
      if (type.IsArray || Nullable.GetUnderlyingType(type) != (Type) null)
        return true;
      if (type.GetIsGenericType())
      {
        Type genericTypeDefinition = type.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (List<>) || genericTypeDefinition == typeof (Dictionary<,>))
          return true;
      }
      if (GenericSerializer.DetermineImmutableCollectionType(type) != GenericSerializer.ImmutableCollectionType.Unknown)
        return true;
      return preferReflectionBasedSerializer && traits.DetailedCollectionType != CollectionDetailedKind.NotCollection && traits.DetailedCollectionType != CollectionDetailedKind.Unserializable;
    }

    private interface IGenericBuiltInSerializerFactory
    {
      IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        PolymorphismSchema schema);
    }

    private sealed class NullableInstanceFactory<T> : GenericSerializer.IGenericBuiltInSerializerFactory
      where T : struct
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new NullableMessagePackSerializer<T>(context);
      }
    }

    private sealed class ListInstanceFactory<T> : GenericSerializer.IGenericBuiltInSerializerFactory
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        PolymorphismSchema schema)
      {
        PolymorphismSchema polymorphismSchema = schema ?? PolymorphismSchema.Default;
        return (IMessagePackSingleObjectSerializer) new System_Collections_Generic_List_1MessagePackSerializer<T>(context, polymorphismSchema.ItemSchema);
      }
    }

    private sealed class DictionaryInstanceFactory<TKey, TValue> : GenericSerializer.IGenericBuiltInSerializerFactory
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        PolymorphismSchema schema)
      {
        PolymorphismSchema polymorphismSchema = schema ?? PolymorphismSchema.Default;
        return (IMessagePackSingleObjectSerializer) new System_Collections_Generic_Dictionary_2MessagePackSerializer<TKey, TValue>(context, polymorphismSchema.KeySchema, polymorphismSchema.ItemSchema);
      }
    }

    private sealed class ImmutableCollectionSerializerFactory<T, TItem> : GenericSerializer.IGenericBuiltInSerializerFactory
      where T : IEnumerable<TItem>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        PolymorphismSchema schema)
      {
        PolymorphismSchema polymorphismSchema = schema ?? PolymorphismSchema.Default;
        return (IMessagePackSingleObjectSerializer) new ImmutableCollectionSerializer<T, TItem>(context, polymorphismSchema.ItemSchema);
      }
    }

    private sealed class ImmutableStackSerializerFactory<T, TItem> : GenericSerializer.IGenericBuiltInSerializerFactory
      where T : IEnumerable<TItem>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        PolymorphismSchema schema)
      {
        PolymorphismSchema polymorphismSchema = schema ?? PolymorphismSchema.Default;
        return (IMessagePackSingleObjectSerializer) new ImmutableStackSerializer<T, TItem>(context, polymorphismSchema.ItemSchema);
      }
    }

    private sealed class ImmutableDictionarySerializerFactory<T, TKey, TValue> : GenericSerializer.IGenericBuiltInSerializerFactory
      where T : IDictionary<TKey, TValue>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        PolymorphismSchema schema)
      {
        PolymorphismSchema polymorphismSchema = schema ?? PolymorphismSchema.Default;
        return (IMessagePackSingleObjectSerializer) new ImmutableDictionarySerializer<T, TKey, TValue>(context, polymorphismSchema.KeySchema, polymorphismSchema.ItemSchema);
      }
    }

    private interface IVariantSerializerFactory
    {
      IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema);
    }

    private sealed class NonGenericEnumerableSerializerFactory<T> : GenericSerializer.IVariantSerializerFactory
      where T : IEnumerable
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractNonGenericEnumerableMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class NonGenericCollectionSerializerFactory<T> : GenericSerializer.IVariantSerializerFactory
      where T : ICollection
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractNonGenericCollectionMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class NonGenericListSerializerFactory<T> : GenericSerializer.IVariantSerializerFactory
      where T : IList
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractNonGenericListMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class NonGenericDictionarySerializerFactory<T> : GenericSerializer.IVariantSerializerFactory
      where T : IDictionary
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractNonGenericDictionaryMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class EnumerableSerializerFactory<TCollection, TItem> : GenericSerializer.IVariantSerializerFactory
      where TCollection : IEnumerable<TItem>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractEnumerableMessagePackSerializer<TCollection, TItem>(context, targetType, schema);
      }
    }

    private sealed class CollectionSerializerFactory<TCollection, TItem> : GenericSerializer.IVariantSerializerFactory
      where TCollection : ICollection<TItem>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractCollectionMessagePackSerializer<TCollection, TItem>(context, targetType, schema);
      }
    }

    private sealed class ReadOnlyCollectionSerializerFactory<TCollection, TItem> : GenericSerializer.IVariantSerializerFactory
      where TCollection : IReadOnlyCollection<TItem>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractReadOnlyCollectionMessagePackSerializer<TCollection, TItem>(context, targetType, schema);
      }
    }

    private sealed class DictionarySerializerFactory<TDictionary, TKey, TValue> : GenericSerializer.IVariantSerializerFactory
      where TDictionary : IDictionary<TKey, TValue>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractDictionaryMessagePackSerializer<TDictionary, TKey, TValue>(context, targetType, schema);
      }
    }

    private sealed class ReadOnlyDictionarySerializerFactory<TDictionary, TKey, TValue> : GenericSerializer.IVariantSerializerFactory
      where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new AbstractReadOnlyDictionaryMessagePackSerializer<TDictionary, TKey, TValue>(context, targetType, schema);
      }
    }

    private enum ImmutableCollectionType
    {
      Unknown,
      ImmutableArray,
      ImmutableDictionary,
      ImmutableHashSet,
      ImmutableList,
      ImmutableQueue,
      ImmutableSortedDictionary,
      ImmutableSortedSet,
      ImmutableStack,
    }
  }
}
