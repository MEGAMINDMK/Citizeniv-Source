// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionSerializerNilImplicationHandlerParameter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal struct ReflectionSerializerNilImplicationHandlerParameter : INilImplicationHandlerParameter
  {
    private readonly Type _itemType;
    private readonly string _memberName;

    public Type ItemType
    {
      get
      {
        return this._itemType;
      }
    }

    public string MemberName
    {
      get
      {
        return this._memberName;
      }
    }

    public ReflectionSerializerNilImplicationHandlerParameter(Type itemType, string memberName)
    {
      this._itemType = itemType;
      this._memberName = memberName;
    }
  }
}
