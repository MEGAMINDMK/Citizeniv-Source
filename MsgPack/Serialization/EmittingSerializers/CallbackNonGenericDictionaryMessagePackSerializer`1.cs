// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.CallbackNonGenericDictionaryMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class CallbackNonGenericDictionaryMessagePackSerializer<TDictionary> : NonGenericDictionaryMessagePackSerializer<TDictionary>
    where TDictionary : IDictionary
  {
    private readonly Func<SerializationContext, int, TDictionary> _createInstance;

    public CallbackNonGenericDictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema,
      Func<SerializationContext, int, TDictionary> createInstance)
      : base(ownerContext, schema)
    {
      this._createInstance = createInstance;
    }

    protected override TDictionary CreateInstance(int initialCapacity)
    {
      return this._createInstance(this.OwnerContext, initialCapacity);
    }
  }
}
