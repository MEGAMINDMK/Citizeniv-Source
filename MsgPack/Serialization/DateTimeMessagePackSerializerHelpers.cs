// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DateTimeMessagePackSerializerHelpers
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;

namespace MsgPack.Serialization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DateTimeMessagePackSerializerHelpers
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTimeConversionMethod DetermineDateTimeConversionMethod(
      SerializationContext context,
      DateTimeMemberConversionMethod dateTimeMemberConversionMethod)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      switch (dateTimeMemberConversionMethod)
      {
        case DateTimeMemberConversionMethod.Native:
          return DateTimeConversionMethod.Native;
        case DateTimeMemberConversionMethod.UnixEpoc:
          return DateTimeConversionMethod.UnixEpoc;
        default:
          return context.DefaultDateTimeConversionMethod;
      }
    }

    internal static bool IsDateTime(Type dateTimeType)
    {
      return dateTimeType == typeof (DateTime) || dateTimeType == typeof (DateTime?) || (dateTimeType == typeof (FILETIME) || dateTimeType == typeof (FILETIME?)) || dateTimeType == typeof (DateTimeOffset);
    }
  }
}
