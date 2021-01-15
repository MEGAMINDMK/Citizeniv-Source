// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.ArraySerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal static class ArraySerializer
  {
    private static readonly Dictionary<Type, Func<SerializationContext, object>> _arraySerializerFactories = ArraySerializer.InitializeArraySerializerFactories();

    public static MessagePackSerializer<T> Create<T>(
      SerializationContext context,
      PolymorphismSchema itemsSchema)
    {
      return ArraySerializer.Create(context, typeof (T), itemsSchema) as MessagePackSerializer<T>;
    }

    public static IMessagePackSingleObjectSerializer Create(
      SerializationContext context,
      Type targetType,
      PolymorphismSchema itemsSchema)
    {
      if (targetType.GetElementType().MakeArrayType().IsAssignableFrom(targetType))
      {
        object obj = ArraySerializer.GetPrimitiveArraySerializer(context, targetType);
        if (obj == null)
          obj = ReflectionExtensions.CreateInstancePreservingExceptionType(typeof (ArraySerializer<>).MakeGenericType(targetType.GetElementType()), (object) context, (object) itemsSchema);
        return obj as IMessagePackSingleObjectSerializer;
      }
      return ReflectionExtensions.CreateInstancePreservingExceptionType(typeof (MultidimensionalArraySerializer<,>).MakeGenericType(targetType, targetType.GetElementType()), (object) context, (object) itemsSchema) as IMessagePackSingleObjectSerializer;
    }

    private static object GetPrimitiveArraySerializer(SerializationContext context, Type targetType)
    {
      Func<SerializationContext, object> func;
      return !ArraySerializer._arraySerializerFactories.TryGetValue(targetType, out func) ? (object) null : func(context);
    }

    private static Dictionary<Type, Func<SerializationContext, object>> InitializeArraySerializerFactories()
    {
      return new Dictionary<Type, Func<SerializationContext, object>>(25)
      {
        {
          typeof (sbyte[]),
          (Func<SerializationContext, object>) (context => (object) new SByteArraySerializer(context))
        },
        {
          typeof (sbyte?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableSByteArraySerializer(context))
        },
        {
          typeof (short[]),
          (Func<SerializationContext, object>) (context => (object) new Int16ArraySerializer(context))
        },
        {
          typeof (short?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableInt16ArraySerializer(context))
        },
        {
          typeof (int[]),
          (Func<SerializationContext, object>) (context => (object) new Int32ArraySerializer(context))
        },
        {
          typeof (int?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableInt32ArraySerializer(context))
        },
        {
          typeof (long[]),
          (Func<SerializationContext, object>) (context => (object) new Int64ArraySerializer(context))
        },
        {
          typeof (long?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableInt64ArraySerializer(context))
        },
        {
          typeof (byte[]),
          (Func<SerializationContext, object>) (context => (object) new ByteArraySerializer(context))
        },
        {
          typeof (byte?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableByteArraySerializer(context))
        },
        {
          typeof (ushort[]),
          (Func<SerializationContext, object>) (context => (object) new UInt16ArraySerializer(context))
        },
        {
          typeof (ushort?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableUInt16ArraySerializer(context))
        },
        {
          typeof (uint[]),
          (Func<SerializationContext, object>) (context => (object) new UInt32ArraySerializer(context))
        },
        {
          typeof (uint?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableUInt32ArraySerializer(context))
        },
        {
          typeof (ulong[]),
          (Func<SerializationContext, object>) (context => (object) new UInt64ArraySerializer(context))
        },
        {
          typeof (ulong?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableUInt64ArraySerializer(context))
        },
        {
          typeof (float[]),
          (Func<SerializationContext, object>) (context => (object) new SingleArraySerializer(context))
        },
        {
          typeof (float?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableSingleArraySerializer(context))
        },
        {
          typeof (double[]),
          (Func<SerializationContext, object>) (context => (object) new DoubleArraySerializer(context))
        },
        {
          typeof (double?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableDoubleArraySerializer(context))
        },
        {
          typeof (bool[]),
          (Func<SerializationContext, object>) (context => (object) new BooleanArraySerializer(context))
        },
        {
          typeof (bool?[]),
          (Func<SerializationContext, object>) (context => (object) new NullableBooleanArraySerializer(context))
        },
        {
          typeof (string[]),
          (Func<SerializationContext, object>) (context => (object) new StringArraySerializer(context))
        },
        {
          typeof (byte[][]),
          (Func<SerializationContext, object>) (context => (object) new BinaryArraySerializer(context))
        },
        {
          typeof (MessagePackObject[]),
          (Func<SerializationContext, object>) (context => (object) new MessagePackObjectArraySerializer(context))
        }
      };
    }
  }
}
