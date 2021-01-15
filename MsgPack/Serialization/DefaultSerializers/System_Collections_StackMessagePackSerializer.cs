// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Collections_StackMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using System.Collections;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Collections_StackMessagePackSerializer : MessagePackSerializer<Stack>, ICollectionInstanceFactory
  {
    public System_Collections_StackMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, Stack objectTree)
    {
      packer.PackArrayHeader(objectTree.Count);
      foreach (object obj in objectTree)
        packer.PackObject(obj);
    }

    protected internal override Stack UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      return new Stack(System_Collections_StackMessagePackSerializer.UnpackItemsInReverseOrder(unpacker, UnpackHelpers.GetItemsCount(unpacker)));
    }

    protected internal override void UnpackToCore(Unpacker unpacker, Stack collection)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      foreach (object obj in (IEnumerable) System_Collections_StackMessagePackSerializer.UnpackItemsInReverseOrder(unpacker, UnpackHelpers.GetItemsCount(unpacker)))
        collection.Push(obj);
    }

    private static ICollection UnpackItemsInReverseOrder(Unpacker unpacker, int count)
    {
      object[] objArray = new object[count];
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        for (int index = objArray.Length - 1; index >= 0; --index)
        {
          if (!unpacker1.Read())
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          objArray[index] = (object) unpacker1.LastReadData;
        }
      }
      return (ICollection) objArray;
    }

    public object CreateInstance(int initialCapacity)
    {
      return (object) new Stack(initialCapacity);
    }
  }
}
