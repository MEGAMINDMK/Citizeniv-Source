// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializationCompatibilityOptions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization
{
  public sealed class SerializationCompatibilityOptions
  {
    private PackerCompatibilityOptions _packerCompatibilityOptions;

    public bool OneBoundDataMemberOrder { get; set; }

    public PackerCompatibilityOptions PackerCompatibilityOptions
    {
      get
      {
        return this._packerCompatibilityOptions;
      }
      set
      {
        this._packerCompatibilityOptions = value;
      }
    }

    internal SerializationCompatibilityOptions()
    {
      this._packerCompatibilityOptions = PackerCompatibilityOptions.Classic;
    }
  }
}
