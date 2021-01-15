// Decompiled with JetBrains decompiler
// Type: MsgPack.ItemsUnpacker
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MsgPack
{
  internal sealed class ItemsUnpacker : Unpacker
  {
    private readonly byte[] _scalarBuffer = new byte[8];
    private readonly bool _ownsStream;
    private readonly Stream _source;
    internal long InternalItemsCount;
    internal ItemsUnpacker.CollectionType InternalCollectionType;
    internal MessagePackObject InternalData;
    private long _offset;

    [Obsolete("Consumer should not use this property. Query LastReadData instead.")]
    public override MessagePackObject? Data
    {
      get
      {
        return new MessagePackObject?(this.InternalData);
      }
      protected set
      {
        this.InternalData = value.GetValueOrDefault();
      }
    }

    public override MessagePackObject LastReadData
    {
      get
      {
        return this.InternalData;
      }
      protected set
      {
        this.InternalData = value;
      }
    }

    public override bool IsArrayHeader
    {
      get
      {
        return this.InternalCollectionType == ItemsUnpacker.CollectionType.Array;
      }
    }

    public override bool IsMapHeader
    {
      get
      {
        return this.InternalCollectionType == ItemsUnpacker.CollectionType.Map;
      }
    }

    public override bool IsCollectionHeader
    {
      get
      {
        return this.InternalCollectionType != ItemsUnpacker.CollectionType.None;
      }
    }

    public override long ItemsCount
    {
      get
      {
        return this.InternalCollectionType == ItemsUnpacker.CollectionType.None ? 0L : this.InternalItemsCount;
      }
    }

    protected override Stream UnderlyingStream
    {
      get
      {
        return this._source;
      }
    }

    public ItemsUnpacker(Stream stream, bool ownsStream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      this._source = stream;
      this._ownsStream = ownsStream;
      this._offset = stream.CanSeek ? stream.Position : 0L;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this._ownsStream)
        this._source.Dispose();
      base.Dispose(disposing);
    }

    protected override bool ReadCore()
    {
      MessagePackObject result;
      if (!this.ReadSubtreeObject(out result))
        return false;
      this.InternalData = result;
      return true;
    }

    protected override Unpacker ReadSubtreeCore()
    {
      return (Unpacker) new SubtreeUnpacker(this);
    }

    internal bool ReadSubtreeItem()
    {
      return this.ReadCore();
    }

    internal long? SkipSubtreeItem()
    {
      return this.SkipCore();
    }

    private void ReadStrict(byte[] buffer, int size)
    {
      if (size == 0)
        return;
      long offset1 = this._offset;
      int count = size;
      int offset2 = 0;
      int num;
      do
      {
        num = this._source.Read(buffer, offset2, count);
        count -= num;
        offset2 += num;
      }
      while (num > 0 && count > 0);
      this._offset += (long) offset2;
      if (offset2 >= size)
        return;
      this.ThrowEofException((long) size, offset1);
    }

    private byte ReadByteStrict()
    {
      long offset = this._offset;
      int num = this._source.ReadByte();
      if (num < 0)
        this.ThrowEofException(offset, 1L);
      ++this._offset;
      return (byte) num;
    }

    internal override void ThrowEofException()
    {
      throw new InvalidMessagePackStreamException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, this._source.CanSeek ? "Stream unexpectedly ends. Cannot read object from stream. Current position is {0:#,0}." : "Stream unexpectedly ends. Cannot read object from stream. Current offset is {0:#,0}.", (object) this._offset));
    }

    private void ThrowEofException(long lastOffset, long reading)
    {
      throw new InvalidMessagePackStreamException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, this._source.CanSeek ? "Stream unexpectedly ends. Cannot read {0:#,0} bytes from stream at position {1:#,0}." : "Stream unexpectedly ends. Cannot read {0:#,0} bytes from stream at offset {1:#,0}.", (object) reading, (object) lastOffset));
    }

    public override bool ReadBoolean(out bool result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeBoolean(out result);
    }

    internal bool ReadSubtreeBoolean(out bool result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = false;
          return false;
        case ItemsUnpacker.ReadValueResult.Boolean:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = integral != 0L;
          return true;
        default:
          this.ThrowTypeException(typeof (bool), header);
          result = false;
          return false;
      }
    }

    public override bool ReadNullableBoolean(out bool? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableBoolean(out result);
    }

    internal bool ReadSubtreeNullableBoolean(out bool? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new bool?(false);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new bool?();
          return true;
        case ItemsUnpacker.ReadValueResult.Boolean:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new bool?(integral != 0L);
          return true;
        default:
          this.ThrowTypeException(typeof (bool), header);
          result = new bool?(false);
          return false;
      }
    }

    public override bool ReadByte(out byte result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeByte(out result);
    }

    internal bool ReadSubtreeByte(out byte result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = (byte) 0;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = checked ((byte) integral);
          return true;
        case ItemsUnpacker.ReadValueResult.Byte:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (byte) integral;
          return true;
        default:
          this.ThrowTypeException(typeof (byte), header);
          result = (byte) 0;
          return false;
      }
    }

    public override bool ReadNullableByte(out byte? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableByte(out result);
    }

    internal bool ReadSubtreeNullableByte(out byte? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new byte?((byte) 0);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new byte?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new byte?(checked ((byte) integral));
          return true;
        case ItemsUnpacker.ReadValueResult.Byte:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new byte?((byte) integral);
          return true;
        default:
          this.ThrowTypeException(typeof (byte), header);
          result = new byte?((byte) 0);
          return false;
      }
    }

    public override bool ReadSByte(out sbyte result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeSByte(out result);
    }

    internal bool ReadSubtreeSByte(out sbyte result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = (sbyte) 0;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (sbyte) integral;
          return true;
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = checked ((sbyte) integral);
          return true;
        default:
          this.ThrowTypeException(typeof (sbyte), header);
          result = (sbyte) 0;
          return false;
      }
    }

    public override bool ReadNullableSByte(out sbyte? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableSByte(out result);
    }

    internal bool ReadSubtreeNullableSByte(out sbyte? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new sbyte?((sbyte) 0);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new sbyte?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new sbyte?((sbyte) integral);
          return true;
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new sbyte?(checked ((sbyte) integral));
          return true;
        default:
          this.ThrowTypeException(typeof (sbyte), header);
          result = new sbyte?((sbyte) 0);
          return false;
      }
    }

    public override bool ReadInt16(out short result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeInt16(out result);
    }

    internal bool ReadSubtreeInt16(out short result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = (short) 0;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = checked ((short) integral);
          return true;
        case ItemsUnpacker.ReadValueResult.Int16:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (short) integral;
          return true;
        default:
          this.ThrowTypeException(typeof (short), header);
          result = (short) 0;
          return false;
      }
    }

    public override bool ReadNullableInt16(out short? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableInt16(out result);
    }

    internal bool ReadSubtreeNullableInt16(out short? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new short?((short) 0);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new short?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new short?(checked ((short) integral));
          return true;
        case ItemsUnpacker.ReadValueResult.Int16:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new short?((short) integral);
          return true;
        default:
          this.ThrowTypeException(typeof (short), header);
          result = new short?((short) 0);
          return false;
      }
    }

    public override bool ReadUInt16(out ushort result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeUInt16(out result);
    }

    internal bool ReadSubtreeUInt16(out ushort result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = (ushort) 0;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = checked ((ushort) integral);
          return true;
        case ItemsUnpacker.ReadValueResult.UInt16:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (ushort) integral;
          return true;
        default:
          this.ThrowTypeException(typeof (ushort), header);
          result = (ushort) 0;
          return false;
      }
    }

    public override bool ReadNullableUInt16(out ushort? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableUInt16(out result);
    }

    internal bool ReadSubtreeNullableUInt16(out ushort? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new ushort?((ushort) 0);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new ushort?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new ushort?(checked ((ushort) integral));
          return true;
        case ItemsUnpacker.ReadValueResult.UInt16:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new ushort?((ushort) integral);
          return true;
        default:
          this.ThrowTypeException(typeof (ushort), header);
          result = new ushort?((ushort) 0);
          return false;
      }
    }

    public override bool ReadInt32(out int result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeInt32(out result);
    }

    internal bool ReadSubtreeInt32(out int result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = checked ((int) integral);
          return true;
        case ItemsUnpacker.ReadValueResult.Int32:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (int) integral;
          return true;
        default:
          this.ThrowTypeException(typeof (int), header);
          result = 0;
          return false;
      }
    }

    public override bool ReadNullableInt32(out int? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableInt32(out result);
    }

    internal bool ReadSubtreeNullableInt32(out int? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new int?(0);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new int?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new int?(checked ((int) integral));
          return true;
        case ItemsUnpacker.ReadValueResult.Int32:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new int?((int) integral);
          return true;
        default:
          this.ThrowTypeException(typeof (int), header);
          result = new int?(0);
          return false;
      }
    }

    public override bool ReadUInt32(out uint result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeUInt32(out result);
    }

    internal bool ReadSubtreeUInt32(out uint result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0U;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = checked ((uint) integral);
          return true;
        case ItemsUnpacker.ReadValueResult.UInt32:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (uint) integral;
          return true;
        default:
          this.ThrowTypeException(typeof (uint), header);
          result = 0U;
          return false;
      }
    }

    public override bool ReadNullableUInt32(out uint? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableUInt32(out result);
    }

    internal bool ReadSubtreeNullableUInt32(out uint? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new uint?(0U);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new uint?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new uint?(checked ((uint) integral));
          return true;
        case ItemsUnpacker.ReadValueResult.UInt32:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new uint?((uint) integral);
          return true;
        default:
          this.ThrowTypeException(typeof (uint), header);
          result = new uint?(0U);
          return false;
      }
    }

    public override bool ReadInt64(out long result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeInt64(out result);
    }

    internal bool ReadSubtreeInt64(out long result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0L;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = integral;
          return true;
        case ItemsUnpacker.ReadValueResult.Int64:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = integral;
          return true;
        default:
          this.ThrowTypeException(typeof (long), header);
          result = 0L;
          return false;
      }
    }

    public override bool ReadNullableInt64(out long? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableInt64(out result);
    }

    internal bool ReadSubtreeNullableInt64(out long? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new long?(0L);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new long?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new long?(integral);
          return true;
        case ItemsUnpacker.ReadValueResult.Int64:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new long?(integral);
          return true;
        default:
          this.ThrowTypeException(typeof (long), header);
          result = new long?(0L);
          return false;
      }
    }

    public override bool ReadUInt64(out ulong result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeUInt64(out result);
    }

    internal bool ReadSubtreeUInt64(out ulong result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0UL;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = checked ((ulong) integral);
          return true;
        case ItemsUnpacker.ReadValueResult.UInt64:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (ulong) integral;
          return true;
        default:
          this.ThrowTypeException(typeof (ulong), header);
          result = 0UL;
          return false;
      }
    }

    public override bool ReadNullableUInt64(out ulong? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableUInt64(out result);
    }

    internal bool ReadSubtreeNullableUInt64(out ulong? result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new ulong?(0UL);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new ulong?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.Single:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new ulong?(checked ((ulong) integral));
          return true;
        case ItemsUnpacker.ReadValueResult.UInt64:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new ulong?((ulong) integral);
          return true;
        default:
          this.ThrowTypeException(typeof (ulong), header);
          result = new ulong?(0UL);
          return false;
      }
    }

    public override bool ReadSingle(out float result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeSingle(out result);
    }

    internal bool ReadSubtreeSingle(out float result)
    {
      byte header;
      float real32;
      switch (this.ReadValue(out header, out long _, out real32, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0.0f;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = real32;
          return true;
        case ItemsUnpacker.ReadValueResult.Single:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = real32;
          return true;
        default:
          this.ThrowTypeException(typeof (float), header);
          result = 0.0f;
          return false;
      }
    }

    public override bool ReadNullableSingle(out float? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableSingle(out result);
    }

    internal bool ReadSubtreeNullableSingle(out float? result)
    {
      byte header;
      float real32;
      switch (this.ReadValue(out header, out long _, out real32, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new float?(0.0f);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new float?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new float?(real32);
          return true;
        case ItemsUnpacker.ReadValueResult.Single:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new float?(real32);
          return true;
        default:
          this.ThrowTypeException(typeof (float), header);
          result = new float?(0.0f);
          return false;
      }
    }

    public override bool ReadDouble(out double result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeDouble(out result);
    }

    internal bool ReadSubtreeDouble(out double result)
    {
      byte header;
      double real64;
      switch (this.ReadValue(out header, out long _, out float _, out real64))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0.0;
          return false;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = real64;
          return true;
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = real64;
          return true;
        default:
          this.ThrowTypeException(typeof (double), header);
          result = 0.0;
          return false;
      }
    }

    public override bool ReadNullableDouble(out double? result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeNullableDouble(out result);
    }

    internal bool ReadSubtreeNullableDouble(out double? result)
    {
      byte header;
      double real64;
      switch (this.ReadValue(out header, out long _, out float _, out real64))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new double?(0.0);
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new double?();
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
        case ItemsUnpacker.ReadValueResult.Byte:
        case ItemsUnpacker.ReadValueResult.Int16:
        case ItemsUnpacker.ReadValueResult.UInt16:
        case ItemsUnpacker.ReadValueResult.Int32:
        case ItemsUnpacker.ReadValueResult.UInt32:
        case ItemsUnpacker.ReadValueResult.Int64:
        case ItemsUnpacker.ReadValueResult.UInt64:
        case ItemsUnpacker.ReadValueResult.Single:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new double?(real64);
          return true;
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = new double?(real64);
          return true;
        default:
          this.ThrowTypeException(typeof (double), header);
          result = new double?(0.0);
          return false;
      }
    }

    public override bool ReadBinary(out byte[] result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeBinary(out result);
    }

    internal bool ReadSubtreeBinary(out byte[] result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = (byte[]) null;
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (byte[]) null;
          return true;
        case ItemsUnpacker.ReadValueResult.String:
        case ItemsUnpacker.ReadValueResult.Binary:
          result = this.ReadBinaryCore(integral);
          return true;
        default:
          this.ThrowTypeException(typeof (byte[]), header);
          result = (byte[]) null;
          return false;
      }
    }

    public override bool ReadString(out string result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeString(out result);
    }

    internal bool ReadSubtreeString(out string result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = (string) null;
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (string) null;
          return true;
        case ItemsUnpacker.ReadValueResult.String:
        case ItemsUnpacker.ReadValueResult.Binary:
          result = this.ReadStringCore(integral);
          return true;
        default:
          this.ThrowTypeException(typeof (string), header);
          result = (string) null;
          return false;
      }
    }

    public override bool ReadObject(out MessagePackObject result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeObject(out result);
    }

    internal bool ReadSubtreeObject(out MessagePackObject result)
    {
      byte header;
      long integral;
      float real32;
      double real64;
      ItemsUnpacker.ReadValueResult type = this.ReadValue(out header, out integral, out real32, out real64);
      switch (type)
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new MessagePackObject();
          return false;
        case ItemsUnpacker.ReadValueResult.Nil:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = MessagePackObject.Nil;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Boolean:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (integral != 0L);
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.SByte:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (sbyte) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Byte:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (byte) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Int16:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (short) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.UInt16:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (ushort) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Int32:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (int) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.UInt32:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (uint) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Int64:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.UInt64:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) (ulong) integral;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Single:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) real32;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Double:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          result = (MessagePackObject) real64;
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.ArrayLength:
          result = (MessagePackObject) (uint) this.ReadArrayLengthCore(integral);
          return true;
        case ItemsUnpacker.ReadValueResult.MapLength:
          result = (MessagePackObject) (uint) this.ReadMapLengthCore(integral);
          return true;
        case ItemsUnpacker.ReadValueResult.String:
          result = new MessagePackObject(new MessagePackString(this.ReadBinaryCore(integral), false));
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.Binary:
          result = new MessagePackObject(new MessagePackString(this.ReadBinaryCore(integral), true));
          this.InternalData = result;
          return true;
        case ItemsUnpacker.ReadValueResult.FixExt1:
        case ItemsUnpacker.ReadValueResult.FixExt2:
        case ItemsUnpacker.ReadValueResult.FixExt4:
        case ItemsUnpacker.ReadValueResult.FixExt8:
        case ItemsUnpacker.ReadValueResult.FixExt16:
        case ItemsUnpacker.ReadValueResult.Ext8:
        case ItemsUnpacker.ReadValueResult.Ext16:
        case ItemsUnpacker.ReadValueResult.Ext32:
          result = (MessagePackObject) this.ReadMessagePackExtendedTypeObjectCore(type);
          this.InternalData = result;
          return true;
        default:
          this.ThrowTypeException(typeof (MessagePackObject), header);
          result = new MessagePackObject();
          return false;
      }
    }

    public override bool ReadArrayLength(out long result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeArrayLength(out result);
    }

    internal bool ReadSubtreeArrayLength(out long result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0L;
          return false;
        case ItemsUnpacker.ReadValueResult.ArrayLength:
          result = this.ReadArrayLengthCore(integral);
          this.CheckLength(result, ItemsUnpacker.ReadValueResult.ArrayLength);
          return true;
        default:
          this.ThrowTypeException(typeof (long), header);
          result = 0L;
          return false;
      }
    }

    public override bool ReadMapLength(out long result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeMapLength(out result);
    }

    internal bool ReadSubtreeMapLength(out long result)
    {
      byte header;
      long integral;
      switch (this.ReadValue(out header, out integral, out float _, out double _))
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = 0L;
          return false;
        case ItemsUnpacker.ReadValueResult.MapLength:
          result = this.ReadMapLengthCore(integral);
          this.CheckLength(result, ItemsUnpacker.ReadValueResult.MapLength);
          return true;
        default:
          this.ThrowTypeException(typeof (long), header);
          result = 0L;
          return false;
      }
    }

    public override bool ReadMessagePackExtendedTypeObject(out MessagePackExtendedTypeObject result)
    {
      this.EnsureNotInSubtreeMode();
      return this.ReadSubtreeMessagePackExtendedTypeObject(out result);
    }

    internal bool ReadSubtreeMessagePackExtendedTypeObject(out MessagePackExtendedTypeObject result)
    {
      byte header;
      ItemsUnpacker.ReadValueResult type = this.ReadValue(out header, out long _, out float _, out double _);
      switch (type)
      {
        case ItemsUnpacker.ReadValueResult.Eof:
          result = new MessagePackExtendedTypeObject();
          return false;
        case ItemsUnpacker.ReadValueResult.FixExt1:
        case ItemsUnpacker.ReadValueResult.FixExt2:
        case ItemsUnpacker.ReadValueResult.FixExt4:
        case ItemsUnpacker.ReadValueResult.FixExt8:
        case ItemsUnpacker.ReadValueResult.FixExt16:
        case ItemsUnpacker.ReadValueResult.Ext8:
        case ItemsUnpacker.ReadValueResult.Ext16:
        case ItemsUnpacker.ReadValueResult.Ext32:
          result = this.ReadMessagePackExtendedTypeObjectCore(type);
          return true;
        default:
          this.ThrowTypeException(typeof (MessagePackExtendedTypeObject), header);
          result = new MessagePackExtendedTypeObject();
          return false;
      }
    }

    private ItemsUnpacker.ReadValueResult ReadValue(
      out byte header,
      out long integral,
      out float real32,
      out double real64)
    {
      integral = 0L;
      real32 = 0.0f;
      real64 = 0.0;
      int num = this._source.ReadByte();
      if (this._source.CanSeek)
        this._offset = this._source.Position;
      else
        ++this._offset;
      if (num < 0)
      {
        header = (byte) 0;
        return ItemsUnpacker.ReadValueResult.Eof;
      }
      header = (byte) num;
      switch ((int) header >> 4)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          integral = (long) header;
          return ItemsUnpacker.ReadValueResult.Byte;
        case 8:
          integral = (long) ((int) header & 15);
          return ItemsUnpacker.ReadValueResult.MapLength;
        case 9:
          integral = (long) ((int) header & 15);
          return ItemsUnpacker.ReadValueResult.ArrayLength;
        case 10:
        case 11:
          integral = (long) ((int) header & 31);
          return ItemsUnpacker.ReadValueResult.String;
        case 14:
        case 15:
          this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
          integral = (long) header | -256L;
          return ItemsUnpacker.ReadValueResult.SByte;
        default:
          switch (header)
          {
            case 192:
              return ItemsUnpacker.ReadValueResult.Nil;
            case 194:
              integral = 0L;
              return ItemsUnpacker.ReadValueResult.Boolean;
            case 195:
              integral = 1L;
              return ItemsUnpacker.ReadValueResult.Boolean;
            case 196:
              this.ReadStrict(this._scalarBuffer, 1);
              integral = (long) BigEndianBinary.ToByte(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Binary;
            case 197:
              this.ReadStrict(this._scalarBuffer, 2);
              integral = (long) BigEndianBinary.ToUInt16(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Binary;
            case 198:
              this.ReadStrict(this._scalarBuffer, 4);
              integral = (long) BigEndianBinary.ToUInt32(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Binary;
            case 199:
              return ItemsUnpacker.ReadValueResult.Ext8;
            case 200:
              return ItemsUnpacker.ReadValueResult.Ext16;
            case 201:
              return ItemsUnpacker.ReadValueResult.Ext32;
            case 202:
              this.ReadStrict(this._scalarBuffer, 4);
              real32 = BigEndianBinary.ToSingle(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Single;
            case 203:
              this.ReadStrict(this._scalarBuffer, 8);
              real64 = BigEndianBinary.ToDouble(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Double;
            case 204:
              this.ReadStrict(this._scalarBuffer, 1);
              integral = (long) BigEndianBinary.ToByte(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Byte;
            case 205:
              this.ReadStrict(this._scalarBuffer, 2);
              integral = (long) BigEndianBinary.ToUInt16(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.UInt16;
            case 206:
              this.ReadStrict(this._scalarBuffer, 4);
              integral = (long) BigEndianBinary.ToUInt32(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.UInt32;
            case 207:
              this.ReadStrict(this._scalarBuffer, 8);
              integral = (long) BigEndianBinary.ToUInt64(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.UInt64;
            case 208:
              this.ReadStrict(this._scalarBuffer, 1);
              integral = (long) BigEndianBinary.ToSByte(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.SByte;
            case 209:
              this.ReadStrict(this._scalarBuffer, 2);
              integral = (long) BigEndianBinary.ToInt16(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Int16;
            case 210:
              this.ReadStrict(this._scalarBuffer, 4);
              integral = (long) BigEndianBinary.ToInt32(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Int32;
            case 211:
              this.ReadStrict(this._scalarBuffer, 8);
              integral = BigEndianBinary.ToInt64(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.Int64;
            case 212:
              return ItemsUnpacker.ReadValueResult.FixExt1;
            case 213:
              return ItemsUnpacker.ReadValueResult.FixExt2;
            case 214:
              return ItemsUnpacker.ReadValueResult.FixExt4;
            case 215:
              return ItemsUnpacker.ReadValueResult.FixExt8;
            case 216:
              return ItemsUnpacker.ReadValueResult.FixExt16;
            case 217:
              this.ReadStrict(this._scalarBuffer, 1);
              integral = (long) BigEndianBinary.ToByte(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.String;
            case 218:
              this.ReadStrict(this._scalarBuffer, 2);
              integral = (long) BigEndianBinary.ToUInt16(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.String;
            case 219:
              this.ReadStrict(this._scalarBuffer, 4);
              integral = (long) BigEndianBinary.ToUInt32(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.String;
            case 220:
              this.ReadStrict(this._scalarBuffer, 2);
              integral = (long) BigEndianBinary.ToUInt16(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.ArrayLength;
            case 221:
              this.ReadStrict(this._scalarBuffer, 4);
              integral = (long) BigEndianBinary.ToUInt32(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.ArrayLength;
            case 222:
              this.ReadStrict(this._scalarBuffer, 2);
              integral = (long) BigEndianBinary.ToUInt16(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.MapLength;
            case 223:
              this.ReadStrict(this._scalarBuffer, 4);
              integral = (long) BigEndianBinary.ToUInt32(this._scalarBuffer, 0);
              return ItemsUnpacker.ReadValueResult.MapLength;
            default:
              ItemsUnpacker.ThrowUnassingedMessageTypeException((int) header);
              return ItemsUnpacker.ReadValueResult.Eof;
          }
      }
    }

    private static void ThrowUnassingedMessageTypeException(int header)
    {
      throw new UnassignedMessageTypeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unknown header value 0x{0:X}", (object) header));
    }

    private long ReadArrayLengthCore(long length)
    {
      this.InternalCollectionType = ItemsUnpacker.CollectionType.Array;
      this.InternalItemsCount = length;
      this.InternalData = (MessagePackObject) (uint) length;
      return length;
    }

    private long ReadMapLengthCore(long length)
    {
      this.InternalCollectionType = ItemsUnpacker.CollectionType.Map;
      this.InternalItemsCount = length;
      this.InternalData = (MessagePackObject) (uint) length;
      return length;
    }

    private byte[] ReadBinaryCore(long length)
    {
      if (length == 0L)
      {
        this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
        return Binary.Empty;
      }
      this.CheckLength(length, ItemsUnpacker.ReadValueResult.Binary);
      byte[] buffer = new byte[length];
      this.ReadStrict(buffer, buffer.Length);
      this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
      return buffer;
    }

    private string ReadStringCore(long length)
    {
      if (length == 0L)
      {
        this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
        return string.Empty;
      }
      this.CheckLength(length, ItemsUnpacker.ReadValueResult.String);
      int num1 = (int) length;
      byte[] byteBuffer = BufferManager.GetByteBuffer();
      if (num1 <= byteBuffer.Length)
      {
        this.ReadStrict(byteBuffer, num1);
        string str = Encoding.UTF8.GetString(byteBuffer, 0, num1);
        this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
        return str;
      }
      Decoder decoder = Encoding.UTF8.GetDecoder();
      char[] charBuffer = BufferManager.GetCharBuffer();
      StringBuilder stringBuilder = new StringBuilder(Math.Min(num1, int.MaxValue));
      int val1 = num1;
      do
      {
        int count = Math.Min(val1, byteBuffer.Length);
        int num2 = this._source.Read(byteBuffer, 0, count);
        this._offset += (long) num2;
        if (num2 == 0)
          this.ThrowEofException(0L, (long) count);
        val1 -= num2;
        bool completed = false;
        int byteIndex = 0;
        while (!completed)
        {
          int bytesUsed;
          int charsUsed;
          decoder.Convert(byteBuffer, byteIndex, num2 - byteIndex, charBuffer, 0, charBuffer.Length, num2 == 0, out bytesUsed, out charsUsed, out completed);
          stringBuilder.Append(charBuffer, 0, charsUsed);
          byteIndex += bytesUsed;
        }
      }
      while (val1 > 0);
      this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
      return stringBuilder.ToString();
    }

    private MessagePackExtendedTypeObject ReadMessagePackExtendedTypeObjectCore(
      ItemsUnpacker.ReadValueResult type)
    {
      byte typeCode;
      uint num;
      switch (type)
      {
        case ItemsUnpacker.ReadValueResult.FixExt1:
          typeCode = this.ReadByteStrict();
          num = 1U;
          break;
        case ItemsUnpacker.ReadValueResult.FixExt2:
          typeCode = this.ReadByteStrict();
          num = 2U;
          break;
        case ItemsUnpacker.ReadValueResult.FixExt4:
          typeCode = this.ReadByteStrict();
          num = 4U;
          break;
        case ItemsUnpacker.ReadValueResult.FixExt8:
          typeCode = this.ReadByteStrict();
          num = 8U;
          break;
        case ItemsUnpacker.ReadValueResult.FixExt16:
          typeCode = this.ReadByteStrict();
          num = 16U;
          break;
        case ItemsUnpacker.ReadValueResult.Ext8:
          this.ReadStrict(this._scalarBuffer, 1);
          num = (uint) BigEndianBinary.ToByte(this._scalarBuffer, 0);
          typeCode = this.ReadByteStrict();
          break;
        case ItemsUnpacker.ReadValueResult.Ext16:
          this.ReadStrict(this._scalarBuffer, 2);
          num = (uint) BigEndianBinary.ToUInt16(this._scalarBuffer, 0);
          typeCode = this.ReadByteStrict();
          break;
        case ItemsUnpacker.ReadValueResult.Ext32:
          this.ReadStrict(this._scalarBuffer, 4);
          num = BigEndianBinary.ToUInt32(this._scalarBuffer, 0);
          typeCode = this.ReadByteStrict();
          break;
        default:
          ItemsUnpacker.ThrowUnexpectedExtCodeException(type);
          return new MessagePackExtendedTypeObject();
      }
      byte[] numArray = new byte[(IntPtr) num];
      this.ReadStrict(numArray, numArray.Length);
      this.InternalCollectionType = ItemsUnpacker.CollectionType.None;
      return new MessagePackExtendedTypeObject(typeCode, numArray);
    }

    private static void ThrowUnexpectedExtCodeException(ItemsUnpacker.ReadValueResult type)
    {
      throw new NotSupportedException("Unexpeded ext-code type. " + (object) type);
    }

    private void CheckLength(long length, ItemsUnpacker.ReadValueResult type)
    {
      if (length <= (long) int.MaxValue)
        return;
      this.ThrowTooLongLengthException(length, type);
    }

    private void ThrowTooLongLengthException(long length, ItemsUnpacker.ReadValueResult type)
    {
      string format;
      switch (type)
      {
        case ItemsUnpacker.ReadValueResult.ArrayLength:
          format = this._source.CanSeek ? "MessagePack for CLI cannot handle large array (0x{0:X} elements) which has more than Int32.MaxValue elements, at position {1:#,0}" : "MessagePack for CLI cannot handle large array (0x{0:X} elements) which has more than Int32.MaxValue elements, at offset {1:#,0}";
          break;
        case ItemsUnpacker.ReadValueResult.MapLength:
          format = this._source.CanSeek ? "MessagePack for CLI cannot handle large map (0x{0:X} entries) which has more than Int32.MaxValue entries, at position {1:#,0}" : "MessagePack for CLI cannot handle large map (0x{0:X} entries) which has more than Int32.MaxValue entries, at offset {1:#,0}";
          break;
        default:
          format = this._source.CanSeek ? "MessagePack for CLI cannot handle large binary or string (0x{0:X} bytes) which has more than Int32.MaxValue bytes, at position {1:#,0}" : "MessagePack for CLI cannot handle large binary or string (0x{0:X} bytes) which has more than Int32.MaxValue bytes, at offset {1:#,0}";
          break;
      }
      throw new MessageNotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, (object) length, (object) this._offset));
    }

    private void ThrowTypeException(Type type, byte header)
    {
      throw new MessageTypeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, this._source.CanSeek ? "Cannot convert '{0}' type value from type '{2}'(0x{1:X}) in position {3:#,0}." : "Cannot convert '{0}' type value from type '{2}'(0x{1:X}) in offset {3:#,0}.", (object) type, (object) header, (object) MessagePackCode.ToString((int) header), (object) this._offset));
    }

    protected override long? SkipCore()
    {
      Stream source = this._source;
      byte[] scalarBuffer = this._scalarBuffer;
      long num1 = -1;
      long num2 = 0;
      Stack<long> longStack = (Stack<long>) null;
      do
      {
        int header = source.ReadByte();
        if (header < 0)
          return new long?();
        switch (header)
        {
          case 192:
          case 194:
          case 195:
            ++num2;
            --num1;
            if (longStack != null)
            {
              while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                num1 = longStack.Pop() - 1L;
              break;
            }
            break;
          default:
            if (header < 128)
            {
              ++num2;
              --num1;
              if (longStack != null)
              {
                while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                  num1 = longStack.Pop() - 1L;
                break;
              }
              break;
            }
            if (header >= 224)
            {
              ++num2;
              --num1;
              if (longStack != null)
              {
                while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                  num1 = longStack.Pop() - 1L;
                break;
              }
              break;
            }
            switch (header & 240)
            {
              case 128:
                int num3 = header & 15;
                ++num2;
                if (num3 == 0)
                {
                  --num1;
                  if (longStack != null)
                  {
                    while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                      num1 = longStack.Pop() - 1L;
                    break;
                  }
                  break;
                }
                if (num1 >= 0L)
                {
                  if (longStack == null)
                    longStack = new Stack<long>(4);
                  longStack.Push(num1);
                }
                num1 = (long) (num3 * 2);
                break;
              case 144:
                int num4 = header & 15;
                ++num2;
                if (num4 == 0)
                {
                  --num1;
                  if (longStack != null)
                  {
                    while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                      num1 = longStack.Pop() - 1L;
                    break;
                  }
                  break;
                }
                if (num1 >= 0L)
                {
                  if (longStack == null)
                    longStack = new Stack<long>(4);
                  longStack.Push(num1);
                }
                num1 = (long) num4;
                break;
              case 160:
              case 176:
                int num5 = header & 31;
                long num6 = num2 + 1L;
                long num7 = 0;
                while ((long) num5 > num7)
                {
                  long num8 = (long) num5 - num7;
                  byte[] byteBuffer = BufferManager.GetByteBuffer();
                  int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                  num7 += (long) source.Read(byteBuffer, 0, count);
                  if (num7 < (long) count)
                    return new long?();
                }
                num2 = num6 + num7;
                --num1;
                if (longStack != null)
                {
                  while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                    num1 = longStack.Pop() - 1L;
                  break;
                }
                break;
              default:
                switch (header)
                {
                  case 196:
                  case 217:
                    long num9 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    byte num10 = scalarBuffer[0];
                    long num11 = num9 + 1L;
                    long num12 = 0;
                    while ((long) num10 > num12)
                    {
                      long num8 = (long) num10 - num12;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num12 += (long) source.Read(byteBuffer, 0, count);
                      if (num12 < (long) count)
                        return new long?();
                    }
                    num2 = num11 + num12;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 197:
                  case 218:
                    long num13 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 2) != 2)
                      return new long?();
                    ushort uint16_1 = BigEndianBinary.ToUInt16(scalarBuffer, 0);
                    long num14 = num13 + 2L;
                    long num15 = 0;
                    while ((long) uint16_1 > num15)
                    {
                      long num8 = (long) uint16_1 - num15;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num15 += (long) source.Read(byteBuffer, 0, count);
                      if (num15 < (long) count)
                        return new long?();
                    }
                    num2 = num14 + num15;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 198:
                  case 219:
                    long num16 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 4) != 4)
                      return new long?();
                    uint uint32_1 = BigEndianBinary.ToUInt32(scalarBuffer, 0);
                    long num17 = num16 + 4L;
                    long num18 = 0;
                    while ((long) uint32_1 > num18)
                    {
                      long num8 = (long) uint32_1 - num18;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num18 += (long) source.Read(byteBuffer, 0, count);
                      if (num18 < (long) count)
                        return new long?();
                    }
                    num2 = num17 + num18;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 199:
                    long num19 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    byte num20 = scalarBuffer[0];
                    long num21 = num19 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num22 = num21 + 1L;
                    long num23 = 0;
                    while ((long) num20 > num23)
                    {
                      long num8 = (long) num20 - num23;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num23 += (long) source.Read(byteBuffer, 0, count);
                      if (num23 < (long) count)
                        return new long?();
                    }
                    num2 = num22 + num23;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 200:
                    long num24 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 2) != 2)
                      return new long?();
                    ushort uint16_2 = BigEndianBinary.ToUInt16(scalarBuffer, 0);
                    long num25 = num24 + 2L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num26 = num25 + 1L;
                    long num27 = 0;
                    while ((long) uint16_2 > num27)
                    {
                      long num8 = (long) uint16_2 - num27;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num27 += (long) source.Read(byteBuffer, 0, count);
                      if (num27 < (long) count)
                        return new long?();
                    }
                    num2 = num26 + num27;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 201:
                    long num28 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 4) != 4)
                      return new long?();
                    uint uint32_2 = BigEndianBinary.ToUInt32(scalarBuffer, 0);
                    long num29 = num28 + 4L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num30 = num29 + 1L;
                    long num31 = 0;
                    while ((long) uint32_2 > num31)
                    {
                      long num8 = (long) uint32_2 - num31;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num31 += (long) source.Read(byteBuffer, 0, count);
                      if (num31 < (long) count)
                        return new long?();
                    }
                    num2 = num30 + num31;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 202:
                  case 206:
                  case 210:
                    long num32 = num2 + 1L;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                    }
                    long num33 = 0;
                    while (4L > num33)
                    {
                      long num8 = 4L - num33;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num33 += (long) source.Read(byteBuffer, 0, count);
                      if (num33 < (long) count)
                        return new long?();
                    }
                    num2 = num32 + num33;
                    break;
                  case 203:
                  case 207:
                  case 211:
                    long num34 = num2 + 1L;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                    }
                    long num35 = 0;
                    while (8L > num35)
                    {
                      long num8 = 8L - num35;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num35 += (long) source.Read(byteBuffer, 0, count);
                      if (num35 < (long) count)
                        return new long?();
                    }
                    num2 = num34 + num35;
                    break;
                  case 204:
                  case 208:
                    long num36 = num2 + 1L;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                    }
                    long num37 = 0;
                    while (1L > num37)
                    {
                      long num8 = 1L - num37;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num37 += (long) source.Read(byteBuffer, 0, count);
                      if (num37 < (long) count)
                        return new long?();
                    }
                    num2 = num36 + num37;
                    break;
                  case 205:
                  case 209:
                    long num38 = num2 + 1L;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                    }
                    long num39 = 0;
                    while (2L > num39)
                    {
                      long num8 = 2L - num39;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num39 += (long) source.Read(byteBuffer, 0, count);
                      if (num39 < (long) count)
                        return new long?();
                    }
                    num2 = num38 + num39;
                    break;
                  case 212:
                    long num40 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num41 = num40 + 1L;
                    long num42 = 0;
                    while (1L > num42)
                    {
                      long num8 = 1L - num42;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num42 += (long) source.Read(byteBuffer, 0, count);
                      if (num42 < (long) count)
                        return new long?();
                    }
                    num2 = num41 + num42;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 213:
                    long num43 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num44 = num43 + 1L;
                    long num45 = 0;
                    while (2L > num45)
                    {
                      long num8 = 2L - num45;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num45 += (long) source.Read(byteBuffer, 0, count);
                      if (num45 < (long) count)
                        return new long?();
                    }
                    num2 = num44 + num45;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 214:
                    long num46 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num47 = num46 + 1L;
                    long num48 = 0;
                    while (4L > num48)
                    {
                      long num8 = 4L - num48;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num48 += (long) source.Read(byteBuffer, 0, count);
                      if (num48 < (long) count)
                        return new long?();
                    }
                    num2 = num47 + num48;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 215:
                    long num49 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num50 = num49 + 1L;
                    long num51 = 0;
                    while (8L > num51)
                    {
                      long num8 = 8L - num51;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num51 += (long) source.Read(byteBuffer, 0, count);
                      if (num51 < (long) count)
                        return new long?();
                    }
                    num2 = num50 + num51;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 216:
                    long num52 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 1) != 1)
                      return new long?();
                    long num53 = num52 + 1L;
                    long num54 = 0;
                    while (16L > num54)
                    {
                      long num8 = 16L - num54;
                      byte[] byteBuffer = BufferManager.GetByteBuffer();
                      int count = num8 > (long) byteBuffer.Length ? byteBuffer.Length : (int) num8;
                      num54 += (long) source.Read(byteBuffer, 0, count);
                      if (num54 < (long) count)
                        return new long?();
                    }
                    num2 = num53 + num54;
                    --num1;
                    if (longStack != null)
                    {
                      while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                        num1 = longStack.Pop() - 1L;
                      break;
                    }
                    break;
                  case 220:
                    long num55 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 2) != 2)
                      return new long?();
                    ushort uint16_3 = BigEndianBinary.ToUInt16(scalarBuffer, 0);
                    num2 = num55 + 2L;
                    if (uint16_3 == (ushort) 0)
                    {
                      --num1;
                      if (longStack != null)
                      {
                        while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                          num1 = longStack.Pop() - 1L;
                        break;
                      }
                      break;
                    }
                    if (num1 >= 0L)
                    {
                      if (longStack == null)
                        longStack = new Stack<long>(4);
                      longStack.Push(num1);
                    }
                    num1 = (long) uint16_3;
                    break;
                  case 221:
                    long num56 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 4) != 4)
                      return new long?();
                    uint uint32_3 = BigEndianBinary.ToUInt32(scalarBuffer, 0);
                    num2 = num56 + 4L;
                    if (uint32_3 == 0U)
                    {
                      --num1;
                      if (longStack != null)
                      {
                        while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                          num1 = longStack.Pop() - 1L;
                        break;
                      }
                      break;
                    }
                    if (num1 >= 0L)
                    {
                      if (longStack == null)
                        longStack = new Stack<long>(4);
                      longStack.Push(num1);
                    }
                    num1 = (long) uint32_3;
                    break;
                  case 222:
                    long num57 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 2) != 2)
                      return new long?();
                    ushort uint16_4 = BigEndianBinary.ToUInt16(scalarBuffer, 0);
                    num2 = num57 + 2L;
                    if (uint16_4 == (ushort) 0)
                    {
                      --num1;
                      if (longStack != null)
                      {
                        while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                          num1 = longStack.Pop() - 1L;
                        break;
                      }
                      break;
                    }
                    if (num1 >= 0L)
                    {
                      if (longStack == null)
                        longStack = new Stack<long>(4);
                      longStack.Push(num1);
                    }
                    num1 = (long) ((int) uint16_4 * 2);
                    break;
                  case 223:
                    long num58 = num2 + 1L;
                    if (source.Read(scalarBuffer, 0, 4) != 4)
                      return new long?();
                    uint uint32_4 = BigEndianBinary.ToUInt32(scalarBuffer, 0);
                    num2 = num58 + 4L;
                    if (uint32_4 == 0U)
                    {
                      --num1;
                      if (longStack != null)
                      {
                        while (num1 == 0L && longStack.Count > 0 && longStack.Count != 0)
                          num1 = longStack.Pop() - 1L;
                        break;
                      }
                      break;
                    }
                    if (num1 >= 0L)
                    {
                      if (longStack == null)
                        longStack = new Stack<long>(4);
                      longStack.Push(num1);
                    }
                    num1 = (long) (uint32_4 * 2U);
                    break;
                  default:
                    ItemsUnpacker.ThrowUnassingedMessageTypeException(header);
                    return new long?();
                }
                break;
            }
            break;
        }
      }
      while (num1 > 0L);
      return new long?(num2);
    }

    internal enum CollectionType
    {
      None,
      Array,
      Map,
    }

    private enum ReadValueResult
    {
      Eof,
      Nil,
      Boolean,
      SByte,
      Byte,
      Int16,
      UInt16,
      Int32,
      UInt32,
      Int64,
      UInt64,
      Single,
      Double,
      ArrayLength,
      MapLength,
      String,
      Binary,
      FixExt1,
      FixExt2,
      FixExt4,
      FixExt8,
      FixExt16,
      Ext8,
      Ext16,
      Ext32,
    }
  }
}
