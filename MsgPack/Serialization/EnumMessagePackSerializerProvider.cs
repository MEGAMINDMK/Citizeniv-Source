// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EnumMessagePackSerializerProvider
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization
{
  internal sealed class EnumMessagePackSerializerProvider : MessagePackSerializerProvider
  {
    private readonly Type _enumType;
    private readonly object _serializerForName;
    private readonly object _serializerForIntegral;

    public EnumMessagePackSerializerProvider(Type enumType, ICustomizableEnumSerializer serializer)
    {
      this._enumType = enumType;
      this._serializerForName = (object) serializer.GetCopyAs(EnumSerializationMethod.ByName);
      this._serializerForIntegral = (object) serializer.GetCopyAs(EnumSerializationMethod.ByUnderlyingValue);
    }

    public override object Get(SerializationContext context, object providerParameter)
    {
      if (providerParameter is EnumSerializationMethod serializationMethod)
      {
        switch (serializationMethod)
        {
          case EnumSerializationMethod.ByName:
            return this._serializerForName;
          case EnumSerializationMethod.ByUnderlyingValue:
            return this._serializerForIntegral;
        }
      }
      return EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context, this._enumType, EnumMemberSerializationMethod.Default) == EnumSerializationMethod.ByUnderlyingValue ? this._serializerForIntegral : this._serializerForName;
    }
  }
}
