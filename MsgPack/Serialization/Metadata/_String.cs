// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._String
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _String
  {
    public static readonly MethodInfo op_Equality = FromExpression.ToOperator<string, string, bool>((Expression<Func<string, string, bool>>) ((left, right) => left == right));
    public static readonly MethodInfo Format_P = FromExpression.ToMethod<IFormatProvider, string, object[], string>((Expression<Func<IFormatProvider, string, object[], string>>) ((provider, format, args) => string.Format(provider, format, args)));
  }
}
