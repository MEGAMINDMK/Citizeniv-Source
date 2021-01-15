// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.UnpackHelpers
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.DefaultSerializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace MsgPack.Serialization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class UnpackHelpers
  {
    private static readonly MessagePackSerializer<MessagePackObject> _messagePackObjectSerializer = (MessagePackSerializer<MessagePackObject>) new MsgPack_MessagePackObjectMessagePackSerializer(SerializationContext.Default);

    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UnpackArrayTo<T>(
      Unpacker unpacker,
      MessagePackSerializer<T> serializer,
      T[] array)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        T obj;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          obj = serializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            obj = serializer.UnpackFrom(unpacker1);
        }
        array[index] = obj;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    public static void UnpackCollectionTo(
      Unpacker unpacker,
      IEnumerable collection,
      Action<object> addition)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (addition == null)
        throw new ArgumentNullException(nameof (addition));
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        MessagePackObject messagePackObject;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          messagePackObject = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            messagePackObject = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker1);
        }
        addition((object) messagePackObject);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    public static void UnpackCollectionTo<T>(
      Unpacker unpacker,
      MessagePackSerializer<T> serializer,
      IEnumerable<T> collection,
      Action<T> addition)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (addition == null)
        throw new ArgumentNullException(nameof (addition));
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        T obj;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          obj = serializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            obj = serializer.UnpackFrom(unpacker1);
        }
        addition(obj);
      }
    }

    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UnpackCollectionTo<TDiscarded>(
      Unpacker unpacker,
      IEnumerable collection,
      Func<object, TDiscarded> addition)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (addition == null)
        throw new ArgumentNullException(nameof (addition));
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        MessagePackObject messagePackObject;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          messagePackObject = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            messagePackObject = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker1);
        }
        TDiscarded discarded = addition((object) messagePackObject);
      }
    }

    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UnpackCollectionTo<T, TDiscarded>(
      Unpacker unpacker,
      MessagePackSerializer<T> serializer,
      IEnumerable<T> collection,
      Func<T, TDiscarded> addition)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (addition == null)
        throw new ArgumentNullException(nameof (addition));
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        T obj;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          obj = serializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            obj = serializer.UnpackFrom(unpacker1);
        }
        TDiscarded discarded = addition(obj);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    public static void UnpackMapTo<TKey, TValue>(
      Unpacker unpacker,
      MessagePackSerializer<TKey> keySerializer,
      MessagePackSerializer<TValue> valueSerializer,
      IDictionary<TKey, TValue> dictionary)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (keySerializer == null)
        throw new ArgumentNullException(nameof (keySerializer));
      if (valueSerializer == null)
        throw new ArgumentNullException(nameof (valueSerializer));
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        TKey key;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          key = keySerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            key = keySerializer.UnpackFrom(unpacker1);
        }
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        TValue obj;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          obj = valueSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            obj = valueSerializer.UnpackFrom(unpacker1);
        }
        dictionary.Add(key, obj);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    public static void UnpackMapTo(Unpacker unpacker, IDictionary dictionary)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      for (int index = 0; index < itemsCount; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        MessagePackObject messagePackObject1;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          messagePackObject1 = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            messagePackObject1 = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker1);
        }
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        MessagePackObject messagePackObject2;
        if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        {
          messagePackObject2 = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker);
        }
        else
        {
          using (Unpacker unpacker1 = unpacker.ReadSubtree())
            messagePackObject2 = UnpackHelpers._messagePackObjectSerializer.UnpackFrom(unpacker1);
        }
        dictionary.Add((object) messagePackObject1, (object) messagePackObject2);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int GetItemsCount(Unpacker unpacker)
    {
      if (unpacker == null)
        throw new ArgumentNullException(nameof (unpacker));
      long itemsCount;
      try
      {
        itemsCount = unpacker.ItemsCount;
      }
      catch (InvalidOperationException ex)
      {
        throw SerializationExceptions.NewIsIncorrectStream((Exception) ex);
      }
      if (itemsCount > (long) int.MaxValue)
        throw SerializationExceptions.NewIsTooLargeCollection();
      return (int) itemsCount;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    public static T ConvertWithEnsuringNotNull<T>(object boxed, string name, Type targetType)
    {
      if (typeof (T).GetIsValueType() && boxed == null && Nullable.GetUnderlyingType(typeof (T)) == (Type) null)
        throw SerializationExceptions.NewValueTypeCannotBeNull(name, typeof (T), targetType);
      return (T) boxed;
    }

    [Obsolete("This API is not used at generated serializers in current release, so this API will be removed future.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T InvokeUnpackFrom<T>(MessagePackSerializer<T> serializer, Unpacker unpacker)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      return serializer.UnpackFromCore(unpacker);
    }

    internal static ConstructorInfo GetCollectionConstructor(Type instanceType)
    {
      ConstructorInfo constructorInfo = (ConstructorInfo) null;
      int num = -1;
      foreach (ConstructorInfo constructor in instanceType.GetConstructors())
      {
        ParameterInfo[] parameters = constructor.GetParameters();
        switch (parameters.Length)
        {
          case 0:
            if (num < 0)
            {
              constructorInfo = constructor;
              num = 0;
              break;
            }
            break;
          case 1:
            if (num < 10 && parameters[0].ParameterType == typeof (int))
            {
              constructorInfo = constructor;
              num = 0;
              break;
            }
            if (num < 11 && UnpackHelpers.IsIEqualityComparer(parameters[0].ParameterType))
            {
              constructorInfo = constructor;
              num = 0;
              break;
            }
            break;
          case 2:
            if (num < 21 && parameters[0].ParameterType == typeof (int) && UnpackHelpers.IsIEqualityComparer(parameters[1].ParameterType))
            {
              constructorInfo = constructor;
              num = 21;
              break;
            }
            if (num < 20 && parameters[1].ParameterType == typeof (int) && UnpackHelpers.IsIEqualityComparer(parameters[0].ParameterType))
            {
              constructorInfo = constructor;
              num = 20;
              break;
            }
            break;
        }
      }
      if (constructorInfo == (ConstructorInfo) null)
        throw SerializationExceptions.NewTargetDoesNotHavePublicDefaultConstructorNorInitialCapacity(instanceType);
      return constructorInfo;
    }

    internal static bool IsIEqualityComparer(Type type)
    {
      return type.GetIsGenericType() && type.GetGenericTypeDefinition() == typeof (IEqualityComparer<>);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEqualityComparer<T> GetEqualityComparer<T>()
    {
      return (IEqualityComparer<T>) EqualityComparer<T>.Default;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool UnpackBooleanValue(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        bool result;
        if (!unpacker.ReadBoolean(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool? UnpackNullableBooleanValue(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        bool? result;
        if (!unpacker.ReadNullableBoolean(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static byte UnpackByteValue(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        byte result;
        if (!unpacker.ReadByte(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static byte? UnpackNullableByteValue(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        byte? result;
        if (!unpacker.ReadNullableByte(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static short UnpackInt16Value(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        short result;
        if (!unpacker.ReadInt16(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static short? UnpackNullableInt16Value(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        short? result;
        if (!unpacker.ReadNullableInt16(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int UnpackInt32Value(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        int result;
        if (!unpacker.ReadInt32(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int? UnpackNullableInt32Value(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        int? result;
        if (!unpacker.ReadNullableInt32(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static long UnpackInt64Value(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        long result;
        if (!unpacker.ReadInt64(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static long? UnpackNullableInt64Value(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        long? result;
        if (!unpacker.ReadNullableInt64(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [CLSCompliant(false)]
    public static sbyte UnpackSByteValue(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        sbyte result;
        if (!unpacker.ReadSByte(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static sbyte? UnpackNullableSByteValue(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        sbyte? result;
        if (!unpacker.ReadNullableSByte(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ushort UnpackUInt16Value(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        ushort result;
        if (!unpacker.ReadUInt16(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ushort? UnpackNullableUInt16Value(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        ushort? result;
        if (!unpacker.ReadNullableUInt16(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static uint UnpackUInt32Value(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        uint result;
        if (!unpacker.ReadUInt32(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [CLSCompliant(false)]
    public static uint? UnpackNullableUInt32Value(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        uint? result;
        if (!unpacker.ReadNullableUInt32(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ulong UnpackUInt64Value(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        ulong result;
        if (!unpacker.ReadUInt64(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [CLSCompliant(false)]
    public static ulong? UnpackNullableUInt64Value(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        ulong? result;
        if (!unpacker.ReadNullableUInt64(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static float UnpackSingleValue(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        float result;
        if (!unpacker.ReadSingle(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static float? UnpackNullableSingleValue(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        float? result;
        if (!unpacker.ReadNullableSingle(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static double UnpackDoubleValue(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        double result;
        if (!unpacker.ReadDouble(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static double? UnpackNullableDoubleValue(
      Unpacker unpacker,
      Type objectType,
      string memberName)
    {
      try
      {
        double? result;
        if (!unpacker.ReadNullableDouble(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string UnpackStringValue(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        string result;
        if (!unpacker.ReadString(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static byte[] UnpackBinaryValue(Unpacker unpacker, Type objectType, string memberName)
    {
      try
      {
        byte[] result;
        if (!unpacker.ReadBinary(out result))
          throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) null);
        return result;
      }
      catch (MessageTypeException ex)
      {
        throw SerializationExceptions.NewFailedToDeserializeMember(objectType, memberName, (Exception) ex);
      }
    }
  }
}
