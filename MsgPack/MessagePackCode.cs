// Decompiled with JetBrains decompiler
// Type: MsgPack.MessagePackCode
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack
{
  internal static class MessagePackCode
  {
    public const int NilValue = 192;
    public const int TrueValue = 195;
    public const int FalseValue = 194;
    public const int SignedInt8 = 208;
    public const int UnsignedInt8 = 204;
    public const int SignedInt16 = 209;
    public const int UnsignedInt16 = 205;
    public const int SignedInt32 = 210;
    public const int UnsignedInt32 = 206;
    public const int SignedInt64 = 211;
    public const int UnsignedInt64 = 207;
    public const int Real32 = 202;
    public const int Real64 = 203;
    public const int MinimumFixedArray = 144;
    public const int MaximumFixedArray = 159;
    public const int Array16 = 220;
    public const int Array32 = 221;
    public const int MinimumFixedMap = 128;
    public const int MaximumFixedMap = 143;
    public const int Map16 = 222;
    public const int Map32 = 223;
    public const int MinimumFixedRaw = 160;
    public const int MaximumFixedRaw = 191;
    public const int Str8 = 217;
    public const int Raw16 = 218;
    public const int Raw32 = 219;
    public const int Bin8 = 196;
    public const int Bin16 = 197;
    public const int Bin32 = 198;
    public const int Ext8 = 199;
    public const int Ext16 = 200;
    public const int Ext32 = 201;
    public const int FixExt1 = 212;
    public const int FixExt2 = 213;
    public const int FixExt4 = 214;
    public const int FixExt8 = 215;
    public const int FixExt16 = 216;

    public static string ToString(int code)
    {
      if (code < 128)
        return "PositiveFixNum";
      if (code >= 224)
        return "NegativeFixNum";
      switch (code - 192)
      {
        case 0:
          return "Nil";
        case 3:
          return "True";
        case 10:
          return "Real32";
        case 11:
          return "Real64";
        case 12:
          return "UnsignedInt8";
        case 13:
          return "UnsignedInt16";
        case 14:
          return "UnsignedInt32";
        case 15:
          return "UnsignedInt64";
        case 16:
          return "SingnedInt8";
        case 17:
          return "SignedInt16";
        case 18:
          return "SignedInt32";
        case 19:
          return "SignedInt64";
        case 26:
          return "Raw16";
        case 27:
          return "Raw32";
        case 28:
          return "Array16";
        case 29:
          return "Array32";
        case 30:
          return "Map16";
        case 31:
          return "Map32";
        default:
          switch (code & 240)
          {
            case 128:
              return "FixedMap";
            case 144:
              return "FixedArray";
            case 160:
            case 176:
              return "FixedRaw";
            default:
              return "Unknown";
          }
      }
    }
  }
}
