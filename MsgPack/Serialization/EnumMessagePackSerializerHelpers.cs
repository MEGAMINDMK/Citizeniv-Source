// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EnumMessagePackSerializerHelpers
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.ComponentModel;

namespace MsgPack.Serialization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class EnumMessagePackSerializerHelpers
  {
    public static EnumSerializationMethod DetermineEnumSerializationMethod(
      SerializationContext context,
      Type enumType,
      EnumMemberSerializationMethod enumMemberSerializationMethod)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (enumType == (Type) null)
        throw new ArgumentNullException(nameof (enumType));
      EnumSerializationMethod serializationMethod = context.EnumSerializationMethod;
      switch (enumMemberSerializationMethod)
      {
        case EnumMemberSerializationMethod.ByName:
          serializationMethod = EnumSerializationMethod.ByName;
          break;
        case EnumMemberSerializationMethod.ByUnderlyingValue:
          serializationMethod = EnumSerializationMethod.ByUnderlyingValue;
          break;
        default:
          object[] customAttributes = enumType.GetCustomAttributes(typeof (MessagePackEnumAttribute), true);
          if (customAttributes.Length > 0)
          {
            serializationMethod = (customAttributes[0] as MessagePackEnumAttribute).SerializationMethod;
            break;
          }
          break;
      }
      return serializationMethod;
    }
  }
}
