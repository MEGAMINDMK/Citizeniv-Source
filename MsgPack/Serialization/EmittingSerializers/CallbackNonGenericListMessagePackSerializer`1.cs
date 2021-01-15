// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.CallbackNonGenericListMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class CallbackNonGenericListMessagePackSerializer<TCollection> : NonGenericListMessagePackSerializer<TCollection>
    where TCollection : IList
  {
    private readonly Func<SerializationContext, int, TCollection> _createInstance;

    public CallbackNonGenericListMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<SerializationContext, int, TCollection> createInstance)
      : base(ownerContext, schema)
    {
      this._createInstance = createInstance;
    }

    protected override TCollection CreateInstance(int initialCapacity)
    {
      return this._createInstance(this.OwnerContext, initialCapacity);
    }
  }
}
