// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DataMemberContract
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace MsgPack.Serialization
{
  internal struct DataMemberContract
  {
    internal const int UnspecifiedId = -1;
    private readonly string _name;
    private readonly int _id;
    private readonly NilImplication _nilImplication;

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public int Id
    {
      get
      {
        return this._id;
      }
    }

    public NilImplication NilImplication
    {
      get
      {
        return this._nilImplication;
      }
    }

    public DataMemberContract(MemberInfo member)
    {
      this._name = member.Name;
      this._nilImplication = NilImplication.MemberDefault;
      this._id = -1;
    }

    public DataMemberContract(
      MemberInfo member,
      string name,
      NilImplication nilImplication,
      int? id)
    {
      int? nullable = id;
      if ((nullable.GetValueOrDefault() >= 0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The member ID cannot be negative. The member is '{0}' in the '{1}' type.", (object) member.Name, (object) member.DeclaringType));
      this._name = string.IsNullOrEmpty(name) ? member.Name : name;
      this._nilImplication = nilImplication;
      this._id = id ?? -1;
    }

    public DataMemberContract(MemberInfo member, MessagePackMemberAttribute attribute)
    {
      if (attribute.Id < 0)
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The member ID cannot be negative. The member is '{0}' in the '{1}' type.", (object) member.Name, (object) member.DeclaringType));
      this._name = string.IsNullOrEmpty(attribute.Name) ? member.Name : attribute.Name;
      this._nilImplication = attribute.NilImplication;
      this._id = attribute.Id;
    }
  }
}
