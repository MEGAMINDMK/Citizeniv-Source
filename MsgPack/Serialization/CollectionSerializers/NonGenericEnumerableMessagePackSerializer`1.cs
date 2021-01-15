// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.NonGenericEnumerableMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections;
using System.Linq;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class NonGenericEnumerableMessagePackSerializer<TCollection> : NonGenericEnumerableMessagePackSerializerBase<TCollection>
    where TCollection : IEnumerable
  {
    protected NonGenericEnumerableMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, TCollection objectTree)
    {
      if (!(objectTree is ICollection collection))
        collection = (ICollection) objectTree.Cast<object>().ToArray<object>();
      packer.PackArrayHeader(collection.Count);
    }
  }
}
