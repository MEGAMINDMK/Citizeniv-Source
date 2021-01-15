// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._MessagePackObject
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _MessagePackObject
  {
    public static readonly PropertyInfo IsNil = FromExpression.ToProperty<MessagePackObject, bool>((Expression<Func<MessagePackObject, bool>>) (mpo => mpo.IsNil));
  }
}
