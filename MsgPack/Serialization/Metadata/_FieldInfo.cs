// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._FieldInfo
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _FieldInfo
  {
    public static readonly MethodInfo GetFieldFromHandle = FromExpression.ToMethod<RuntimeFieldHandle, RuntimeTypeHandle, FieldInfo>((Expression<Func<RuntimeFieldHandle, RuntimeTypeHandle, FieldInfo>>) ((handle, declaringType) => FieldInfo.GetFieldFromHandle(handle, declaringType)));
    public static readonly MethodInfo GetValue = FromExpression.ToMethod<FieldInfo, object, object>((Expression<Func<FieldInfo, object, object>>) ((@this, obj) => @this.GetValue(obj)));
    public static readonly MethodInfo SetValue = FromExpression.ToMethod<FieldInfo, object, object>((Expression<Action<FieldInfo, object, object>>) ((@this, obj, value) => @this.SetValue(obj, value)));
  }
}
