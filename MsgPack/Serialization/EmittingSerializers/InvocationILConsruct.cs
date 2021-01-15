// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.InvocationILConsruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class InvocationILConsruct : ContextfulILConstruct
  {
    private readonly ILConstruct _target;
    private readonly MethodBase _method;
    private readonly IEnumerable<ILConstruct> _arguments;

    public InvocationILConsruct(
      MethodInfo method,
      ILConstruct target,
      IEnumerable<ILConstruct> arguments)
      : base(method.ReturnType)
    {
      if (method.IsStatic)
      {
        if (target != null)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "target must be null for static method '{0}'", (object) method));
      }
      else if (target == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "target must not be null for instance method '{0}'", (object) method));
      this._method = (MethodBase) method;
      this._target = target;
      this._arguments = arguments;
    }

    public InvocationILConsruct(
      ConstructorInfo ctor,
      ILConstruct target,
      IEnumerable<ILConstruct> arguments)
      : base(ctor.DeclaringType)
    {
      if (ctor.DeclaringType.GetIsValueType() && target == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "target must not be null for expression type constructor '{0}'", (object) ctor));
      this._method = (MethodBase) ctor;
      this._target = target;
      this._arguments = arguments;
    }

    public override void Evaluate(TracingILGenerator il)
    {
      il.TraceWriteLine("// Eval->: {0}", (object) this);
      this.Invoke(il);
      il.TraceWriteLine("// ->Eval: {0}", (object) this);
    }

    public override void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      il.TraceWriteLine("// Load->: {0}", (object) this);
      this.Invoke(il);
      il.TraceWriteLine("// ->Load: {0}", (object) this);
    }

    public override void StoreValue(TracingILGenerator il)
    {
      il.TraceWriteLine("// Stor->: {0}", (object) this);
      this.Invoke(il);
      il.TraceWriteLine("// ->Stor: {0}", (object) this);
    }

    private void Invoke(TracingILGenerator il)
    {
      ConstructorInfo method;
      if ((method = this._method as ConstructorInfo) != (ConstructorInfo) null)
      {
        if (method.DeclaringType.GetIsValueType())
        {
          this._target.LoadValue(il, true);
          foreach (ILConstruct ilConstruct in this._arguments)
            ilConstruct.LoadValue(il, false);
          il.EmitCallConstructor(method);
          this._target.LoadValue(il, false);
        }
        else
        {
          foreach (ILConstruct ilConstruct in this._arguments)
            ilConstruct.LoadValue(il, false);
          il.EmitNewobj(method);
        }
      }
      else
      {
        if (!this._method.IsStatic)
          this._target.LoadValue(il, this._target.ContextType.GetIsValueType());
        foreach (ILConstruct ilConstruct in this._arguments)
          ilConstruct.LoadValue(il, false);
        if (this._method.IsStatic || this._target.ContextType.GetIsValueType())
          il.EmitCall(this._method as MethodInfo);
        else
          il.EmitCallvirt(this._method as MethodInfo);
      }
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invoke[{0}]: {1}", (object) this.ContextType, (object) this._method);
    }
  }
}
