// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.CollectionMessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class CollectionMessagePackSerializer<TCollection, TItem> : CollectionMessagePackSerializerBase<TCollection, TItem>
    where TCollection : ICollection<TItem>
  {
    protected CollectionMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
    }

    protected override int GetCount(TCollection collection)
    {
      return collection.Count;
    }

    protected override void AddItem(TCollection collection, TItem item)
    {
      collection.Add(item);
    }
  }
}
