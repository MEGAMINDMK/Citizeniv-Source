// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.NonGenericCollectionMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class NonGenericCollectionMessagePackSerializer<TCollection> : NonGenericEnumerableMessagePackSerializerBase<TCollection>
    where TCollection : ICollection
  {
    protected NonGenericCollectionMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
    }

    protected internal override sealed void PackToCore(Packer packer, TCollection objectTree)
    {
      packer.PackArrayHeader(objectTree.Count);
      IMessagePackSingleObjectSerializer itemSerializer = this.ItemSerializer;
      IEnumerator enumerator = objectTree.GetEnumerator();
      try
      {
        while (enumerator.MoveNext())
        {
          object current = enumerator.Current;
          itemSerializer.PackTo(packer, current);
        }
      }
      finally
      {
        if (enumerator is IDisposable disposable)
          disposable.Dispose();
      }
    }
  }
}
