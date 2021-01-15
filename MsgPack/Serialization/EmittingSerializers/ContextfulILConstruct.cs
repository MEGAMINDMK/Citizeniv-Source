// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.ContextfulILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Globalization;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal abstract class ContextfulILConstruct : ILConstruct
  {
    protected ContextfulILConstruct(Type contextType)
      : base(contextType)
    {
    }

    public override sealed void Branch(TracingILGenerator il, Label @else)
    {
      il.TraceWriteLine("// Brnc->: {0}", (object) this);
      if (this.ContextType != typeof (bool))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot branch with non boolean type '{0}'.", (object) this.ContextType));
      this.BranchCore(il, @else);
      il.TraceWriteLine("// ->Brnc: {0}", (object) this);
    }

    protected virtual void BranchCore(TracingILGenerator il, Label @else)
    {
      this.LoadValue(il, false);
      il.EmitBrfalse(@else);
    }
  }
}
