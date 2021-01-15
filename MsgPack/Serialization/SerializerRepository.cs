// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializerRepository
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.DefaultSerializers;
using MsgPack.Serialization.Polymorphic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MsgPack.Serialization
{
  public sealed class SerializerRepository : IDisposable
  {
    private static readonly object SyncRoot = new object();
    internal const int DefaultTableCapacity = 451;
    private static SerializerRepository _internalDefault;
    private readonly SerializerTypeKeyRepository _repository;

    internal static Dictionary<RuntimeTypeHandle, object> InitializeDefaultTable(
      SerializationContext ownerContext)
    {
      return new Dictionary<RuntimeTypeHandle, object>(451)
      {
        {
          typeof (MessagePackObject).TypeHandle,
          (object) new MsgPack_MessagePackObjectMessagePackSerializer(ownerContext)
        },
        {
          typeof (MessagePackObjectDictionary).TypeHandle,
          (object) new MsgPack_MessagePackObjectDictionaryMessagePackSerializer(ownerContext)
        },
        {
          typeof (MessagePackExtendedTypeObject).TypeHandle,
          (object) new MsgPack_MessagePackExtendedTypeObjectMessagePackSerializer(ownerContext)
        },
        {
          typeof (List<MessagePackObject>).TypeHandle,
          (object) new System_Collections_Generic_ListOfMessagePackObjectMessagePackSerializer(ownerContext)
        },
        {
          typeof (object).TypeHandle,
          (object) new PolymorphicSerializerProvider<object>((MessagePackSerializer<object>) new System_ObjectMessagePackSerializer(ownerContext))
        },
        {
          typeof (string).TypeHandle,
          (object) new System_StringMessagePackSerializer(ownerContext)
        },
        {
          typeof (StringBuilder).TypeHandle,
          (object) new System_Text_StringBuilderMessagePackSerializer(ownerContext)
        },
        {
          typeof (char[]).TypeHandle,
          (object) new System_CharArrayMessagePackSerializer(ownerContext)
        },
        {
          typeof (byte[]).TypeHandle,
          (object) new System_ByteArrayMessagePackSerializer(ownerContext)
        },
        {
          typeof (DateTime).TypeHandle,
          (object) new DateTimeMessagePackSerializerProvider(ownerContext, false)
        },
        {
          typeof (DateTimeOffset).TypeHandle,
          (object) new DateTimeOffsetMessagePackSerializerProvider(ownerContext, false)
        },
        {
          typeof (FILETIME).TypeHandle,
          (object) new FileTimeMessagePackSerializerProvider(ownerContext, false)
        },
        {
          typeof (DateTime?).TypeHandle,
          (object) new DateTimeMessagePackSerializerProvider(ownerContext, true)
        },
        {
          typeof (DateTimeOffset?).TypeHandle,
          (object) new DateTimeOffsetMessagePackSerializerProvider(ownerContext, true)
        },
        {
          typeof (FILETIME?).TypeHandle,
          (object) new FileTimeMessagePackSerializerProvider(ownerContext, true)
        },
        {
          typeof (DBNull).TypeHandle,
          (object) new System_DBNullMessagePackSerializer(ownerContext)
        },
        {
          typeof (bool).TypeHandle,
          (object) new System_BooleanMessagePackSerializer(ownerContext)
        },
        {
          typeof (byte).TypeHandle,
          (object) new System_ByteMessagePackSerializer(ownerContext)
        },
        {
          typeof (char).TypeHandle,
          (object) new System_CharMessagePackSerializer(ownerContext)
        },
        {
          typeof (Decimal).TypeHandle,
          (object) new System_DecimalMessagePackSerializer(ownerContext)
        },
        {
          typeof (double).TypeHandle,
          (object) new System_DoubleMessagePackSerializer(ownerContext)
        },
        {
          typeof (Guid).TypeHandle,
          (object) new System_GuidMessagePackSerializer(ownerContext)
        },
        {
          typeof (short).TypeHandle,
          (object) new System_Int16MessagePackSerializer(ownerContext)
        },
        {
          typeof (int).TypeHandle,
          (object) new System_Int32MessagePackSerializer(ownerContext)
        },
        {
          typeof (long).TypeHandle,
          (object) new System_Int64MessagePackSerializer(ownerContext)
        },
        {
          typeof (sbyte).TypeHandle,
          (object) new System_SByteMessagePackSerializer(ownerContext)
        },
        {
          typeof (float).TypeHandle,
          (object) new System_SingleMessagePackSerializer(ownerContext)
        },
        {
          typeof (TimeSpan).TypeHandle,
          (object) new System_TimeSpanMessagePackSerializer(ownerContext)
        },
        {
          typeof (ushort).TypeHandle,
          (object) new System_UInt16MessagePackSerializer(ownerContext)
        },
        {
          typeof (uint).TypeHandle,
          (object) new System_UInt32MessagePackSerializer(ownerContext)
        },
        {
          typeof (ulong).TypeHandle,
          (object) new System_UInt64MessagePackSerializer(ownerContext)
        },
        {
          typeof (BitVector32).TypeHandle,
          (object) new System_Collections_Specialized_BitVector32MessagePackSerializer(ownerContext)
        },
        {
          typeof (BigInteger).TypeHandle,
          (object) new System_Numerics_BigIntegerMessagePackSerializer(ownerContext)
        },
        {
          typeof (ArraySegment<>).TypeHandle,
          (object) typeof (System_ArraySegment_1MessagePackSerializer<>)
        },
        {
          typeof (CultureInfo).TypeHandle,
          (object) new System_Globalization_CultureInfoMessagePackSerializer(ownerContext)
        },
        {
          typeof (DictionaryEntry).TypeHandle,
          (object) new System_Collections_DictionaryEntryMessagePackSerializer(ownerContext)
        },
        {
          typeof (Stack).TypeHandle,
          (object) new System_Collections_StackMessagePackSerializer(ownerContext)
        },
        {
          typeof (Queue).TypeHandle,
          (object) new System_Collections_QueueMessagePackSerializer(ownerContext)
        },
        {
          typeof (KeyValuePair<,>).TypeHandle,
          (object) typeof (System_Collections_Generic_KeyValuePair_2MessagePackSerializer<,>)
        },
        {
          typeof (Stack<>).TypeHandle,
          (object) typeof (System_Collections_Generic_Stack_1MessagePackSerializer<>)
        },
        {
          typeof (Queue<>).TypeHandle,
          (object) typeof (System_Collections_Generic_Queue_1MessagePackSerializer<>)
        },
        {
          typeof (Complex).TypeHandle,
          (object) new System_Numerics_ComplexMessagePackSerializer(ownerContext)
        },
        {
          typeof (Uri).TypeHandle,
          (object) new System_UriMessagePackSerializer(ownerContext)
        },
        {
          typeof (Version).TypeHandle,
          (object) new System_VersionMessagePackSerializer(ownerContext)
        },
        {
          typeof (NameValueCollection).TypeHandle,
          (object) new System_Collections_Specialized_NameValueCollectionMessagePackSerializer(ownerContext)
        }
      };
    }

    internal static SerializerRepository InternalDefault
    {
      get
      {
        lock (SerializerRepository.SyncRoot)
        {
          if (SerializerRepository._internalDefault == null)
            SerializerRepository._internalDefault = SerializerRepository.GetDefault(SerializationContext.Default);
          return SerializerRepository._internalDefault;
        }
      }
    }

    public SerializerRepository()
    {
      this._repository = new SerializerTypeKeyRepository();
    }

    public SerializerRepository(SerializerRepository copiedFrom)
    {
      if (copiedFrom == null)
        throw new ArgumentNullException(nameof (copiedFrom));
      this._repository = new SerializerTypeKeyRepository(copiedFrom._repository);
    }

    private SerializerRepository(Dictionary<RuntimeTypeHandle, object> table)
    {
      this._repository = new SerializerTypeKeyRepository(table);
    }

    [Obsolete("This class should not be disposable, so IDisposable will be removed in future.")]
    public void Dispose()
    {
    }

    public MessagePackSerializer<T> Get<T>(SerializationContext context)
    {
      return this.Get<T>(context, (object) null);
    }

    public MessagePackSerializer<T> Get<T>(
      SerializationContext context,
      object providerParameter)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      object obj = this._repository.Get(context, typeof (T));
      return (obj is MessagePackSerializerProvider serializerProvider ? serializerProvider.Get(context, providerParameter) : obj) as MessagePackSerializer<T>;
    }

    public bool Register<T>(MessagePackSerializer<T> serializer)
    {
      return this.Register<T>(serializer, SerializerRegistrationOptions.None);
    }

    public bool Register<T>(
      MessagePackSerializer<T> serializer,
      SerializerRegistrationOptions options)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      Type nullableType = (Type) null;
      MessagePackSerializerProvider nullableSerializerProvider = (MessagePackSerializerProvider) null;
      if ((options & SerializerRegistrationOptions.WithNullable) != SerializerRegistrationOptions.None)
        SerializerRepository.GetNullableCompanion(typeof (T), serializer.OwnerContext, (object) serializer, out nullableType, out nullableSerializerProvider);
      return this.Register(typeof (T), (MessagePackSerializerProvider) new PolymorphicSerializerProvider<T>(serializer), nullableType, nullableSerializerProvider, options);
    }

    internal static void GetNullableCompanion(
      Type targetType,
      SerializationContext context,
      object serializer,
      out Type nullableType,
      out MessagePackSerializerProvider nullableSerializerProvider)
    {
      if (targetType.GetIsValueType() && Nullable.GetUnderlyingType(targetType) == (Type) null)
      {
        nullableType = typeof (Nullable<>).MakeGenericType(targetType);
        ConstructorInfo constructor = typeof (NullableMessagePackSerializer<>).MakeGenericType(targetType).GetConstructor(new Type[2]
        {
          typeof (SerializationContext),
          typeof (MessagePackSerializer<>).MakeGenericType(targetType)
        });
        nullableSerializerProvider = (MessagePackSerializerProvider) ReflectionExtensions.CreateInstancePreservingExceptionType(typeof (PolymorphicSerializerProvider<>).MakeGenericType(nullableType), constructor.InvokePreservingExceptionType((object) context, serializer));
      }
      else
      {
        nullableType = (Type) null;
        nullableSerializerProvider = (MessagePackSerializerProvider) null;
      }
    }

    internal bool Register(
      Type targetType,
      MessagePackSerializerProvider serializerProvider,
      Type nullableType,
      MessagePackSerializerProvider nullableSerializerProvider,
      SerializerRegistrationOptions options)
    {
      return this._repository.Register(targetType, (object) serializerProvider, nullableType, (object) nullableSerializerProvider, options);
    }

    public void RegisterOverride<T>(MessagePackSerializer<T> serializer)
    {
      this.Register<T>(serializer, SerializerRegistrationOptions.AllowOverride);
    }

    [Obsolete("Use GetDefault()")]
    public static SerializerRepository Default
    {
      get
      {
        return SerializerRepository.GetDefault(SerializationContext.Default);
      }
    }

    public static SerializerRepository GetDefault()
    {
      return SerializerRepository.GetDefault(SerializationContext.Default);
    }

    [Obsolete("Use GetDefault()")]
    public static SerializerRepository GetDefault(
      PackerCompatibilityOptions packerCompatibilityOptions)
    {
      return SerializerRepository.GetDefault(SerializationContext.Default);
    }

    public static SerializerRepository GetDefault(
      SerializationContext ownerContext)
    {
      if (ownerContext == null)
        throw new ArgumentNullException(nameof (ownerContext));
      return new SerializerRepository(SerializerRepository.InitializeDefaultTable(ownerContext));
    }

    internal bool Contains(Type rootType)
    {
      return this._repository.Contains(rootType);
    }
  }
}
