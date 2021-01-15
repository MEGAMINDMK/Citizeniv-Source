// Decompiled with JetBrains decompiler
// Type: MsgPack.UnpackingResult`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MsgPack
{
  public struct UnpackingResult<T> : IEquatable<UnpackingResult<T>>
  {
    private readonly int _readCount;
    private readonly T _value;

    public int ReadCount
    {
      get
      {
        return this._readCount;
      }
    }

    public T Value
    {
      get
      {
        return this._value;
      }
    }

    internal UnpackingResult(T value, int readCount)
    {
      this._value = value;
      this._readCount = readCount;
    }

    public override bool Equals(object obj)
    {
      return obj is UnpackingResult<T> other && this.Equals(other);
    }

    public bool Equals(UnpackingResult<T> other)
    {
      return this._readCount == other._readCount && EqualityComparer<T>.Default.Equals(this._value, other._value);
    }

    public override int GetHashCode()
    {
      return this._readCount.GetHashCode() ^ ((object) this._value == null ? 0 : this._value.GetHashCode());
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}({1} bytes)", (object) this._value, (object) this._readCount);
    }

    public static bool operator ==(UnpackingResult<T> left, UnpackingResult<T> right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(UnpackingResult<T> left, UnpackingResult<T> right)
    {
      return !left.Equals(right);
    }
  }
}
