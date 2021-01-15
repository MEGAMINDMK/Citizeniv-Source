﻿// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionCallbackReadOnlyCollectionMessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal class ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem> : ReadOnlyCollectionMessagePackSerializer<TCollection, TItem>
    where TCollection : IReadOnlyCollection<TItem>
  {
    private readonly Func<ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, int, TCollection> _createInstance;
    private readonly Action<ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, TCollection, TItem> _addItem;

    public ExpressionCallbackReadOnlyCollectionMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, int, TCollection> createInstance,
      Action<ExpressionCallbackReadOnlyCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, TCollection, TItem> addItem)
      : base(ownerContext, schema)
    {
      this._createInstance = createInstance;
      this._addItem = addItem;
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return this._createInstance(this, this.OwnerContext, initialCapacity);
    }

    protected override void AddItem(TCollection collection, TItem item)
    {
      if (this._addItem != null)
        this._addItem(this, this.OwnerContext, collection, item);
      else
        base.AddItem(collection, item);
    }
  }
}
