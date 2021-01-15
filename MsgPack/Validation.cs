// Decompiled with JetBrains decompiler
// Type: MsgPack.Validation
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MsgPack
{
  internal static class Validation
  {
    private static readonly Regex NamespacePattern = new Regex("^([\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]*)(\\.[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]*)*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
    private const string UnicodeTr15Annex7Idneifier = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]*";

    public static void ValidateIsNotNullNorEmpty(string value, string parameterName)
    {
      switch (value)
      {
        case "":
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be empty.", (object) parameterName), parameterName);
        case null:
          throw new ArgumentNullException(parameterName);
      }
    }

    public static void ValidateNamespace(string @namespace, string parameterName)
    {
      switch (@namespace)
      {
        case "":
          break;
        case null:
          throw new ArgumentNullException(nameof (@namespace));
        default:
          MatchCollection matchCollection = Validation.NamespacePattern.Matches(@namespace);
          if (matchCollection.Count == 1 && matchCollection[0].Success && (matchCollection[0].Index == 0 && matchCollection[0].Length == @namespace.Length))
            break;
          int index1 = 0;
          int num = 0;
          for (int index2 = 0; index2 < matchCollection.Count; ++index2)
          {
            if (matchCollection[index2].Index == num)
            {
              num += matchCollection[index2].Length;
            }
            else
            {
              index1 = num;
              break;
            }
          }
          UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(@namespace, index1);
          if (Validation.IsPrintable(unicodeCategory))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Char at {0}('{1}'\\u{2}[{3}] is not used for namespace.", (object) index1, (object) @namespace[index1], (object) (ushort) @namespace[index1], (object) unicodeCategory));
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Char at {0}(\\u{1}[{2}] is not used for namespace.", (object) index1, (object) (ushort) @namespace[index1], (object) unicodeCategory));
      }
    }

    private static bool IsPrintable(UnicodeCategory category)
    {
      switch (category)
      {
        case UnicodeCategory.UppercaseLetter:
        case UnicodeCategory.LowercaseLetter:
        case UnicodeCategory.TitlecaseLetter:
        case UnicodeCategory.OtherLetter:
        case UnicodeCategory.NonSpacingMark:
        case UnicodeCategory.EnclosingMark:
        case UnicodeCategory.DecimalDigitNumber:
        case UnicodeCategory.LetterNumber:
        case UnicodeCategory.OtherNumber:
        case UnicodeCategory.ConnectorPunctuation:
        case UnicodeCategory.DashPunctuation:
        case UnicodeCategory.OpenPunctuation:
        case UnicodeCategory.ClosePunctuation:
        case UnicodeCategory.InitialQuotePunctuation:
        case UnicodeCategory.FinalQuotePunctuation:
        case UnicodeCategory.OtherPunctuation:
        case UnicodeCategory.MathSymbol:
        case UnicodeCategory.CurrencySymbol:
        case UnicodeCategory.OtherSymbol:
          return false;
        default:
          return true;
      }
    }
  }
}
