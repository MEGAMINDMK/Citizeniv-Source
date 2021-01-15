// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializationContext
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.DefaultSerializers;
using MsgPack.Serialization.Metadata;
using MsgPack.Serialization.Polymorphic;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MsgPack.Serialization
{
  public sealed class SerializationContext
  {
    private static SerializationContext _default = new SerializationContext(PackerCompatibilityOptions.None);
    private readonly ExtTypeCodeMapping _extTypeCodes = new ExtTypeCodeMapping();
    private EmitterFlavor _emitterFlavor = EmitterFlavor.FieldBased;
    private readonly SerializerRepository _serializers;
    private readonly ConcurrentDictionary<Type, object> _typeLock;
    private readonly SerializationCompatibilityOptions _compatibilityOptions;
    private SerializationMethod _serializationMethod;
    private EnumSerializationMethod _enumSerializationMethod;
    private SerializationMethodGeneratorOption _generatorOption;
    private readonly DefaultConcreteTypeRepository _defaultCollectionTypes;
    private EventHandler<ResolveSerializerEventArgs> _resolveSerializer;

    public ExtTypeCodeMapping ExtTypeCodeMapping
    {
      get
      {
        return this._extTypeCodes;
      }
    }

    public static SerializationContext Default
    {
      get
      {
        return Interlocked.CompareExchange<SerializationContext>(ref SerializationContext._default, (SerializationContext) null, (SerializationContext) null);
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        Interlocked.Exchange<SerializationContext>(ref SerializationContext._default, value);
      }
    }

    public SerializerRepository Serializers
    {
      get
      {
        return this._serializers;
      }
    }

    internal EmitterFlavor EmitterFlavor
    {
      get
      {
        return this._emitterFlavor;
      }
      set
      {
        this._emitterFlavor = value;
      }
    }

    public SerializationCompatibilityOptions CompatibilityOptions
    {
      get
      {
        return this._compatibilityOptions;
      }
    }

    public SerializationMethod SerializationMethod
    {
      get
      {
        return this._serializationMethod;
      }
      set
      {
        switch (value)
        {
          case SerializationMethod.Array:
          case SerializationMethod.Map:
            this._serializationMethod = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (value));
        }
      }
    }

    public EnumSerializationMethod EnumSerializationMethod
    {
      get
      {
        return this._enumSerializationMethod;
      }
      set
      {
        switch (value)
        {
          case EnumSerializationMethod.ByName:
          case EnumSerializationMethod.ByUnderlyingValue:
            this._enumSerializationMethod = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (value));
        }
      }
    }

    public SerializationMethodGeneratorOption GeneratorOption
    {
      get
      {
        return this._generatorOption;
      }
      set
      {
        switch (value)
        {
          case SerializationMethodGeneratorOption.CanDump:
          case SerializationMethodGeneratorOption.CanCollect:
          case SerializationMethodGeneratorOption.Fast:
            this._generatorOption = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (value));
        }
      }
    }

    public DefaultConcreteTypeRepository DefaultCollectionTypes
    {
      get
      {
        return this._defaultCollectionTypes;
      }
    }

    internal bool IsRuntimeGenerationDisabled { get; set; }

    public DateTimeConversionMethod DefaultDateTimeConversionMethod { get; set; }

    public event EventHandler<ResolveSerializerEventArgs> ResolveSerializer
    {
      add
      {
        EventHandler<ResolveSerializerEventArgs> eventHandler = this._resolveSerializer;
        EventHandler<ResolveSerializerEventArgs> comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler<ResolveSerializerEventArgs>>(ref this._resolveSerializer, comparand + value, comparand);
        }
        while (comparand != eventHandler);
      }
      remove
      {
        EventHandler<ResolveSerializerEventArgs> eventHandler = this._resolveSerializer;
        EventHandler<ResolveSerializerEventArgs> comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler<ResolveSerializerEventArgs>>(ref this._resolveSerializer, comparand - value, comparand);
        }
        while (comparand != eventHandler);
      }
    }

    private MessagePackSerializer<T> OnResolveSerializer<T>(
      PolymorphismSchema schema)
    {
      EventHandler<ResolveSerializerEventArgs> eventHandler = Interlocked.CompareExchange<EventHandler<ResolveSerializerEventArgs>>(ref this._resolveSerializer, (EventHandler<ResolveSerializerEventArgs>) null, (EventHandler<ResolveSerializerEventArgs>) null);
      if (eventHandler == null)
        return (MessagePackSerializer<T>) null;
      ResolveSerializerEventArgs e = new ResolveSerializerEventArgs(this, typeof (T), schema);
      eventHandler((object) this, e);
      return e.GetFoundSerializer<T>();
    }

    public static SerializationContext ConfigureClassic()
    {
      return Interlocked.Exchange<SerializationContext>(ref SerializationContext._default, SerializationContext.CreateClassicContext());
    }

    public static SerializationContext CreateClassicContext()
    {
      return new SerializationContext(PackerCompatibilityOptions.Classic)
      {
        DefaultDateTimeConversionMethod = DateTimeConversionMethod.UnixEpoc
      };
    }

    public SerializationContext()
      : this(PackerCompatibilityOptions.None)
    {
    }

    public SerializationContext(
      PackerCompatibilityOptions packerCompatibilityOptions)
    {
      this._compatibilityOptions = new SerializationCompatibilityOptions()
      {
        PackerCompatibilityOptions = packerCompatibilityOptions
      };
      this._serializers = new SerializerRepository(SerializerRepository.GetDefault(this));
      this._typeLock = new ConcurrentDictionary<Type, object>();
      this._defaultCollectionTypes = new DefaultConcreteTypeRepository();
      this._generatorOption = SerializationMethodGeneratorOption.Fast;
    }

    internal bool ContainsSerializer(Type rootType)
    {
      if (this._serializers.Contains(rootType))
        return true;
      return rootType.GetIsGenericType() && this._serializers.Contains(rootType.GetGenericTypeDefinition());
    }

    public MessagePackSerializer<T> GetSerializer<T>()
    {
      return this.GetSerializer<T>((object) null);
    }

    public MessagePackSerializer<T> GetSerializer<T>(object providerParameter)
    {
      PolymorphismSchema schema = providerParameter as PolymorphismSchema;
      MessagePackSerializer<T> messagePackSerializer = this._serializers.Get<T>(this, providerParameter);
      if (messagePackSerializer != null)
        return messagePackSerializer;
      object obj = (object) null;
      bool flag = false;
      try
      {
        try
        {
        }
        finally
        {
          object newLock = new object();
          bool lockTaken = false;
          try
          {
            Monitor.Enter(newLock, ref lockTaken);
            obj = this._typeLock.GetOrAdd(typeof (T), (Func<Type, object>) (_ => newLock));
            flag = newLock == obj;
          }
          finally
          {
            if (!flag && lockTaken)
              Monitor.Exit(newLock);
          }
        }
        if (Monitor.TryEnter(obj))
        {
          Monitor.Exit(obj);
          if (flag)
          {
            MessagePackSerializer<T> defaultSerializer = GenericSerializer.Create<T>(this, schema) ?? (!this.IsRuntimeGenerationDisabled ? this.OnResolveSerializer<T>(schema) ?? MessagePackSerializer.CreateInternal<T>(this, schema) : this.GetSerializerWithoutGeneration<T>(schema) ?? this.OnResolveSerializer<T>(schema) ?? MessagePackSerializer.CreateReflectionInternal<T>(this, this.EnsureConcreteTypeRegistered(typeof (T)), schema));
            MessagePackSerializerProvider serializerProvider = !(defaultSerializer is ICustomizableEnumSerializer serializer) ? (MessagePackSerializerProvider) new PolymorphicSerializerProvider<T>(defaultSerializer) : (MessagePackSerializerProvider) new EnumMessagePackSerializerProvider(typeof (T), serializer);
            Type nullableType;
            MessagePackSerializerProvider nullableSerializerProvider;
            SerializerRepository.GetNullableCompanion(typeof (T), this, (object) defaultSerializer, out nullableType, out nullableSerializerProvider);
            this._serializers.Register(typeof (T), serializerProvider, nullableType, nullableSerializerProvider, SerializerRegistrationOptions.WithNullable);
          }
          else
          {
            obj = (object) null;
            return (MessagePackSerializer<T>) new LazyDelegatingMessagePackSerializer<T>(this, providerParameter);
          }
        }
        else
          Monitor.Enter(obj);
        return this._serializers.Get<T>(this, providerParameter);
      }
      finally
      {
        if (flag)
          this._typeLock.TryRemove(typeof (T), out object _);
        if (obj != null)
          Monitor.Exit(obj);
      }
    }

    private Type EnsureConcreteTypeRegistered(Type mayBeAbstractType)
    {
      if (!mayBeAbstractType.GetIsAbstract() && !mayBeAbstractType.GetIsInterface())
        return mayBeAbstractType;
      Type concreteType = this.DefaultCollectionTypes.GetConcreteType(mayBeAbstractType);
      if (concreteType == (Type) null)
        throw SerializationExceptions.NewNotSupportedBecauseCannotInstanciateAbstractType(mayBeAbstractType);
      return concreteType;
    }

    private MessagePackSerializer<T> GetSerializerWithoutGeneration<T>(
      PolymorphismSchema schema)
    {
      if (!typeof (T).GetIsInterface() && !typeof (T).GetIsAbstract())
        return (MessagePackSerializer<T>) null;
      Type concreteType = this._defaultCollectionTypes.GetConcreteType(typeof (T));
      PolymorphicSerializerProvider<T> serializerProvider;
      if (concreteType != (Type) null)
      {
        IMessagePackSingleObjectSerializer collectionSerializer = GenericSerializer.TryCreateAbstractCollectionSerializer(this, typeof (T), concreteType, schema);
        serializerProvider = collectionSerializer == null ? new PolymorphicSerializerProvider<T>((MessagePackSerializer<T>) null) : new PolymorphicSerializerProvider<T>(collectionSerializer as MessagePackSerializer<T>);
      }
      else
        serializerProvider = new PolymorphicSerializerProvider<T>((MessagePackSerializer<T>) null);
      this.Serializers.Register(typeof (T), (MessagePackSerializerProvider) serializerProvider, (Type) null, (MessagePackSerializerProvider) null, SerializerRegistrationOptions.None);
      return (MessagePackSerializer<T>) serializerProvider.Get(this, (object) schema);
    }

    public IMessagePackSingleObjectSerializer GetSerializer(
      Type targetType)
    {
      return this.GetSerializer(targetType, (object) null);
    }

    public IMessagePackSingleObjectSerializer GetSerializer(
      Type targetType,
      object providerParameter)
    {
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      return SerializationContext.SerializerGetter.Instance.Get(this, targetType, providerParameter);
    }

    private sealed class SerializerGetter
    {
      public static readonly SerializationContext.SerializerGetter Instance = new SerializationContext.SerializerGetter();
      private readonly ConcurrentDictionary<RuntimeTypeHandle, Func<SerializationContext, object, IMessagePackSingleObjectSerializer>> _cache = new ConcurrentDictionary<RuntimeTypeHandle, Func<SerializationContext, object, IMessagePackSingleObjectSerializer>>();

      private SerializerGetter()
      {
      }

      public IMessagePackSingleObjectSerializer Get(
        SerializationContext context,
        Type targetType,
        object providerParameter)
      {
        Func<SerializationContext, object, IMessagePackSingleObjectSerializer> func;
        if (!this._cache.TryGetValue(targetType.TypeHandle, out func) || func == null)
        {
          func = Delegate.CreateDelegate(typeof (Func<SerializationContext, object, IMessagePackSingleObjectSerializer>), typeof (SerializationContext.SerializerGetter<>).MakeGenericType(targetType).GetMethod(nameof (Get))) as Func<SerializationContext, object, IMessagePackSingleObjectSerializer>;
          this._cache[targetType.TypeHandle] = func;
        }
        return func(context, providerParameter);
      }
    }

    private static class SerializerGetter<T>
    {
      private static readonly Func<SerializationContext, object, MessagePackSerializer<T>> _func = Delegate.CreateDelegate(typeof (Func<SerializationContext, object, MessagePackSerializer<T>>), _SerializationContext.GetSerializer1_Parameter_Method.MakeGenericMethod(typeof (T))) as Func<SerializationContext, object, MessagePackSerializer<T>>;

      public static IMessagePackSingleObjectSerializer Get(
        SerializationContext context,
        object providerParameter)
      {
        return (IMessagePackSingleObjectSerializer) SerializationContext.SerializerGetter<T>._func(context, providerParameter);
      }
    }
  }
}
