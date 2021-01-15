// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._DictionaryEntry
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _DictionaryEntry
  {
    public static readonly PropertyInfo Key = FromExpression.ToProperty<DictionaryEntry, object>((Expression<Func<DictionaryEntry, object>>) (entry => entry.Key));
    public static readonly PropertyInfo Value = FromExpression.ToProperty<DictionaryEntry, object>((Expression<Func<DictionaryEntry, object>>) (entry => entry.Value));
  }
}
