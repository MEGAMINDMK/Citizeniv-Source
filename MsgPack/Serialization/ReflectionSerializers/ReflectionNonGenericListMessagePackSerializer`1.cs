// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionNonGenericListMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System;
using System.Collections;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal sealed class ReflectionNonGenericListMessagePackSerializer<TList> : NonGenericListMessagePackSerializer<TList>
    where TList : IList
  {
    private readonly Func<int, TList> _factory;

    public ReflectionNonGenericListMessagePackSerializer(
      SerializationContext ownerContext,
      Type targetType,
      PolymorphismSchema itemsSchema)
      : base(ownerContext, itemsSchema)
    {
      this._factory = ReflectionSerializerHelper.CreateCollectionInstanceFactory<TList, object>(targetType);
    }

    protected override TList CreateInstance(int initialCapacity)
    {
      return this._factory(initialCapacity);
    }
  }
}
