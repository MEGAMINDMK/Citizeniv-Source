// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExtTypeCodeMapping
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MsgPack.Serialization
{
  public sealed class ExtTypeCodeMapping : IEnumerable<KeyValuePair<string, byte>>, IEnumerable
  {
    private readonly object _syncRoot;
    private readonly Dictionary<string, byte> _index;
    private readonly Dictionary<byte, string> _types;

    public byte this[string name]
    {
      get
      {
        ExtTypeCodeMapping.ValidateName(name);
        lock (this._syncRoot)
        {
          byte num;
          if (!this._index.TryGetValue(name, out num))
            throw new KeyNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Ext type '{0}' is not found.", (object) name));
          return num;
        }
      }
    }

    internal ExtTypeCodeMapping()
    {
      this._syncRoot = new object();
      this._index = new Dictionary<string, byte>(2);
      this._types = new Dictionary<byte, string>(2);
      this.Add(KnownExtTypeName.MultidimensionalArray, KnownExtTypeCode.MultidimensionalArray);
    }

    public bool Add(string name, byte typeCode)
    {
      ExtTypeCodeMapping.ValidateName(name);
      ExtTypeCodeMapping.ValidateTypeCode(typeCode);
      lock (this._syncRoot)
      {
        try
        {
          this._types.Add(typeCode, name);
        }
        catch (ArgumentException ex)
        {
          return false;
        }
        this._index[name] = typeCode;
        return true;
      }
    }

    public bool Remove(string name)
    {
      ExtTypeCodeMapping.ValidateName(name);
      lock (this._syncRoot)
      {
        byte typeCode;
        if (!this._index.TryGetValue(name, out typeCode))
          return false;
        this.RemoveCore(name, typeCode);
        return true;
      }
    }

    public bool Remove(byte typeCode)
    {
      ExtTypeCodeMapping.ValidateTypeCode(typeCode);
      lock (this._syncRoot)
      {
        string name;
        if (!this._types.TryGetValue(typeCode, out name))
          return false;
        this.RemoveCore(name, typeCode);
        return true;
      }
    }

    private void RemoveCore(string name, byte typeCode)
    {
      this._types.Remove(typeCode);
      this._index.Remove(name);
    }

    public void Clear()
    {
      lock (this._syncRoot)
      {
        this._types.Clear();
        this._index.Clear();
      }
    }

    public IEnumerator<KeyValuePair<string, byte>> GetEnumerator()
    {
      List<KeyValuePair<string, byte>> list;
      lock (this._syncRoot)
        list = this._index.ToList<KeyValuePair<string, byte>>();
      return (IEnumerator<KeyValuePair<string, byte>>) list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private static void ValidateName(string name)
    {
      switch (name)
      {
        case "":
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "A parameter cannot be empty."), nameof (name));
        case null:
          throw new ArgumentNullException(nameof (name));
      }
    }

    private static void ValidateTypeCode(byte typeCode)
    {
      if (typeCode > (byte) 127)
        throw new ArgumentOutOfRangeException(nameof (typeCode), "Ext type code must be between 0 and 0x7F.");
    }
  }
}
