// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.BinaryOperatorILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class BinaryOperatorILConstruct : ContextfulILConstruct
  {
    private readonly string _operator;
    private readonly ILConstruct _left;
    private readonly ILConstruct _right;
    private readonly Action<TracingILGenerator, ILConstruct, ILConstruct> _operation;
    private readonly Action<TracingILGenerator, ILConstruct, ILConstruct, Label> _branchOperation;

    public BinaryOperatorILConstruct(
      string @operator,
      Type resultType,
      ILConstruct left,
      ILConstruct right,
      Action<TracingILGenerator, ILConstruct, ILConstruct> operation,
      Action<TracingILGenerator, ILConstruct, ILConstruct, Label> branchOperation)
      : base(resultType)
    {
      ILConstruct.ValidateContextTypeMatch(left, right);
      this._operator = @operator;
      this._left = left;
      this._right = right;
      this._operation = operation;
      this._branchOperation = branchOperation;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this._operation(il, this._left, this._right);
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Load->: {0}", (object) this);
      this._operation(il, this._left, this._right);
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    protected override void BranchCore(TracingILGenerator il, Label @else)
    {
      this._branchOperation(il, this._left, this._right, @else);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BinaryOperator[{0}]: ({2} {1} {3})", (object) this.ContextType, (object) this._operator, (object) this._left, (object) this._right);
    }
  }
}
