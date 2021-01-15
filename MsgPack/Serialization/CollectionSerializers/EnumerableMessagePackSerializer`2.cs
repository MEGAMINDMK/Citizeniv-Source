// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.EnumerableMessagePackSerializer`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;
using System.Linq;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class EnumerableMessagePackSerializer<TCollection, TItem> : EnumerableMessagePackSerializerBase<TCollection, TItem>
    where TCollection : IEnumerable<TItem>
  {
    protected EnumerableMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, TCollection objectTree)
    {
      if (!(objectTree is ICollection<TItem> objs))
        objs = (ICollection<TItem>) objectTree.ToArray<TItem>();
      packer.PackArrayHeader(objs.Count);
      MessagePackSerializer<TItem> itemSerializer = this.ItemSerializer;
      foreach (TItem objectTree1 in (IEnumerable<TItem>) objs)
        itemSerializer.PackTo(packer, objectTree1);
    }
  }
}
