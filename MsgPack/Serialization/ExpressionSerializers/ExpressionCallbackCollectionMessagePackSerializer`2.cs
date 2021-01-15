// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionCallbackCollectionMessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal class ExpressionCallbackCollectionMessagePackSerializer<TCollection, TItem> : CollectionMessagePackSerializer<TCollection, TItem>
    where TCollection : ICollection<TItem>
  {
    private readonly Func<ExpressionCallbackCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, int, TCollection> _createInstance;

    public ExpressionCallbackCollectionMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<ExpressionCallbackCollectionMessagePackSerializer<TCollection, TItem>, SerializationContext, int, TCollection> createInstance)
      : base(ownerContext, schema)
    {
      this._createInstance = createInstance;
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return this._createInstance(this, this.OwnerContext, initialCapacity);
    }
  }
}
