// Decompiled with JetBrains decompiler
// Type: MsgPack.PackingOptions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Text;

namespace MsgPack
{
  public sealed class PackingOptions
  {
    private Encoding _stringEncoding = MessagePackConvert.Utf8NonBom;

    public Encoding StringEncoding
    {
      get
      {
        return this._stringEncoding;
      }
      set
      {
        this._stringEncoding = value;
      }
    }
  }
}
