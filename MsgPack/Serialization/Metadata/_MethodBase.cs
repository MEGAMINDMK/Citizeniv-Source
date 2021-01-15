// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._MethodBase
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _MethodBase
  {
    public static readonly MethodInfo GetMethodFromHandle = FromExpression.ToMethod<RuntimeMethodHandle, RuntimeTypeHandle, MethodBase>((Expression<Func<RuntimeMethodHandle, RuntimeTypeHandle, MethodBase>>) ((handle, declaringType) => MethodBase.GetMethodFromHandle(handle, declaringType)));
    public static readonly MethodInfo Invoke_2 = FromExpression.ToMethod<MethodBase, object, object[], object>((Expression<Func<MethodBase, object, object[], object>>) ((@this, obj, parameters) => @this.Invoke(obj, parameters)));
  }
}
