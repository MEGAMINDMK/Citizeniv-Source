// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.VariableILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class VariableILConstruct : ContextfulILConstruct
  {
    private readonly bool _isLocal;
    private int _index;
    private readonly string _name;

    public VariableILConstruct(string name, Type valueType)
      : base(valueType)
    {
      this._isLocal = true;
      this._name = name;
      this._index = -1;
    }

    public VariableILConstruct(string name, Type valueType, int parameterIndex)
      : base(valueType)
    {
      this._isLocal = false;
      this._name = name;
      this._index = parameterIndex;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      if (!this._isLocal || this._index >= 0)
        return;
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this._index = il.DeclareLocal(this.ContextType, this._name).LocalIndex;
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      this.Evaluate(il);
      il.TraceWriteLine("// Load->: {0}", (object) this);
      if (this.ContextType.GetIsValueType() && shouldBeAddress)
      {
        if (this._isLocal)
          il.EmitAnyLdloca(this._index);
        else
          il.EmitAnyLdarga(this._index);
      }
      else if (this._isLocal)
        il.EmitAnyLdloc(this._index);
      else
        il.EmitAnyLdarg(this._index);
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    public override void StoreValue(TracingILGenerator il)
    {
      il.TraceWriteLine("// Stor->: {0}", (object) this);
      if (!this._isLocal)
        throw new InvalidOperationException("Cannot overwrite argument.");
      il.EmitAnyStloc(this._index);
      il.TraceWriteLine("// ->Stor: {0}", (object) this);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Variable[{0}]: [{2}{3}]{1}({0})", (object) this.ContextType, (object) this._name, this._isLocal ? (object) "local" : (object) "arg", (object) this._index);
    }
  }
}
