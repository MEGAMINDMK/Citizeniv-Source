// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Extensions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Neo.IronLua;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CitizenMP.Server
{
  public static class Extensions
  {
    public static void Initialize()
    {
      LuaType.RegisterTypeExtension(typeof (Extensions));
    }

    [LuaMember("ssub", false)]
    public static string sub(this string s, int i, int j = -1)
    {
      if (string.IsNullOrEmpty(s) || j == 0)
        return string.Empty;
      if (i == 0)
        i = 1;
      int startIndex;
      int length;
      if (i < 0)
      {
        startIndex = s.Length + i;
        if (startIndex < 0)
          startIndex = 0;
        length = (j < 0 ? s.Length + j + 1 : j) - startIndex;
      }
      else
      {
        startIndex = i - 1;
        if (j < 0)
          j = s.Length + j + 1;
        length = j - startIndex;
      }
      if (startIndex + length > s.Length)
        length = s.Length - startIndex;
      return length <= 0 ? string.Empty : s.Substring(startIndex, length);
    }

    private static string TranslateRegularExpression(string sRegEx)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = false;
      for (int index = 0; index < sRegEx.Length; ++index)
      {
        char lower = sRegEx[index];
        if (flag1)
        {
          if (lower == '%')
          {
            stringBuilder.Append('%');
            flag1 = false;
          }
          else
          {
            bool flag2 = false;
            if (char.IsUpper(lower))
            {
              lower = char.ToLower(lower);
              stringBuilder.Append("[^");
              flag2 = true;
            }
            switch (lower)
            {
              case 'a':
                stringBuilder.Append("[\\w-[\\d]]");
                break;
              case 'c':
              case 'g':
              case 'l':
              case 'p':
              case 'u':
              case 'x':
                throw new NotImplementedException();
              case 'd':
                stringBuilder.Append("\\d");
                break;
              case 's':
                stringBuilder.Append("\\s");
                break;
              case 'w':
                stringBuilder.Append("\\w");
                break;
              case 'z':
                stringBuilder.Append("\0");
                break;
              default:
                stringBuilder.Append('\\');
                stringBuilder.Append(lower);
                break;
            }
            if (flag2)
              stringBuilder.Append("]");
            flag1 = false;
          }
        }
        else
        {
          switch (lower)
          {
            case '%':
              flag1 = true;
              continue;
            case '\\':
              stringBuilder.Append("\\\\");
              continue;
            default:
              stringBuilder.Append(lower);
              continue;
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static LuaResult @byte(this string s, int i = 1, int j = 2147483647)
    {
      if (string.IsNullOrEmpty(s) || i > j)
        return LuaResult.get_Empty();
      if (i < 1)
        i = 1;
      if (j == int.MaxValue)
        j = i;
      else if (j > s.Length)
        j = s.Length;
      int length = j - i + 1;
      object[] objArray = new object[length];
      for (int index = 0; index < length; ++index)
        objArray[index] = (object) (int) s[i + index - 1];
      return LuaResult.op_Implicit(objArray);
    }

    public static string dump(Delegate dlg)
    {
      throw new NotImplementedException();
    }

    [LuaMember("sfind", false)]
    public static LuaResult find(this string s, string pattern, int init = 1, bool plain = false)
    {
      if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
        return LuaResult.get_Empty();
      if (init < 0)
        init = s.Length + init + 1;
      if (init <= 0)
        init = 1;
      if (plain)
      {
        int num = s.IndexOf(pattern, init - 1);
        if (num < 0)
          return new LuaResult((object[]) null);
        return new LuaResult(new object[2]
        {
          (object) (num + 1),
          (object) (num + pattern.Length)
        });
      }
      pattern = Extensions.TranslateRegularExpression(pattern);
      Match match = new Regex(pattern).Match(s.Substring(init - 1));
      if (!match.Success)
        return new LuaResult((object[]) null);
      object[] objArray = new object[match.Captures.Count + 2];
      objArray[0] = (object) (match.Index + (init - 1) + 1);
      objArray[1] = (object) (match.Index + (init - 1) + match.Length);
      for (int index = 0; index < match.Captures.Count; ++index)
        objArray[index + 2] = (object) match.Captures[index].Value;
      return LuaResult.op_Implicit(objArray);
    }

    private static LuaResult matchEnum(object s, object current)
    {
      IEnumerator enumerator = (IEnumerator) s;
      return enumerator.MoveNext() ? Extensions.MatchResult((Match) enumerator.Current) : LuaResult.get_Empty();
    }

    public static LuaResult gmatch(this string s, string pattern)
    {
      if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
        return LuaResult.get_Empty();
      pattern = Extensions.TranslateRegularExpression(pattern);
      IEnumerator enumerator = new Regex(pattern).Matches(s).GetEnumerator();
      return new LuaResult(new object[3]
      {
        (object) new Func<object, object, LuaResult>(Extensions.matchEnum),
        (object) enumerator,
        (object) enumerator
      });
    }

    [LuaMember("gsub", false)]
    public static string gsub(this string s, string pattern, string repl, int n)
    {
      if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
        return string.Empty;
      pattern = Extensions.TranslateRegularExpression(pattern);
      return new Regex(pattern).Replace(s, repl.Replace('%', '$'), n == 0 ? int.MaxValue : n);
    }

    [LuaMember("gsubf", false)]
    public static string gsub(this string s, string pattern, Func<string, LuaResult> repl, int n)
    {
      if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
        return string.Empty;
      pattern = Extensions.TranslateRegularExpression(pattern);
      return new Regex(pattern).Replace(s, (MatchEvaluator) (match => repl(match.Value).get_Item(0).ToString()), n == 0 ? int.MaxValue : n);
    }

    public static int len(this string s)
    {
      return s != null ? s.Length : 0;
    }

    public static string lower(this string s)
    {
      return string.IsNullOrEmpty(s) ? s : s.ToLower();
    }

    public static LuaResult match(this string s, string pattern, int init = 1)
    {
      if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
        return LuaResult.get_Empty();
      if (init < 0)
        init = s.Length + init + 1;
      if (init <= 0)
        init = 1;
      pattern = Extensions.TranslateRegularExpression(pattern);
      return Extensions.MatchResult(new Regex(pattern).Match(s, init));
    }

    private static LuaResult MatchResult(Match m)
    {
      if (!m.Success)
        return LuaResult.get_Empty();
      object[] objArray = new object[m.Captures.Count];
      for (int index = 0; index < m.Captures.Count; ++index)
        objArray[index] = (object) m.Captures[index].Value;
      return LuaResult.op_Implicit(objArray);
    }

    public static string rep(this string s, int n, string sep = "")
    {
      return string.IsNullOrEmpty(s) || n == 0 ? s : string.Join(sep, Enumerable.Repeat<string>(s, n));
    }

    public static string reverse(this string s)
    {
      if (string.IsNullOrEmpty(s) || s.Length == 1)
        return s;
      char[] charArray = s.ToCharArray();
      Array.Reverse((Array) charArray);
      return new string(charArray);
    }

    public static string upper(this string s)
    {
      return string.IsNullOrEmpty(s) ? s : s.ToUpper();
    }
  }
}
