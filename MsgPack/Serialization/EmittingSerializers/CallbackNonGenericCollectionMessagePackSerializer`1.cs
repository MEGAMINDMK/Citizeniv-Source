﻿// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.CallbackNonGenericCollectionMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class CallbackNonGenericCollectionMessagePackSerializer<TCollection> : NonGenericCollectionMessagePackSerializer<TCollection>
    where TCollection : ICollection
  {
    private readonly Func<SerializationContext, int, TCollection> _createInstance;
    private readonly Func<SerializationContext, Unpacker, TCollection> _unpackFromCore;
    private readonly Action<SerializationContext, TCollection, object> _addItem;

    public CallbackNonGenericCollectionMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<SerializationContext, int, TCollection> createInstance,
      Func<SerializationContext, Unpacker, TCollection> unpackFromCore,
      Action<SerializationContext, TCollection, object> addItem)
      : base(ownerContext, schema)
    {
      this._createInstance = createInstance;
      this._unpackFromCore = unpackFromCore;
      this._addItem = addItem;
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return this._createInstance(this.OwnerContext, initialCapacity);
    }

    protected internal override TCollection UnpackFromCore(Unpacker unpacker)
    {
      return this._unpackFromCore(this.OwnerContext, unpacker);
    }

    protected override void AddItem(TCollection collection, object item)
    {
      if (this._addItem != null)
        this._addItem(this.OwnerContext, collection, item);
      else
        base.AddItem(collection, item);
    }
  }
}
