// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.LoadFieldILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;
using System.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class LoadFieldILConstruct : ContextfulILConstruct
  {
    private readonly ILConstruct _instance;
    private readonly FieldInfo _field;

    public LoadFieldILConstruct(ILConstruct instance, FieldInfo field)
      : base(field.FieldType)
    {
      this._instance = instance;
      this._field = field;
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Load->: {0}", (object) this);
      if (this._instance != null)
        this._instance.LoadValue(il, this._instance.ContextType.GetIsValueType());
      if (shouldBeAddress)
        il.EmitLdflda(this._field);
      else
        il.EmitLdfld(this._field);
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "LoadField[{0}]: {1}", (object) this.ContextType, (object) this._field);
    }
  }
}
