// Decompiled with JetBrains decompiler
// Type: MsgPack.UnpackingStreamReader
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.IO;
using System.Text;

namespace MsgPack
{
  public abstract class UnpackingStreamReader : StreamReader
  {
    private readonly long _byteLength;

    public long ByteLength
    {
      get
      {
        return this._byteLength;
      }
    }

    internal UnpackingStreamReader(Stream stream, Encoding encoding, long byteLength)
      : base(stream, encoding, true)
    {
      this._byteLength = byteLength;
    }
  }
}
