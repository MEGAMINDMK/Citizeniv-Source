// Decompiled with JetBrains decompiler
// Type: MsgPack.CollectionOperation
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace MsgPack
{
  internal static class CollectionOperation
  {
    public static void CopyTo<T>(
      IEnumerable<T> source,
      int sourceCount,
      int index,
      T[] array,
      int arrayIndex,
      int count)
    {
      CollectionOperation.ValidateCopyToArguments<T>(sourceCount, index, array, arrayIndex, count);
      int num = 0;
      foreach (T obj in source.Skip<T>(index).Take<T>(count))
      {
        array[num + arrayIndex] = obj;
        ++num;
      }
    }

    public static void CopyTo<TSource, TDestination>(
      IEnumerable<TSource> source,
      int sourceCount,
      int index,
      TDestination[] array,
      int arrayIndex,
      int count,
      Func<TSource, TDestination> converter)
    {
      CollectionOperation.ValidateCopyToArguments<TDestination>(sourceCount, index, array, arrayIndex, count);
      int num = 0;
      foreach (TSource source1 in source.Skip<TSource>(index).Take<TSource>(count))
      {
        array[num + arrayIndex] = converter(source1);
        ++num;
      }
    }

    private static void ValidateCopyToArguments<T>(
      int sourceCount,
      int index,
      T[] array,
      int arrayIndex,
      int count)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      if (0 < sourceCount && sourceCount <= index)
        throw new ArgumentException("index is too large to finish copying.", nameof (index));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (arrayIndex));
      if (array.Length - arrayIndex < count)
        throw new ArgumentException("array is too small to finish copying.", nameof (array));
    }

    public static void CopyTo<T>(
      IEnumerable<T> source,
      int sourceCount,
      Array array,
      int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (array.Rank != 1 || array.GetLowerBound(0) != 0)
        throw new ArgumentException("array is not zero-based one dimensional array.", nameof (array));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (arrayIndex));
      if (array.Length - arrayIndex < sourceCount)
        throw new ArgumentException("array is too small to finish copying.", nameof (array));
      if (array is T[] array1)
      {
        CollectionOperation.CopyTo<T>(source, sourceCount, 0, array1, arrayIndex, array1.Length);
      }
      else
      {
        int num = 0;
        foreach (T obj in source)
        {
          try
          {
            array.SetValue((object) obj, num + arrayIndex);
            ++num;
          }
          catch (InvalidCastException ex)
          {
            throw new ArgumentException("The type of destination array is not compatible to the type of items in the collection.", nameof (array));
          }
        }
      }
    }
  }
}
