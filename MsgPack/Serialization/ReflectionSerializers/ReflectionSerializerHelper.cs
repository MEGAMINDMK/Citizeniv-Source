// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionSerializerHelper
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.DefaultSerializers;
using MsgPack.Serialization.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal static class ReflectionSerializerHelper
  {
    public static MessagePackSerializer<T> CreateReflectionEnumMessagePackSerializer<T>(
      SerializationContext context)
    {
      return MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<MessagePackSerializer<T>>(typeof (ReflectionEnumMessagePackSerializer<>).MakeGenericType(typeof (T)), (object) context);
    }

    public static MessagePackSerializer<T> CreateCollectionSerializer<T>(
      SerializationContext context,
      Type targetType,
      CollectionTraits traits,
      PolymorphismSchema schema)
    {
      switch (traits.DetailedCollectionType)
      {
        case CollectionDetailedKind.Array:
          return ArraySerializer.Create<T>(context, schema);
        case CollectionDetailedKind.GenericList:
        case CollectionDetailedKind.GenericSet:
        case CollectionDetailedKind.GenericCollection:
          return (MessagePackSerializer<T>) MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ReflectionSerializerHelper.IVariantReflectionSerializerFactory>(typeof (ReflectionSerializerHelper.CollectionSerializerFactory<,>).MakeGenericType(typeof (T), traits.ElementType)).Create(context, targetType, schema);
        case CollectionDetailedKind.NonGenericList:
          return (MessagePackSerializer<T>) MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ReflectionSerializerHelper.IVariantReflectionSerializerFactory>(typeof (ReflectionSerializerHelper.NonGenericListSerializerFactory<>).MakeGenericType(typeof (T))).Create(context, targetType, schema);
        case CollectionDetailedKind.GenericDictionary:
          Type[] genericArguments = traits.ElementType.GetGenericArguments();
          return (MessagePackSerializer<T>) MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ReflectionSerializerHelper.IVariantReflectionSerializerFactory>(typeof (ReflectionSerializerHelper.DictionarySerializerFactory<,,>).MakeGenericType(typeof (T), genericArguments[0], genericArguments[1])).Create(context, targetType, schema);
        case CollectionDetailedKind.NonGenericDictionary:
          return (MessagePackSerializer<T>) MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ReflectionSerializerHelper.IVariantReflectionSerializerFactory>(typeof (ReflectionSerializerHelper.NonGenericDictionarySerializerFactory<>).MakeGenericType(typeof (T))).Create(context, targetType, schema);
        case CollectionDetailedKind.NonGenericCollection:
          return (MessagePackSerializer<T>) MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ReflectionSerializerHelper.IVariantReflectionSerializerFactory>(typeof (ReflectionSerializerHelper.NonGenericCollectionSerializerFactory<>).MakeGenericType(typeof (T), traits.ElementType)).Create(context, targetType, schema);
        case CollectionDetailedKind.GenericEnumerable:
          return (MessagePackSerializer<T>) MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ReflectionSerializerHelper.IVariantReflectionSerializerFactory>(typeof (ReflectionSerializerHelper.EnumerableSerializerFactory<,>).MakeGenericType(typeof (T), traits.ElementType)).Create(context, targetType, schema);
        case CollectionDetailedKind.NonGenericEnumerable:
          return (MessagePackSerializer<T>) MsgPack.Serialization.ReflectionExtensions.CreateInstancePreservingExceptionType<ReflectionSerializerHelper.IVariantReflectionSerializerFactory>(typeof (ReflectionSerializerHelper.NonGenericEnumerableSerializerFactory<>).MakeGenericType(typeof (T), traits.ElementType)).Create(context, targetType, schema);
        default:
          return (MessagePackSerializer<T>) null;
      }
    }

    public static Action<TCollection, TItem> GetAddItem<TCollection, TItem>(Type targetType)
    {
      MethodInfo addMethod = targetType.GetCollectionTraits().AddMethod;
      if (addMethod == (MethodInfo) null)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Reflection based serializer only supports collection types which implement interface to add new item such as '{0}' and '{1}'", (object) typeof (ICollection<>).GetFullName(), (object) typeof (IList)));
      try
      {
        return addMethod.CreateDelegate(typeof (Action<TCollection, TItem>)) as Action<TCollection, TItem>;
      }
      catch (ArgumentException ex)
      {
        return (Action<TCollection, TItem>) ((collection, item) => addMethod.InvokePreservingExceptionType((object) collection, (object) item));
      }
    }

    public static void GetMetadata(
      IList<SerializingMember> members,
      SerializationContext context,
      out Func<object, object>[] getters,
      out Action<object, object>[] setters,
      out MemberInfo[] memberInfos,
      out DataMemberContract[] contracts,
      out IMessagePackSerializer[] serializers)
    {
      getters = new Func<object, object>[members.Count];
      setters = new Action<object, object>[members.Count];
      memberInfos = new MemberInfo[members.Count];
      contracts = new DataMemberContract[members.Count];
      serializers = new IMessagePackSerializer[members.Count];
      for (int index = 0; index < members.Count; ++index)
      {
        SerializingMember member1 = members[index];
        if (!(member1.Member == (MemberInfo) null))
        {
          FieldInfo member2;
          if ((member2 = member1.Member as FieldInfo) != (FieldInfo) null)
          {
            getters[index] = new Func<object, object>(member2.GetValue);
            setters[index] = new Action<object, object>(member2.SetValue);
          }
          else
          {
            PropertyInfo property = member1.Member as PropertyInfo;
            getters[index] = (Func<object, object>) (target => property.GetGetMethod(true).InvokePreservingExceptionType(target, (object[]) null));
            MethodInfo setter = property.GetSetMethod(true);
            if (setter != (MethodInfo) null)
              setters[index] = (Action<object, object>) ((target, value) => setter.InvokePreservingExceptionType(target, value));
          }
          memberInfos[index] = member1.Member;
          contracts[index] = member1.Contract;
          Type memberValueType = member1.Member.GetMemberValueType();
          serializers[index] = !memberValueType.GetIsEnum() ? (!DateTimeMessagePackSerializerHelpers.IsDateTime(memberValueType) ? (IMessagePackSerializer) context.GetSerializer(memberValueType, (object) PolymorphismSchema.Create(memberValueType, new SerializingMember?(member1))) : (IMessagePackSerializer) context.GetSerializer(memberValueType, (object) DateTimeMessagePackSerializerHelpers.DetermineDateTimeConversionMethod(context, member1.GetDateTimeMemberConversionMethod()))) : (IMessagePackSerializer) context.GetSerializer(memberValueType, (object) EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context, memberValueType, member1.GetEnumMemberSerializationMethod()));
        }
      }
    }

    public static Func<int, T> CreateCollectionInstanceFactory<T, TKey>(Type targetType)
    {
      ConstructorInfo constructor = UnpackHelpers.GetCollectionConstructor(targetType);
      ParameterInfo[] parameters = constructor.GetParameters();
      switch (parameters.Length)
      {
        case 0:
          return (Func<int, T>) (_ => (T) constructor.InvokePreservingExceptionType());
        case 1:
          if (parameters[0].ParameterType == typeof (int))
            return (Func<int, T>) (capacity => (T) constructor.InvokePreservingExceptionType((object) capacity));
          if (UnpackHelpers.IsIEqualityComparer(parameters[0].ParameterType))
          {
            EqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;
            return (Func<int, T>) (_ => (T) constructor.InvokePreservingExceptionType((object) comparer));
          }
          break;
        case 2:
          EqualityComparer<TKey> comparer1 = EqualityComparer<TKey>.Default;
          if (parameters[0].ParameterType == typeof (int) && UnpackHelpers.IsIEqualityComparer(parameters[1].ParameterType))
            return (Func<int, T>) (capacity => (T) constructor.InvokePreservingExceptionType((object) capacity, (object) comparer1));
          if (UnpackHelpers.IsIEqualityComparer(parameters[0].ParameterType) && parameters[0].ParameterType == typeof (int))
            return (Func<int, T>) (capacity => (T) constructor.InvokePreservingExceptionType((object) comparer1, (object) capacity));
          break;
      }
      throw SerializationExceptions.NewTargetDoesNotHavePublicDefaultConstructorNorInitialCapacity(typeof (T));
    }

    private interface IVariantReflectionSerializerFactory
    {
      IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema);
    }

    private sealed class NonGenericEnumerableSerializerFactory<T> : ReflectionSerializerHelper.IVariantReflectionSerializerFactory
      where T : IEnumerable
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new ReflectionNonGenericEnumerableMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class NonGenericCollectionSerializerFactory<T> : ReflectionSerializerHelper.IVariantReflectionSerializerFactory
      where T : ICollection
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new ReflectionNonGenericCollectionMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class NonGenericListSerializerFactory<T> : ReflectionSerializerHelper.IVariantReflectionSerializerFactory
      where T : IList
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new ReflectionNonGenericListMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class NonGenericDictionarySerializerFactory<T> : ReflectionSerializerHelper.IVariantReflectionSerializerFactory
      where T : IDictionary
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new ReflectionNonGenericDictionaryMessagePackSerializer<T>(context, targetType, schema);
      }
    }

    private sealed class EnumerableSerializerFactory<TCollection, TItem> : ReflectionSerializerHelper.IVariantReflectionSerializerFactory
      where TCollection : IEnumerable<TItem>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        PolymorphismSchema itemsSchema = schema ?? PolymorphismSchema.Default;
        return (IMessagePackSingleObjectSerializer) new ReflectionEnumerableMessagePackSerializer<TCollection, TItem>(context, targetType, itemsSchema);
      }
    }

    private sealed class CollectionSerializerFactory<TCollection, TItem> : ReflectionSerializerHelper.IVariantReflectionSerializerFactory
      where TCollection : ICollection<TItem>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        PolymorphismSchema itemsSchema = schema ?? PolymorphismSchema.Default;
        return (IMessagePackSingleObjectSerializer) new ReflectionCollectionMessagePackSerializer<TCollection, TItem>(context, targetType, itemsSchema);
      }
    }

    private sealed class DictionarySerializerFactory<TDictionary, TKey, TValue> : ReflectionSerializerHelper.IVariantReflectionSerializerFactory
      where TDictionary : IDictionary<TKey, TValue>
    {
      public IMessagePackSingleObjectSerializer Create(
        SerializationContext context,
        Type targetType,
        PolymorphismSchema schema)
      {
        return (IMessagePackSingleObjectSerializer) new ReflectionDictionaryMessagePackSerializer<TDictionary, TKey, TValue>(context, targetType, schema);
      }
    }
  }
}
