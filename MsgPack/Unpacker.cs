// Decompiled with JetBrains decompiler
// Type: MsgPack.Unpacker
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MsgPack
{
  public abstract class Unpacker : IEnumerable<MessagePackObject>, IEnumerable, IDisposable
  {
    private Unpacker.UnpackerMode _mode;
    private bool _isSubtreeReading;

    [Obsolete("Consumer should not use this property. Query LastReadData instead.")]
    public abstract MessagePackObject? Data { get; protected set; }

    public virtual MessagePackObject LastReadData
    {
      get
      {
        return this.Data.GetValueOrDefault();
      }
      protected set
      {
        this.Data = new MessagePackObject?(value);
      }
    }

    public abstract bool IsArrayHeader { get; }

    public abstract bool IsMapHeader { get; }

    public virtual bool IsCollectionHeader
    {
      get
      {
        return this.IsArrayHeader || this.IsMapHeader;
      }
    }

    public abstract long ItemsCount { get; }

    private void VerifyMode(Unpacker.UnpackerMode mode)
    {
      this.VerifyIsNotDisposed();
      if (this._mode == Unpacker.UnpackerMode.Unknown)
      {
        this._mode = mode;
      }
      else
      {
        if (this._mode == mode)
          return;
        this.ThrowInvalidModeException();
      }
    }

    private void VerifyIsNotDisposed()
    {
      if (this._mode != Unpacker.UnpackerMode.Disposed)
        return;
      this.ThrowObjectDisposedException();
    }

    private void ThrowObjectDisposedException()
    {
      throw new ObjectDisposedException(this.GetType().FullName);
    }

    private void ThrowInvalidModeException()
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Reader is in '{0}' mode.", (object) this._mode));
    }

    protected virtual Stream UnderlyingStream
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public static Unpacker Create(Stream stream)
    {
      return Unpacker.Create(stream, true);
    }

    public static Unpacker Create(Stream stream, bool ownsStream)
    {
      return (Unpacker) new ItemsUnpacker(stream, ownsStream);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public Unpacker ReadSubtree()
    {
      if (!this.IsCollectionHeader)
        Unpacker.ThrowCannotBeSubtreeModeException();
      if (this._isSubtreeReading)
        Unpacker.ThrowInSubtreeModeException();
      Unpacker unpacker = this.ReadSubtreeCore();
      this._isSubtreeReading = !object.ReferenceEquals((object) unpacker, (object) this);
      return unpacker;
    }

    private static void ThrowCannotBeSubtreeModeException()
    {
      throw new InvalidOperationException("Unpacker does not locate on array nor map header.");
    }

    private static void ThrowInSubtreeModeException()
    {
      throw new InvalidOperationException("Unpacker is in 'Subtree' mode.");
    }

    protected abstract Unpacker ReadSubtreeCore();

    protected internal virtual void EndReadSubtree()
    {
      this._isSubtreeReading = false;
      this.SetStable();
    }

    public bool Read()
    {
      this.EnsureNotInSubtreeMode();
      bool flag = this.ReadCore();
      if (flag && !this.IsCollectionHeader)
        this.SetStable();
      return flag;
    }

    internal void EnsureNotInSubtreeMode()
    {
      this.VerifyMode(Unpacker.UnpackerMode.Streaming);
      if (!this._isSubtreeReading)
        return;
      Unpacker.ThrowInSubtreeModeException();
    }

    private void SetStable()
    {
      this._mode = Unpacker.UnpackerMode.Unknown;
    }

    protected abstract bool ReadCore();

    public IEnumerator<MessagePackObject> GetEnumerator()
    {
      this.VerifyMode(Unpacker.UnpackerMode.Enumerating);
      while (this.ReadCore())
      {
        yield return this.LastReadData;
        this.VerifyMode(Unpacker.UnpackerMode.Enumerating);
      }
      this.SetStable();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public long? Skip()
    {
      this.VerifyIsNotDisposed();
      if (this._mode == Unpacker.UnpackerMode.Enumerating)
        this.ThrowInvalidModeException();
      if (this._isSubtreeReading)
        Unpacker.ThrowInSubtreeModeException();
      this._mode = Unpacker.UnpackerMode.Skipping;
      long? nullable = this.SkipCore();
      if (nullable.HasValue)
        this.SetStable();
      return nullable;
    }

    protected abstract long? SkipCore();

    public MessagePackObject? ReadItem()
    {
      if (!this.Read())
        return new MessagePackObject?();
      this.UnpackSubtree();
      return this.Data;
    }

    public MessagePackObject ReadItemData()
    {
      if (!this.Read())
        this.ThrowEofException();
      return this.UnpackSubtreeData();
    }

    internal virtual void ThrowEofException()
    {
      throw new InvalidMessagePackStreamException("Stream unexpectedly ends.");
    }

    public MessagePackObject? UnpackSubtree()
    {
      MessagePackObject result;
      if (!this.UnpackSubtreeDataCore(out result))
        return new MessagePackObject?();
      this.LastReadData = result;
      return new MessagePackObject?(result);
    }

    public MessagePackObject UnpackSubtreeData()
    {
      MessagePackObject result;
      if (!this.UnpackSubtreeDataCore(out result))
        return this.LastReadData;
      this.LastReadData = result;
      return result;
    }

    internal bool UnpackSubtreeDataCore(out MessagePackObject result)
    {
      if (this.IsArrayHeader)
      {
        MessagePackObject[] messagePackObjectArray = new MessagePackObject[checked ((int) this.LastReadData.AsUInt32())];
        using (Unpacker unpacker = this.ReadSubtree())
        {
          for (int index = 0; index < messagePackObjectArray.Length; ++index)
            messagePackObjectArray[index] = unpacker.ReadItemData();
        }
        result = new MessagePackObject((IList<MessagePackObject>) messagePackObjectArray, true);
        return true;
      }
      if (this.IsMapHeader)
      {
        int initialCapacity = checked ((int) this.LastReadData.AsUInt32());
        MessagePackObjectDictionary objectDictionary = new MessagePackObjectDictionary(initialCapacity);
        using (Unpacker unpacker = this.ReadSubtree())
        {
          for (int index = 0; index < initialCapacity; ++index)
          {
            MessagePackObject key = unpacker.ReadItemData();
            MessagePackObject messagePackObject = unpacker.ReadItemData();
            objectDictionary.Add(key, messagePackObject);
          }
        }
        result = new MessagePackObject(objectDictionary, true);
        return true;
      }
      result = new MessagePackObject();
      return false;
    }

    [CLSCompliant(false)]
    public virtual bool ReadBoolean(out bool result)
    {
      if (!this.Read())
      {
        result = false;
        return false;
      }
      result = this.LastReadData.AsBoolean();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableBoolean(out bool? result)
    {
      if (!this.Read())
      {
        result = new bool?();
        return false;
      }
      result = this.LastReadData.IsNil ? new bool?() : new bool?(this.LastReadData.AsBoolean());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadByte(out byte result)
    {
      if (!this.Read())
      {
        result = (byte) 0;
        return false;
      }
      result = this.LastReadData.AsByte();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableByte(out byte? result)
    {
      if (!this.Read())
      {
        result = new byte?();
        return false;
      }
      result = this.LastReadData.IsNil ? new byte?() : new byte?(this.LastReadData.AsByte());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadSByte(out sbyte result)
    {
      if (!this.Read())
      {
        result = (sbyte) 0;
        return false;
      }
      result = this.LastReadData.AsSByte();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableSByte(out sbyte? result)
    {
      if (!this.Read())
      {
        result = new sbyte?();
        return false;
      }
      result = this.LastReadData.IsNil ? new sbyte?() : new sbyte?(this.LastReadData.AsSByte());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadInt16(out short result)
    {
      if (!this.Read())
      {
        result = (short) 0;
        return false;
      }
      result = this.LastReadData.AsInt16();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableInt16(out short? result)
    {
      if (!this.Read())
      {
        result = new short?();
        return false;
      }
      result = this.LastReadData.IsNil ? new short?() : new short?(this.LastReadData.AsInt16());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadUInt16(out ushort result)
    {
      if (!this.Read())
      {
        result = (ushort) 0;
        return false;
      }
      result = this.LastReadData.AsUInt16();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableUInt16(out ushort? result)
    {
      if (!this.Read())
      {
        result = new ushort?();
        return false;
      }
      result = this.LastReadData.IsNil ? new ushort?() : new ushort?(this.LastReadData.AsUInt16());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadInt32(out int result)
    {
      if (!this.Read())
      {
        result = 0;
        return false;
      }
      result = this.LastReadData.AsInt32();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableInt32(out int? result)
    {
      if (!this.Read())
      {
        result = new int?();
        return false;
      }
      result = this.LastReadData.IsNil ? new int?() : new int?(this.LastReadData.AsInt32());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadUInt32(out uint result)
    {
      if (!this.Read())
      {
        result = 0U;
        return false;
      }
      result = this.LastReadData.AsUInt32();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableUInt32(out uint? result)
    {
      if (!this.Read())
      {
        result = new uint?();
        return false;
      }
      result = this.LastReadData.IsNil ? new uint?() : new uint?(this.LastReadData.AsUInt32());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadInt64(out long result)
    {
      if (!this.Read())
      {
        result = 0L;
        return false;
      }
      result = this.LastReadData.AsInt64();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableInt64(out long? result)
    {
      if (!this.Read())
      {
        result = new long?();
        return false;
      }
      result = this.LastReadData.IsNil ? new long?() : new long?(this.LastReadData.AsInt64());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadUInt64(out ulong result)
    {
      if (!this.Read())
      {
        result = 0UL;
        return false;
      }
      result = this.LastReadData.AsUInt64();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableUInt64(out ulong? result)
    {
      if (!this.Read())
      {
        result = new ulong?();
        return false;
      }
      result = this.LastReadData.IsNil ? new ulong?() : new ulong?(this.LastReadData.AsUInt64());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadSingle(out float result)
    {
      if (!this.Read())
      {
        result = 0.0f;
        return false;
      }
      result = this.LastReadData.AsSingle();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableSingle(out float? result)
    {
      if (!this.Read())
      {
        result = new float?();
        return false;
      }
      result = this.LastReadData.IsNil ? new float?() : new float?(this.LastReadData.AsSingle());
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadDouble(out double result)
    {
      if (!this.Read())
      {
        result = 0.0;
        return false;
      }
      result = this.LastReadData.AsDouble();
      return true;
    }

    [CLSCompliant(false)]
    public virtual bool ReadNullableDouble(out double? result)
    {
      if (!this.Read())
      {
        result = new double?();
        return false;
      }
      result = this.LastReadData.IsNil ? new double?() : new double?(this.LastReadData.AsDouble());
      return true;
    }

    public virtual bool ReadMessagePackExtendedTypeObject(out MessagePackExtendedTypeObject result)
    {
      if (!this.Read())
      {
        result = new MessagePackExtendedTypeObject();
        return false;
      }
      result = this.LastReadData.AsMessagePackExtendedTypeObject();
      return true;
    }

    public virtual bool ReadNullableMessagePackExtendedTypeObject(
      out MessagePackExtendedTypeObject? result)
    {
      if (!this.Read())
      {
        result = new MessagePackExtendedTypeObject?();
        return false;
      }
      result = this.LastReadData.IsNil ? new MessagePackExtendedTypeObject?() : new MessagePackExtendedTypeObject?(this.LastReadData.AsMessagePackExtendedTypeObject());
      return true;
    }

    public virtual bool ReadArrayLength(out long result)
    {
      if (!this.Read())
      {
        result = 0L;
        return false;
      }
      if (!this.IsArrayHeader)
        throw new MessageTypeException("Not in map header.");
      result = this.LastReadData.AsInt64();
      return true;
    }

    public virtual bool ReadMapLength(out long result)
    {
      if (!this.Read())
      {
        result = 0L;
        return false;
      }
      if (!this.IsMapHeader)
        throw new MessageTypeException("Not in map header.");
      result = this.LastReadData.AsInt64();
      return true;
    }

    public virtual bool ReadBinary(out byte[] result)
    {
      if (!this.Read())
      {
        result = (byte[]) null;
        return false;
      }
      result = this.LastReadData.AsBinary();
      return true;
    }

    public virtual bool ReadString(out string result)
    {
      if (!this.Read())
      {
        result = (string) null;
        return false;
      }
      result = this.LastReadData.AsString();
      return true;
    }

    public virtual bool ReadObject(out MessagePackObject result)
    {
      if (!this.Read())
      {
        result = new MessagePackObject();
        return false;
      }
      result = this.LastReadData;
      return true;
    }

    private enum UnpackerMode
    {
      Unknown,
      Skipping,
      Streaming,
      Enumerating,
      Disposed,
    }
  }
}
