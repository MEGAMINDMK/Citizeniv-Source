// Decompiled with JetBrains decompiler
// Type: MsgPack.CollectionDebuggerProxy`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace MsgPack
{
  internal sealed class CollectionDebuggerProxy<T>
  {
    private readonly ICollection<T> _collection;

    public CollectionDebuggerProxy(ICollection<T> target)
    {
      this._collection = target;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items
    {
      get
      {
        if (this._collection == null)
          return (T[]) null;
        T[] array = new T[this._collection.Count];
        this._collection.CopyTo(array, 0);
        return array;
      }
    }
  }
}
