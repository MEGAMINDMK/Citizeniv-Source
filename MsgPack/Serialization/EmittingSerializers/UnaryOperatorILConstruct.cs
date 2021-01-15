// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.UnaryOperatorILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class UnaryOperatorILConstruct : ContextfulILConstruct
  {
    private readonly string _operator;
    private readonly ILConstruct _input;
    private readonly Action<TracingILGenerator, ILConstruct> _operation;
    private readonly Action<TracingILGenerator, ILConstruct, Label> _branchOperation;

    public UnaryOperatorILConstruct(
      string @operator,
      ILConstruct input,
      Action<TracingILGenerator, ILConstruct> operation)
      : this(@operator, input, operation, (Action<TracingILGenerator, ILConstruct, Label>) ((il, i, @else) => UnaryOperatorILConstruct.BranchWithOperationResult(i, operation, il, @else)))
    {
    }

    public UnaryOperatorILConstruct(
      string @operator,
      ILConstruct input,
      Action<TracingILGenerator, ILConstruct> operation,
      Action<TracingILGenerator, ILConstruct, Label> branchOperation)
      : base(input.ContextType)
    {
      this._operator = @operator;
      this._input = input;
      this._operation = operation;
      this._branchOperation = branchOperation;
    }

    private static void BranchWithOperationResult(
      ILConstruct input,
      Action<TracingILGenerator, ILConstruct> operation,
      TracingILGenerator il,
      Label @else)
    {
      operation(il, input);
      il.EmitBrfalse(@else);
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this._operation(il, this._input);
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Stor->: {0}", (object) this);
      this._operation(il, this._input);
      il.TraceWriteLine("// ->Stor: {0}", (object) this);
    }

    protected override void BranchCore(TracingILGenerator il, Label @else)
    {
      this._branchOperation(il, this._input, @else);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UnaryOperator[{0}]: ({1} {2})", (object) this.ContextType, (object) this._operator, (object) this._input);
    }
  }
}
