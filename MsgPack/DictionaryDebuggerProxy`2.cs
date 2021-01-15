// Decompiled with JetBrains decompiler
// Type: MsgPack.DictionaryDebuggerProxy`2
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace MsgPack
{
  internal sealed class DictionaryDebuggerProxy<TKey, TValue>
  {
    private readonly IDictionary<TKey, TValue> _dictionary;

    public DictionaryDebuggerProxy(IDictionary<TKey, TValue> target)
    {
      this._dictionary = target;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<TKey, TValue>[] Items
    {
      get
      {
        if (this._dictionary == null)
          return (KeyValuePair<TKey, TValue>[]) null;
        KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[this._dictionary.Count];
        this._dictionary.CopyTo(array, 0);
        return array;
      }
    }
  }
}
