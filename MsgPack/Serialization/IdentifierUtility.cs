// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.IdentifierUtility
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Text;

namespace MsgPack.Serialization
{
  internal static class IdentifierUtility
  {
    public static string EscapeTypeName(Type type)
    {
      return IdentifierUtility.EscapeTypeName(type.GetFullName());
    }

    public static string EscapeTypeName(string fullName)
    {
      StringBuilder stringBuilder = new StringBuilder(fullName.Length);
      bool flag = false;
      foreach (char ch in fullName)
      {
        switch (ch)
        {
          case ' ':
            flag = false;
            break;
          case '&':
            if (flag)
              stringBuilder.Append('_');
            flag = false;
            stringBuilder.Append("Reference");
            break;
          case '*':
            if (!flag)
            {
              stringBuilder.Append("Pointer");
              break;
            }
            break;
          case '+':
          case '.':
          case '`':
            flag = false;
            stringBuilder.Append('_');
            break;
          case ',':
            flag = false;
            stringBuilder.Append('_');
            break;
          case '[':
            flag = true;
            break;
          case ']':
            if (flag)
            {
              stringBuilder.Append("Array");
              flag = false;
              break;
            }
            stringBuilder.Append('_');
            break;
          default:
            if (flag)
              stringBuilder.Append('_');
            flag = false;
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }
  }
}
