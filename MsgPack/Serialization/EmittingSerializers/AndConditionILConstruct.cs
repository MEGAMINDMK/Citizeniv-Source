// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.AndConditionILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class AndConditionILConstruct : ILConstruct
  {
    private readonly IList<ILConstruct> _expressions;

    public AndConditionILConstruct(IList<ILConstruct> expressions)
      : base(typeof (bool))
    {
      if (expressions.Count == 0)
        throw new ArgumentException("Empty expressions.", nameof (expressions));
      if (expressions.Any<ILConstruct>((Func<ILConstruct, bool>) (c => c.ContextType != typeof (bool))))
        throw new ArgumentException("An argument expressions cannot contains non boolean expression.", nameof (expressions));
      this._expressions = expressions;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this.EvaluateCore(il);
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Load->: {0}", (object) this);
      this.EvaluateCore(il);
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    private void EvaluateCore(TracingILGenerator il)
    {
      for (int index = 0; index < this._expressions.Count; ++index)
      {
        this._expressions[index].LoadValue(il, false);
        if (index > 0)
          il.EmitAnd();
      }
    }

    public override void Branch(TracingILGenerator il, Label @else)
    {
      il.TraceWriteLine("// Brnc->: {0}", (object) this);
      foreach (ILConstruct expression in (IEnumerable<ILConstruct>) this._expressions)
      {
        expression.LoadValue(il, false);
        il.EmitBrfalse(@else);
      }
      il.TraceWriteLine("// ->Brnc: {0}", (object) this);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "And[{0}]: ({1})", (object) this.ContextType, (object) string.Join(", ", this._expressions.Select<ILConstruct, string>((Func<ILConstruct, string>) (e => e.ToString())).ToArray<string>()));
    }
  }
}
