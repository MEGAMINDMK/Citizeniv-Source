// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.StoreFieldILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;
using System.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class StoreFieldILConstruct : ContextfulILConstruct
  {
    private readonly ILConstruct _instance;
    private readonly ILConstruct _value;
    private readonly FieldInfo _field;

    public StoreFieldILConstruct(ILConstruct instance, FieldInfo field, ILConstruct value)
      : base(typeof (void))
    {
      this._instance = instance;
      this._field = field;
      this._value = value;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      this.StoreValue(il);
    }

    public override void StoreValue(TracingILGenerator il)
    {
      il.TraceWriteLine("// Stor->: {0}", (object) this);
      if (this._instance != null)
        this._instance.LoadValue(il, this._instance.ContextType.GetIsValueType());
      this._value.LoadValue(il, false);
      il.EmitStfld(this._field);
      il.TraceWriteLine("// ->Stor: {0}", (object) this);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StoreField[void]: {0}", (object) this._field);
    }
  }
}
