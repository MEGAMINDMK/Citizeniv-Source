// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.ConditionalILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class ConditionalILConstruct : ILConstruct
  {
    private readonly ILConstruct _condition;
    private readonly ILConstruct _thenExpression;
    private readonly ILConstruct _elseExpression;

    public ConditionalILConstruct(
      ILConstruct condition,
      ILConstruct thenExpression,
      ILConstruct elseExpression)
      : base(thenExpression.ContextType)
    {
      if (condition.ContextType != typeof (bool))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "condition must be boolean: {0}", (object) condition), nameof (condition));
      if (elseExpression != null && elseExpression.ContextType != thenExpression.ContextType)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "elseExpression type must be '{0}' but '{1}':{2}", (object) thenExpression.ContextType, (object) elseExpression.ContextType, (object) elseExpression), nameof (elseExpression));
      this._condition = condition;
      this._thenExpression = thenExpression;
      this._elseExpression = elseExpression;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this.DoConditionalInstruction(il, (Action) (() => this._thenExpression.Evaluate(il)), (Action) (() => this._elseExpression.Evaluate(il)));
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Load->: {0}", (object) this);
      this.DoConditionalInstruction(il, (Action) (() => this._thenExpression.LoadValue(il, shouldBeAddress)), (Action) (() => this._elseExpression.LoadValue(il, shouldBeAddress)));
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    public override void StoreValue(TracingILGenerator il)
    {
      il.TraceWriteLine("// Stor->: {0}", (object) this);
      this.DoConditionalInstruction(il, (Action) (() => this._thenExpression.StoreValue(il)), (Action) (() => this._elseExpression.StoreValue(il)));
      il.TraceWriteLine("// ->Stor: {0}", (object) this);
    }

    private void DoConditionalInstruction(TracingILGenerator il, Action onThen, Action onElse)
    {
      if (this._elseExpression != null)
      {
        Label label1 = il.DefineLabel("ELSE");
        Label label2 = il.DefineLabel("END_IF");
        this._condition.Branch(il, label1);
        onThen();
        if (!this._thenExpression.IsTerminating)
          il.EmitBr(label2);
        il.MarkLabel(label1);
        onElse();
        il.MarkLabel(label2);
      }
      else
      {
        Label label = il.DefineLabel("END_IF");
        this._condition.Branch(il, label);
        onThen();
        il.MarkLabel(label);
      }
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Condition[{0}]: ({1}) ? ({2}) : ({3})", (object) this.ContextType, (object) this._condition, (object) this._thenExpression, (object) this._elseExpression);
    }
  }
}
