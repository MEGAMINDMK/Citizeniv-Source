// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.ILEmittingContext
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Reflection;
using System;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class ILEmittingContext : SerializerGenerationContext<ILConstruct>
  {
    private readonly Func<SerializerEmitter> _emitterFactory;
    private readonly Func<EnumSerializerEmitter> _enumEmitterFactory;
    private SerializerEmitter _emitter;
    private EnumSerializerEmitter _enumEmitter;

    internal TracingILGenerator IL { get; set; }

    internal SerializerEmitter Emitter
    {
      get
      {
        if (this._emitter == null)
          this._emitter = this._emitterFactory();
        return this._emitter;
      }
    }

    internal EnumSerializerEmitter EnumEmitter
    {
      get
      {
        if (this._enumEmitter == null)
          this._enumEmitter = this._enumEmitterFactory();
        return this._enumEmitter;
      }
    }

    public Type GetSerializerType(Type targetType)
    {
      return typeof (MessagePackSerializer<>).MakeGenericType(targetType);
    }

    public ILEmittingContext(
      SerializationContext context,
      Func<SerializerEmitter> emitterFactory,
      Func<EnumSerializerEmitter> enumEmitterFactory)
      : base(context)
    {
      this._emitterFactory = emitterFactory;
      this._enumEmitterFactory = enumEmitterFactory;
    }

    protected override sealed void ResetCore(Type targetType, Type baseClass)
    {
      this.Packer = ILConstruct.Argument(1, typeof (Packer), "packer");
      this.PackToTarget = ILConstruct.Argument(2, targetType, "objectTree");
      this.Unpacker = ILConstruct.Argument(1, typeof (Unpacker), "unpacker");
      this.UnpackToTarget = ILConstruct.Argument(2, targetType, "collection");
      CollectionTraits collectionTraits = targetType.GetCollectionTraits();
      if (collectionTraits.ElementType != (Type) null)
      {
        this.CollectionToBeAdded = ILConstruct.Argument(1, targetType, "collection");
        this.ItemToAdd = ILConstruct.Argument(2, collectionTraits.ElementType, "item");
        if (collectionTraits.DetailedCollectionType == CollectionDetailedKind.GenericDictionary || collectionTraits.DetailedCollectionType == CollectionDetailedKind.GenericReadOnlyDictionary)
        {
          this.KeyToAdd = ILConstruct.Argument(2, collectionTraits.ElementType.GetGenericArguments()[0], "key");
          this.ValueToAdd = ILConstruct.Argument(3, collectionTraits.ElementType.GetGenericArguments()[1], "value");
        }
        this.InitialCapacity = ILConstruct.Argument(1, typeof (int), "initialCapacity");
      }
      this._emitter = (SerializerEmitter) null;
      this._enumEmitter = (EnumSerializerEmitter) null;
    }
  }
}
