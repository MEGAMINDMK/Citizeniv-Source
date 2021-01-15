// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.FileTimeMessagePackSerializerProvider
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal class FileTimeMessagePackSerializerProvider : MessagePackSerializerProvider
  {
    private readonly IMessagePackSingleObjectSerializer _unixEpoc;
    private readonly IMessagePackSingleObjectSerializer _native;

    public FileTimeMessagePackSerializerProvider(SerializationContext context, bool isNullable)
    {
      if (isNullable)
      {
        this._unixEpoc = (IMessagePackSingleObjectSerializer) new NullableMessagePackSerializer<FILETIME>(context, (MessagePackSerializer<FILETIME>) new UnixEpocFileTimeMessagePackSerializer(context));
        this._native = (IMessagePackSingleObjectSerializer) new NullableMessagePackSerializer<FILETIME>(context, (MessagePackSerializer<FILETIME>) new NativeFileTimeMessagePackSerializer(context));
      }
      else
      {
        this._unixEpoc = (IMessagePackSingleObjectSerializer) new UnixEpocFileTimeMessagePackSerializer(context);
        this._native = (IMessagePackSingleObjectSerializer) new NativeFileTimeMessagePackSerializer(context);
      }
    }

    public override object Get(SerializationContext context, object providerParameter)
    {
      if (providerParameter is DateTimeConversionMethod conversionMethod)
      {
        switch (conversionMethod)
        {
          case DateTimeConversionMethod.Native:
            return (object) this._native;
          case DateTimeConversionMethod.UnixEpoc:
            return (object) this._unixEpoc;
        }
      }
      switch (context.DefaultDateTimeConversionMethod)
      {
        case DateTimeConversionMethod.Native:
          return (object) this._native;
        case DateTimeConversionMethod.UnixEpoc:
          return (object) this._unixEpoc;
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unknown {0} value '{1:G}'({1:D})'", (object) typeof (DateTimeConversionMethod), (object) context.DefaultDateTimeConversionMethod));
      }
    }
  }
}
