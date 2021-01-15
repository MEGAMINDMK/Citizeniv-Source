// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._Unpacker
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _Unpacker
  {
    public static readonly MethodInfo Read = FromExpression.ToMethod<Unpacker, bool>((Expression<Func<Unpacker, bool>>) (unpacker => unpacker.Read()));
    public static readonly MethodInfo ReadSubtree = FromExpression.ToMethod<Unpacker, Unpacker>((Expression<Func<Unpacker, Unpacker>>) (unpacker => unpacker.ReadSubtree()));
    public static readonly MethodInfo UnpackSubtreeData = FromExpression.ToMethod<Unpacker, MessagePackObject>((Expression<Func<Unpacker, MessagePackObject>>) (unpacker => unpacker.UnpackSubtreeData()));
    public static readonly PropertyInfo LastReadData = FromExpression.ToProperty<Unpacker, MessagePackObject>((Expression<Func<Unpacker, MessagePackObject>>) (unpacker => unpacker.LastReadData));
    public static readonly PropertyInfo ItemsCount = FromExpression.ToProperty<Unpacker, long>((Expression<Func<Unpacker, long>>) (unpacker => unpacker.ItemsCount));
    public static readonly PropertyInfo IsArrayHeader = FromExpression.ToProperty<Unpacker, bool>((Expression<Func<Unpacker, bool>>) (unpacker => unpacker.IsArrayHeader));
    public static readonly PropertyInfo IsMapHeader = FromExpression.ToProperty<Unpacker, bool>((Expression<Func<Unpacker, bool>>) (unpacker => unpacker.IsMapHeader));
  }
}
