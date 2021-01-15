// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.StatementExpressionILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class StatementExpressionILConstruct : ContextfulILConstruct
  {
    private bool _isBound;
    private readonly ILConstruct _binding;
    private readonly ILConstruct _expression;

    public StatementExpressionILConstruct(ILConstruct binding, ILConstruct expression)
      : base(expression.ContextType)
    {
      this._binding = binding;
      this._expression = expression;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this.Evaluate(il, false);
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Load->: {0}", (object) this);
      this.Evaluate(il, shouldBeAddress);
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    private void Evaluate(TracingILGenerator il, bool shouldBeAddress)
    {
      if (!this._isBound)
      {
        this._binding.Evaluate(il);
        this._isBound = true;
      }
      this._expression.LoadValue(il, shouldBeAddress);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bind[{0}]: {1} context: {2}", (object) this.ContextType, (object) this._binding, (object) this._expression);
    }
  }
}
