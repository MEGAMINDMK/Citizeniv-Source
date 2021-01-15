// Decompiled with JetBrains decompiler
// Type: MsgPack.SetOperation
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace MsgPack
{
  internal static class SetOperation
  {
    public static bool IsProperSubsetOf<T>(ISet<T> set, IEnumerable<T> other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      if (other is ICollection<T> objs)
      {
        if (set.Count == 0)
          return 0 < objs.Count;
        if (objs.Count <= set.Count)
          return false;
      }
      int otherCount;
      return SetOperation.IsSubsetOfCore<T>(set, other, out otherCount) && set.Count < otherCount;
    }

    public static bool IsSubsetOf<T>(ISet<T> set, IEnumerable<T> other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      if (set.Count == 0)
        return true;
      return (!(other is ICollection<T> objs) || objs.Count >= set.Count) && SetOperation.IsSubsetOfCore<T>(set, other, out int _);
    }

    private static bool IsSubsetOfCore<T>(ISet<T> set, IEnumerable<T> other, out int otherCount)
    {
      otherCount = 0;
      if (!(other is ISet<T> objSet))
        objSet = (ISet<T>) new HashSet<T>(other);
      int num = 0;
      foreach (T obj in (IEnumerable<T>) objSet)
      {
        ++otherCount;
        if (set.Contains(obj))
          ++num;
      }
      return set.Count <= num;
    }

    public static bool IsProperSupersetOf<T>(ISet<T> set, IEnumerable<T> other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      if (other is ICollection<T> objs && objs.Count == 0)
        return 0 < set.Count;
      int otherCount;
      return SetOperation.IsSupersetOfCore<T>(set, other, out otherCount) && otherCount < set.Count;
    }

    public static bool IsSupersetOf<T>(ISet<T> set, IEnumerable<T> other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      if (other is ICollection<T> objs && objs.Count < set.Count)
      {
        if (objs.Count == 0)
          return true;
        if (set.Count <= objs.Count)
          return false;
      }
      return SetOperation.IsSupersetOfCore<T>(set, other, out int _);
    }

    private static bool IsSupersetOfCore<T>(ISet<T> set, IEnumerable<T> other, out int otherCount)
    {
      otherCount = 0;
      if (!(other is ISet<T> objSet))
        objSet = (ISet<T>) new HashSet<T>(other);
      foreach (T obj in (IEnumerable<T>) objSet)
      {
        ++otherCount;
        if (!set.Contains(obj))
          return false;
      }
      return true;
    }

    public static bool Overlaps<T>(ISet<T> set, IEnumerable<T> other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      return set.Count != 0 && other.Any<T>((Func<T, bool>) (item => ((ICollection<T>) set).Contains(item)));
    }

    public static bool SetEquals<T>(ISet<T> set, IEnumerable<T> other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      if (set.Count == 0 && other is ICollection<T> objs)
        return objs.Count == 0;
      if (!(other is ISet<T> objSet))
        objSet = (ISet<T>) new HashSet<T>(other);
      ISet<T> objSet1 = objSet;
      int num = 0;
      foreach (T obj in (IEnumerable<T>) objSet1)
      {
        if (!set.Contains(obj))
          return false;
        ++num;
      }
      return num == set.Count;
    }
  }
}
