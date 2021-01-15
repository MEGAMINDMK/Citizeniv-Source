// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.ArraySegmentMessageSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal static class ArraySegmentMessageSerializer
  {
    public static void PackByteArraySegmentTo(
      Packer packer,
      ArraySegment<byte> objectTree,
      MessagePackSerializer<byte> itemSerializer)
    {
      if (objectTree.Array == null)
      {
        packer.PackBinaryHeader(0);
      }
      else
      {
        packer.PackBinaryHeader(objectTree.Count);
        packer.PackRawBody(((IEnumerable<byte>) objectTree.Array).Skip<byte>(objectTree.Offset).Take<byte>(objectTree.Count));
      }
    }

    public static void PackCharArraySegmentTo(
      Packer packer,
      ArraySegment<char> objectTree,
      MessagePackSerializer<char> itemSerializer)
    {
      packer.PackStringHeader(objectTree.Count);
      packer.PackRawBody(MessagePackConvert.EncodeString(new string(((IEnumerable<char>) objectTree.Array).Skip<char>(objectTree.Offset).Take<char>(objectTree.Count).ToArray<char>())));
    }

    public static void PackGenericArraySegmentTo<T>(
      Packer packer,
      ArraySegment<T> objectTree,
      MessagePackSerializer<T> itemSerializer)
    {
      packer.PackArrayHeader(objectTree.Count);
      for (int index = 0; index < objectTree.Count; ++index)
        itemSerializer.PackTo(packer, objectTree.Array[index + objectTree.Offset]);
    }

    public static ArraySegment<byte> UnpackByteArraySegmentFrom(
      Unpacker unpacker,
      MessagePackSerializer<byte> itemSerializer)
    {
      return new ArraySegment<byte>(unpacker.LastReadData.AsBinary());
    }

    public static ArraySegment<char> UnpackCharArraySegmentFrom(
      Unpacker unpacker,
      MessagePackSerializer<char> itemSerializer)
    {
      return new ArraySegment<char>(unpacker.LastReadData.AsCharArray());
    }

    public static ArraySegment<T> UnpackGenericArraySegmentFrom<T>(
      Unpacker unpacker,
      MessagePackSerializer<T> itemSerializer)
    {
      T[] array = new T[unpacker.ItemsCount];
      for (int index = 0; index < array.Length; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        array[index] = itemSerializer.UnpackFrom(unpacker);
      }
      return new ArraySegment<T>(array);
    }
  }
}
