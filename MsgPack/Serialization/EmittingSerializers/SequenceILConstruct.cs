// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.SequenceILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class SequenceILConstruct : ILConstruct
  {
    private readonly ILConstruct[] _statements;

    public SequenceILConstruct(Type contextType, IEnumerable<ILConstruct> statements)
      : base(contextType)
    {
      this._statements = statements.ToArray<ILConstruct>();
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      foreach (ILConstruct statement in this._statements)
        statement.Evaluate(il);
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      if (this._statements.Length == 0)
      {
        base.LoadValue(il, shouldBeAddress);
      }
      else
      {
        il.TraceWriteLine("// Eval(Load)->: {0}", (object) this);
        for (int index = 0; index < this._statements.Length - 1; ++index)
          this._statements[index].Evaluate(il);
        ((IEnumerable<ILConstruct>) this._statements).Last<ILConstruct>().LoadValue(il, shouldBeAddress);
        il.TraceWriteLine("// ->Eval(Load): {0}", (object) this);
      }
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Sequence[{0}]", (object) this._statements.Length);
    }
  }
}
