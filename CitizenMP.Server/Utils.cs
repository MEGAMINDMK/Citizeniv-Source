// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Utils
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using MsgPack;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CitizenMP.Server
{
  internal static class Utils
  {
    public static Dictionary<string, string> ParseQueryString(string query)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string str1 = query.TrimStart('?');
      char[] separator1 = new char[1]{ '&' };
      foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
      {
        char[] separator2 = new char[1]{ '=' };
        string[] strArray = str2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
        dictionary[strArray[0].Trim()] = strArray.Length != 2 ? "" : strArray[1].Trim();
      }
      return dictionary;
    }

    public static string GetFileSHA1String(string filename)
    {
      using (FileStream fileStream = File.OpenRead(filename))
      {
        using (BufferedStream bufferedStream = new BufferedStream((Stream) fileStream, 32768))
        {
          using (SHA1 shA1 = SHA1.Create())
          {
            byte[] hash = shA1.ComputeHash((Stream) bufferedStream);
            StringBuilder stringBuilder = new StringBuilder(2 * hash.Length);
            foreach (byte num in hash)
              stringBuilder.AppendFormat("{0:X2}", (object) num);
            return stringBuilder.ToString();
          }
        }
      }
    }

    public static string[] Tokenize(string text)
    {
      int index1 = 0;
      int index2 = 0;
      string[] array = new string[0];
      while (true)
      {
        while (index1 >= text.Length || text[index1] > ' ')
        {
          if (index1 >= text.Length)
            return array;
          if (index1 != 0)
          {
            if (text[index1] == '/' && text[index1 + 1] == '/')
              return array;
            if (text[index1] == '/' && text[index1 + 1] == '*')
            {
              while (index1 < text.Length - 1 && (text[index1] != '*' || text[index1 + 1] != '/'))
                ++index1;
              if (index1 >= text.Length)
                return array;
              index1 += 2;
              continue;
            }
          }
          Array.Resize<string>(ref array, array.Length + 1);
          StringBuilder stringBuilder = new StringBuilder();
          if (text[index1] == '"')
          {
            bool flag = false;
            while (true)
            {
              ++index1;
              if (index1 < text.Length && (text[index1] != '"' || flag))
              {
                if (text[index1] == '\\')
                {
                  flag = true;
                }
                else
                {
                  stringBuilder.Append(text[index1]);
                  flag = false;
                }
              }
              else
                break;
            }
            ++index1;
            array[index2] = stringBuilder.ToString();
            ++index2;
            if (index1 >= text.Length)
              return array;
          }
          else
          {
            for (; index1 < text.Length && text[index1] > ' ' && text[index1] != '"' && (index1 >= text.Length - 1 || (text[index1] != '/' || text[index1 + 1] != '/') && (text[index1] != '/' || text[index1 + 1] != '*')); ++index1)
              stringBuilder.Append(text[index1]);
            array[index2] = stringBuilder.ToString();
            ++index2;
            if (index1 >= text.Length)
              return array;
          }
        }
        ++index1;
      }
    }

    static Utils()
    {
      SerializationContext.Default.ResolveSerializer += (EventHandler<ResolveSerializerEventArgs>) ((sender, args) =>
      {
        if (!args.TargetType.IsSubclassOf(typeof (Delegate)))
          return;
        object instance = Activator.CreateInstance(typeof (DelegateSerializer<>).MakeGenericType(args.TargetType));
        args.GetType().GetMethod("SetSerializer").MakeGenericMethod(args.TargetType).Invoke((object) args, new object[1]
        {
          instance
        });
      });
    }

    public static byte[] SerializeEvent(object[] args)
    {
      MemoryStream memoryStream = new MemoryStream();
      Packer packer = Packer.Create((Stream) memoryStream, PackerCompatibilityOptions.PackBinaryAsRaw);
      packer.PackArrayHeader(args.Length);
      for (int index = 0; index < args.Length; ++index)
        MessagePackSerializer.Get(args[index].GetType()).PackTo(packer, args[index]);
      return memoryStream.ToArray();
    }
  }
}
