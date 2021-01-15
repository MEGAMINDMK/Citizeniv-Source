// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.MessagePackMemberAttribute
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public sealed class MessagePackMemberAttribute : Attribute
  {
    private readonly int _id;
    private string _name;
    private NilImplication _nilImplication;

    public int Id
    {
      get
      {
        return this._id;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    public NilImplication NilImplication
    {
      get
      {
        return this._nilImplication;
      }
      set
      {
        switch (value)
        {
          case NilImplication.MemberDefault:
          case NilImplication.Null:
          case NilImplication.Prohibit:
            this._nilImplication = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (value));
        }
      }
    }

    public MessagePackMemberAttribute(int id)
    {
      this._id = id;
    }
  }
}
