// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Game.PartialStream
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.IO;

namespace CitizenMP.Server.Game
{
  internal class PartialStream : Stream
  {
    private long m_length;

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return true;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public Stream BaseStream { get; private set; }

    public long RangeStart { get; private set; }

    public override long Length
    {
      get
      {
        return this.m_length;
      }
    }

    public override long Position { get; set; }

    public PartialStream(Stream baseStream, long rangeStart, long length)
    {
      this.BaseStream = baseStream;
      this.RangeStart = rangeStart;
      this.m_length = length;
      if (!baseStream.CanSeek)
        throw new ArgumentException("PartialStream requires a seekable stream.", nameof (baseStream));
      if (baseStream.Length < rangeStart + length)
        throw new ArgumentException("Stream does not contain the requested range.", nameof (baseStream));
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      long position = this.BaseStream.Position;
      this.BaseStream.Position = this.Position + this.RangeStart;
      int num = this.BaseStream.Read(buffer, offset, count);
      this.Position += (long) num;
      this.BaseStream.Position = position;
      return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          this.Position = offset;
          break;
        case SeekOrigin.Current:
          this.Position += offset;
          break;
        case SeekOrigin.End:
          this.Position = this.Length - offset;
          break;
      }
      return this.Position;
    }
  }
}
