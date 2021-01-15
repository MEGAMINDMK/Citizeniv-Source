// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._Packer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _Packer
  {
    public static readonly MethodInfo PackArrayHeader = FromExpression.ToMethod<Packer, int, Packer>((Expression<Func<Packer, int, Packer>>) ((packer, count) => packer.PackArrayHeader(count)));
    public static readonly MethodInfo PackMapHeader = FromExpression.ToMethod<Packer, int, Packer>((Expression<Func<Packer, int, Packer>>) ((packer, count) => packer.PackMapHeader(count)));
    public static readonly MethodInfo PackNull = FromExpression.ToMethod<Packer, Packer>((Expression<Func<Packer, Packer>>) (packer => packer.PackNull()));
  }
}
