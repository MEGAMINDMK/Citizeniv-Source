// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializingMember
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Reflection;

namespace MsgPack.Serialization
{
  internal struct SerializingMember
  {
    public readonly MemberInfo Member;
    public readonly DataMemberContract Contract;

    public SerializingMember(MemberInfo member, DataMemberContract contract)
    {
      this.Member = member;
      this.Contract = contract;
    }

    public EnumMemberSerializationMethod GetEnumMemberSerializationMethod()
    {
      object[] customAttributes = this.Member.GetCustomAttributes(typeof (MessagePackEnumMemberAttribute), true);
      return customAttributes.Length > 0 ? (customAttributes[0] as MessagePackEnumMemberAttribute).SerializationMethod : EnumMemberSerializationMethod.Default;
    }

    public DateTimeMemberConversionMethod GetDateTimeMemberConversionMethod()
    {
      object[] customAttributes = this.Member.GetCustomAttributes(typeof (MessagePackDateTimeMemberAttribute), true);
      return customAttributes.Length > 0 ? (customAttributes[0] as MessagePackDateTimeMemberAttribute).DateTimeConversionMethod : DateTimeMemberConversionMethod.Default;
    }
  }
}
