// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.SinglelStepILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class SinglelStepILConstruct : ILConstruct
  {
    private readonly string _description;
    private readonly Action<TracingILGenerator> _instruction;
    private readonly bool _isTerminating;

    public override bool IsTerminating
    {
      get
      {
        return this._isTerminating;
      }
    }

    public SinglelStepILConstruct(
      Type contextType,
      string description,
      bool isTerminating,
      Action<TracingILGenerator> instruction)
      : base(contextType)
    {
      this._description = description;
      this._instruction = instruction;
      this._isTerminating = isTerminating;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this._instruction(il);
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Load->: {0}", (object) this);
      this._instruction(il);
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    public override void StoreValue(TracingILGenerator il)
    {
      il.TraceWriteLine("// Stor->: {0}", (object) this);
      this._instruction(il);
      il.TraceWriteLine("// ->Stor: {0}", (object) this);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Instruction[{0}]: {1}", (object) this.ContextType, (object) this._description);
    }
  }
}
