// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.TypeKeyRepository
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;

namespace MsgPack.Serialization
{
  [SecuritySafeCritical]
  internal class TypeKeyRepository
  {
    private readonly ReaderWriterLockSlim _lock;
    private readonly Dictionary<RuntimeTypeHandle, object> _table;

    public TypeKeyRepository()
      : this((TypeKeyRepository) null)
    {
    }

    public TypeKeyRepository(TypeKeyRepository copiedFrom)
    {
      this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
      if (copiedFrom == null)
        this._table = new Dictionary<RuntimeTypeHandle, object>();
      else
        this._table = copiedFrom.GetClonedTable();
    }

    public TypeKeyRepository(Dictionary<RuntimeTypeHandle, object> table)
    {
      this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
      this._table = table;
    }

    [SecuritySafeCritical]
    private Dictionary<RuntimeTypeHandle, object> GetClonedTable()
    {
      bool flag = false;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          this._lock.EnterReadLock();
          flag = true;
        }
        return new Dictionary<RuntimeTypeHandle, object>((IDictionary<RuntimeTypeHandle, object>) this._table);
      }
      finally
      {
        if (flag)
          this._lock.ExitReadLock();
      }
    }

    public bool Get(Type type, out object matched, out object genericDefinitionMatched)
    {
      return this.GetCore(type, out matched, out genericDefinitionMatched);
    }

    [SecuritySafeCritical]
    private bool GetCore(Type type, out object matched, out object genericDefinitionMatched)
    {
      bool flag = false;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          this._lock.EnterReadLock();
          flag = true;
        }
        object obj;
        if (this._table.TryGetValue(type.TypeHandle, out obj))
        {
          matched = obj;
          genericDefinitionMatched = (object) null;
          return true;
        }
        if (type.GetIsGenericType() && this._table.TryGetValue(type.GetGenericTypeDefinition().TypeHandle, out obj))
        {
          matched = (object) null;
          genericDefinitionMatched = obj;
          return true;
        }
        matched = (object) null;
        genericDefinitionMatched = (object) null;
        return false;
      }
      finally
      {
        if (flag)
          this._lock.ExitReadLock();
      }
    }

    public bool Register(
      Type type,
      object entry,
      Type nullableType,
      object nullableValue,
      SerializerRegistrationOptions options)
    {
      return this.RegisterCore(type, entry, nullableType, nullableValue, options);
    }

    [SecuritySafeCritical]
    private bool RegisterCore(
      Type key,
      object value,
      Type nullableType,
      object nullableValue,
      SerializerRegistrationOptions options)
    {
      bool flag1 = (options & SerializerRegistrationOptions.AllowOverride) != SerializerRegistrationOptions.None;
      if (flag1 || !this.ContainsType(key, nullableType))
      {
        bool flag2 = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          RuntimeHelpers.PrepareConstrainedRegions();
          try
          {
          }
          finally
          {
            this._lock.EnterWriteLock();
            flag2 = true;
          }
          if (!flag1)
          {
            if (this.ContainsType(key, nullableType))
              goto label_13;
          }
          this._table[key.TypeHandle] = value;
          if (nullableValue != null)
            this._table[nullableType.TypeHandle] = nullableValue;
          return true;
        }
        finally
        {
          if (flag2)
            this._lock.ExitWriteLock();
        }
      }
label_13:
      return false;
    }

    private bool ContainsType(Type baseType, Type nullableType)
    {
      if (nullableType == (Type) null)
        return this._table.ContainsKey(baseType.TypeHandle);
      return this._table.ContainsKey(baseType.TypeHandle) && this._table.ContainsKey(nullableType.TypeHandle);
    }

    public bool Unregister(Type type)
    {
      return this.UnregisterCore(type);
    }

    [SecuritySafeCritical]
    private bool UnregisterCore(Type key)
    {
      if (!this._table.ContainsKey(key.TypeHandle))
        return false;
      bool flag = false;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          this._lock.EnterWriteLock();
          flag = true;
        }
        return this._table.Remove(key.TypeHandle);
      }
      finally
      {
        if (flag)
          this._lock.ExitWriteLock();
      }
    }

    [SecuritySafeCritical]
    internal bool Contains(Type type)
    {
      bool flag = false;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          this._lock.EnterReadLock();
          flag = true;
        }
        return this._table.ContainsKey(type.TypeHandle);
      }
      finally
      {
        if (flag)
          this._lock.ExitReadLock();
      }
    }
  }
}
