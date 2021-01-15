// Decompiled with JetBrains decompiler
// Type: MsgPack.UnpackingStream
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.IO;

namespace MsgPack
{
  public abstract class UnpackingStream : Stream
  {
    internal readonly Stream Underlying;
    internal readonly long RawLength;
    internal long CurrentOffset;

    public override sealed bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override sealed bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override sealed long Length
    {
      get
      {
        return this.RawLength;
      }
    }

    public override sealed bool CanTimeout
    {
      get
      {
        return this.Underlying.CanTimeout;
      }
    }

    internal UnpackingStream(Stream underlying, long rawLength)
    {
      this.Underlying = underlying;
      this.RawLength = rawLength;
    }

    public override sealed int Read(byte[] buffer, int offset, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (this.CurrentOffset == this.RawLength)
        return 0;
      int count1 = count;
      long num1 = this.CurrentOffset + (long) count - this.RawLength;
      if (num1 > 0L)
        count1 -= checked ((int) num1);
      int num2 = this.Underlying.Read(buffer, offset, count1);
      this.CurrentOffset += (long) num2;
      return num2;
    }

    public override sealed void Flush()
    {
    }

    public override sealed void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override sealed void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }
  }
}
