// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionCallbackNonGenericListMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal class ExpressionCallbackNonGenericListMessagePackSerializer<TCollection> : NonGenericListMessagePackSerializer<TCollection>
    where TCollection : IList
  {
    private readonly Func<ExpressionCallbackNonGenericListMessagePackSerializer<TCollection>, SerializationContext, int, TCollection> _createInstance;

    public ExpressionCallbackNonGenericListMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<ExpressionCallbackNonGenericListMessagePackSerializer<TCollection>, SerializationContext, int, TCollection> createInstance)
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
