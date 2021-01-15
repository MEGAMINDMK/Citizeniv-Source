// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.StoreVariableILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class StoreVariableILConstruct : ILConstruct
  {
    private readonly ILConstruct _variable;
    private readonly ILConstruct _value;

    public StoreVariableILConstruct(ILConstruct variable, ILConstruct value)
      : base(typeof (void))
    {
      this._variable = variable;
      this._value = value;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      if (this._value != null)
        this._value.LoadValue(il, false);
      this._variable.StoreValue(il);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StoreVariable: {0} = (context)", (object) this._variable);
    }
  }
}
