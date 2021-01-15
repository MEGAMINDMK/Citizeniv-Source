// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.MessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.CodeDomSerializers;
using MsgPack.Serialization.DefaultSerializers;
using MsgPack.Serialization.EmittingSerializers;
using MsgPack.Serialization.ExpressionSerializers;
using MsgPack.Serialization.Metadata;
using MsgPack.Serialization.ReflectionSerializers;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace MsgPack.Serialization
{
  public static class MessagePackSerializer
  {
    private static readonly ConcurrentDictionary<Type, Func<SerializationContext, IMessagePackSingleObjectSerializer>> _creatorCache = new ConcurrentDictionary<Type, Func<SerializationContext, IMessagePackSingleObjectSerializer>>();
    private static readonly MessagePackSerializer<MessagePackObject> _singleTonMpoDeserializer = (MessagePackSerializer<MessagePackObject>) new MsgPack_MessagePackObjectMessagePackSerializer(new SerializationContext());

    [Obsolete("Use Get<T>() instead.")]
    public static MessagePackSerializer<T> Create<T>()
    {
      return MessagePackSerializer.Create<T>(SerializationContext.Default);
    }

    [Obsolete("Use Get<T>(SerializationContext) instead.")]
    public static MessagePackSerializer<T> Create<T>(
      SerializationContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return MessagePackSerializer.Get<T>(context, (object) null);
    }

    public static MessagePackSerializer<T> Get<T>()
    {
      return MessagePackSerializer.Get<T>(SerializationContext.Default);
    }

    public static MessagePackSerializer<T> Get<T>(object providerParameter)
    {
      return MessagePackSerializer.Get<T>(SerializationContext.Default, providerParameter);
    }

    public static MessagePackSerializer<T> Get<T>(SerializationContext context)
    {
      return MessagePackSerializer.Get<T>(context, (object) null);
    }

    public static MessagePackSerializer<T> Get<T>(
      SerializationContext context,
      object providerParameter)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return context.GetSerializer<T>(providerParameter);
    }

    internal static MessagePackSerializer<T> CreateInternal<T>(
      SerializationContext context,
      PolymorphismSchema schema)
    {
      Type type = (Type) null;
      if (typeof (T).GetIsAbstract() || typeof (T).GetIsInterface())
      {
        if (typeof (T).GetCollectionTraits().CollectionType != CollectionKind.NotCollection)
          type = context.DefaultCollectionTypes.GetConcreteType(typeof (T));
        if (type == (Type) null)
          return (MessagePackSerializer<T>) null;
        MessagePackSerializer.ValidateType(type);
      }
      else
        MessagePackSerializer.ValidateType(typeof (T));
      ISerializerBuilder<T> serializerBuilder;
      switch (context.EmitterFlavor)
      {
        case EmitterFlavor.ContextBased:
          serializerBuilder = (ISerializerBuilder<T>) new DynamicMethodSerializerBuilder<T>();
          break;
        case EmitterFlavor.FieldBased:
          serializerBuilder = (ISerializerBuilder<T>) new AssemblyBuilderSerializerBuilder<T>();
          break;
        case EmitterFlavor.ExpressionBased:
          serializerBuilder = (ISerializerBuilder<T>) new ExpressionTreeSerializerBuilder<T>();
          break;
        case EmitterFlavor.ReflectionBased:
          if (GenericSerializer.TryCreateAbstractCollectionSerializer(context, typeof (T), type, schema) is MessagePackSerializer<T> collectionSerializer)
            return collectionSerializer;
          SerializationContext context1 = context;
          Type concreteType = type;
          if ((object) concreteType == null)
            concreteType = typeof (T);
          PolymorphismSchema schema1 = schema;
          return MessagePackSerializer.CreateReflectionInternal<T>(context1, concreteType, schema1);
        default:
          if (!SerializerDebugging.OnTheFlyCodeDomEnabled)
            throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Flavor '{0:G}'({0:D}) is not supported for serializer instance creation.", (object) context.EmitterFlavor));
          serializerBuilder = (ISerializerBuilder<T>) new CodeDomSerializerBuilder<T>();
          break;
      }
      return serializerBuilder.BuildSerializerInstance(context, type, schema == null ? (PolymorphismSchema) null : schema.FilterSelf());
    }

    [Obsolete("Use Get(Type) instead.")]
    public static IMessagePackSingleObjectSerializer Create(
      Type targetType)
    {
      return MessagePackSerializer.Create(targetType, SerializationContext.Default);
    }

    [Obsolete("Use Get(Type,SerializationContext) instead.")]
    public static IMessagePackSingleObjectSerializer Create(
      Type targetType,
      SerializationContext context)
    {
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return MessagePackSerializer._creatorCache.GetOrAdd(targetType, (Func<Type, Func<SerializationContext, IMessagePackSingleObjectSerializer>>) (type => Delegate.CreateDelegate(typeof (Func<SerializationContext, IMessagePackSingleObjectSerializer>), _MessagePackSerializer.Create1_Method.MakeGenericMethod(type)) as Func<SerializationContext, IMessagePackSingleObjectSerializer>))(context);
    }

    public static IMessagePackSingleObjectSerializer Get(
      Type targetType)
    {
      return MessagePackSerializer.Get(targetType, SerializationContext.Default, (object) null);
    }

    public static IMessagePackSingleObjectSerializer Get(
      Type targetType,
      object providerParameter)
    {
      return MessagePackSerializer.Get(targetType, SerializationContext.Default, providerParameter);
    }

    public static IMessagePackSingleObjectSerializer Get(
      Type targetType,
      SerializationContext context)
    {
      return MessagePackSerializer.Get(targetType, context, (object) null);
    }

    public static IMessagePackSingleObjectSerializer Get(
      Type targetType,
      SerializationContext context,
      object providerParameter)
    {
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return context.GetSerializer(targetType, providerParameter);
    }

    internal static MessagePackSerializer<T> CreateReflectionInternal<T>(
      SerializationContext context,
      Type concreteType,
      PolymorphismSchema schema)
    {
      if (concreteType.GetIsAbstract() || concreteType.GetIsInterface())
        return (MessagePackSerializer<T>) null;
      MessagePackSerializer<T> messagePackSerializer = context.Serializers.Get<T>(context);
      if (messagePackSerializer != null)
        return messagePackSerializer;
      MessagePackSerializer.ValidateType(typeof (T));
      CollectionTraits collectionTraits = typeof (T).GetCollectionTraits();
      switch (collectionTraits.CollectionType)
      {
        case CollectionKind.Array:
        case CollectionKind.Map:
          return ReflectionSerializerHelper.CreateCollectionSerializer<T>(context, concreteType, collectionTraits, schema ?? PolymorphismSchema.Default);
        default:
          if (typeof (T).GetIsEnum())
            return ReflectionSerializerHelper.CreateReflectionEnumMessagePackSerializer<T>(context);
          return TupleItems.IsTuple(typeof (T)) ? (MessagePackSerializer<T>) new ReflectionTupleMessagePackSerializer<T>(context, (schema ?? PolymorphismSchema.Default).ChildSchemaList) : (MessagePackSerializer<T>) new ReflectionObjectMessagePackSerializer<T>(context);
      }
    }

    private static void ValidateType(Type type)
    {
      if (!type.GetIsVisible())
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Non-public type '{0}' cannot be serialized.", (object) type));
    }

    public static MessagePackObject UnpackMessagePackObject(Stream stream)
    {
      return MessagePackSerializer._singleTonMpoDeserializer.Unpack(stream);
    }

    public static void PrepareType<T>()
    {
    }
  }
}
