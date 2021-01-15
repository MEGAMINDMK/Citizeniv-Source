// Decompiled with JetBrains decompiler
// Type: MsgPack.StringEscape
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Text;

namespace MsgPack
{
  internal static class StringEscape
  {
    public static string ForDisplay(string value)
    {
      if (value == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      foreach (char ch in value)
      {
        switch (ch)
        {
          case char.MinValue:
            stringBuilder.Append("\\0");
            break;
          case '\a':
            stringBuilder.Append("\\a");
            break;
          case '\b':
            stringBuilder.Append("\\b");
            break;
          case '\t':
            stringBuilder.Append("\\t");
            break;
          case '\n':
            stringBuilder.Append("\\n");
            break;
          case '\v':
            stringBuilder.Append("\\v");
            break;
          case '\f':
            stringBuilder.Append("\\f");
            break;
          case '\r':
            stringBuilder.Append("\\r");
            break;
          default:
            switch (CharUnicodeInfo.GetUnicodeCategory(ch))
            {
              case UnicodeCategory.Control:
              case UnicodeCategory.Format:
              case UnicodeCategory.PrivateUse:
              case UnicodeCategory.OtherNotAssigned:
                stringBuilder.Append("\\u").Append(((int) ch).ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture));
                continue;
              default:
                stringBuilder.Append(ch);
                continue;
            }
        }
      }
      return stringBuilder.ToString();
    }
  }
}
