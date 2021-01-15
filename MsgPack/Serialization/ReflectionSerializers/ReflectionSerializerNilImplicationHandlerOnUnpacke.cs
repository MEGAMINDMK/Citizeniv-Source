// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionSerializerNilImplicationHandlerOnUnpackedParameter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal struct ReflectionSerializerNilImplicationHandlerOnUnpackedParameter : INilImplicationHandlerOnUnpackedParameter<Action<object>>, INilImplicationHandlerParameter
  {
    private readonly Type _itemType;
    private readonly Action<object> _store;
    public readonly string MemberName;
    public readonly Type DeclaringType;

    public Type ItemType
    {
      get
      {
        return this._itemType;
      }
    }

    public Action<object> Store
    {
      get
      {
        return this._store;
      }
    }

    public ReflectionSerializerNilImplicationHandlerOnUnpackedParameter(
      Type itemType,
      Action<object> store,
      string memberName,
      Type declaringType)
    {
      this._itemType = itemType;
      this._store = store;
      this.MemberName = memberName;
      this.DeclaringType = declaringType;
    }
  }
}
