// Decompiled with JetBrains decompiler
// Type: MsgPack.StreamPacker
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.IO;

namespace MsgPack
{
  internal class StreamPacker : Packer
  {
    private readonly Stream _stream;
    private readonly bool _ownsStream;

    public override sealed bool CanSeek
    {
      get
      {
        return this._stream.CanSeek;
      }
    }

    public override sealed long Position
    {
      get
      {
        return this._stream.Position;
      }
    }

    public StreamPacker(
      Stream output,
      PackerCompatibilityOptions compatibilityOptions,
      bool ownsStream)
      : base(compatibilityOptions)
    {
      this._stream = output;
      this._ownsStream = ownsStream;
    }

    protected override sealed void Dispose(bool disposing)
    {
      if (this._ownsStream)
        this._stream.Dispose();
      base.Dispose(disposing);
    }

    protected override sealed void SeekTo(long offset)
    {
      if (!this.CanSeek)
        throw new NotSupportedException();
      this._stream.Seek(offset, SeekOrigin.Current);
    }

    protected override sealed void WriteByte(byte value)
    {
      this._stream.WriteByte(value);
    }

    protected override sealed void WriteBytes(byte[] asArray, bool isImmutable)
    {
      this._stream.Write(asArray, 0, asArray.Length);
    }
  }
}
