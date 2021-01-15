// Decompiled with JetBrains decompiler
// Type: MsgPack.Packer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MsgPack
{
  public abstract class Packer : IDisposable
  {
    private static volatile int _defaultCompatibilityOptions = 3;
    private bool _isDisposed;
    private readonly PackerCompatibilityOptions _compatibilityOptions;

    public static PackerCompatibilityOptions DefaultCompatibilityOptions
    {
      get
      {
        return (PackerCompatibilityOptions) Packer._defaultCompatibilityOptions;
      }
      set
      {
        Packer._defaultCompatibilityOptions = (int) value;
      }
    }

    public virtual bool CanSeek
    {
      get
      {
        return false;
      }
    }

    public virtual long Position
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public PackerCompatibilityOptions CompatibilityOptions
    {
      get
      {
        return this._compatibilityOptions;
      }
    }

    protected Packer()
      : this(Packer.DefaultCompatibilityOptions)
    {
    }

    protected Packer(PackerCompatibilityOptions compatibilityOptions)
    {
      this._compatibilityOptions = compatibilityOptions;
    }

    public static Packer Create(Stream stream)
    {
      return Packer.Create(stream, true);
    }

    public static Packer Create(
      Stream stream,
      PackerCompatibilityOptions compatibilityOptions)
    {
      return Packer.Create(stream, compatibilityOptions, true);
    }

    public static Packer Create(Stream stream, bool ownsStream)
    {
      return Packer.Create(stream, Packer.DefaultCompatibilityOptions, ownsStream);
    }

    public static Packer Create(
      Stream stream,
      PackerCompatibilityOptions compatibilityOptions,
      bool ownsStream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      return (Packer) new StreamPacker(stream, compatibilityOptions, ownsStream);
    }

    public void Dispose()
    {
      if (this._isDisposed)
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
      this._isDisposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private void VerifyNotDisposed()
    {
      if (this._isDisposed)
        throw new ObjectDisposedException(this.ToString());
    }

    protected virtual void SeekTo(long offset)
    {
      throw new NotSupportedException();
    }

    protected abstract void WriteByte(byte value);

    protected virtual void WriteBytes(ICollection<byte> value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      foreach (byte num in (IEnumerable<byte>) value)
        this.WriteByte(num);
    }

    protected virtual void WriteBytes(byte[] value, bool isImmutable)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      foreach (byte num in value)
        this.WriteByte(num);
    }

    private void StreamWrite<TItem>(
      IEnumerable<TItem> value,
      Action<IEnumerable<TItem>, PackingOptions> writeBody,
      PackingOptions options)
    {
      if (this.CanSeek)
      {
        this.SeekTo(4L);
        long position = this.Position;
        writeBody(value, options);
        long offset = this.Position - position;
        this.SeekTo(-offset);
        this.SeekTo(-4L);
        this.WriteByte((byte) ((ulong) (offset >> 24) & (ulong) byte.MaxValue));
        this.WriteByte((byte) ((ulong) (offset >> 16) & (ulong) byte.MaxValue));
        this.WriteByte((byte) ((ulong) (offset >> 8) & (ulong) byte.MaxValue));
        this.WriteByte((byte) ((ulong) offset & (ulong) byte.MaxValue));
        this.SeekTo(offset);
      }
      else
      {
        if (!(value is ICollection<TItem> objs))
          objs = (ICollection<TItem>) value.ToArray<TItem>();
        ICollection<TItem> objs1 = objs;
        int count = objs1.Count;
        this.WriteByte((byte) (count >> 24 & (int) byte.MaxValue));
        this.WriteByte((byte) (count >> 16 & (int) byte.MaxValue));
        this.WriteByte((byte) (count >> 8 & (int) byte.MaxValue));
        this.WriteByte((byte) (count & (int) byte.MaxValue));
        writeBody((IEnumerable<TItem>) objs1, options);
      }
    }

    [CLSCompliant(false)]
    public Packer Pack(sbyte value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(sbyte value)
    {
      if (this.TryPackTinySignedInteger((long) value))
        return;
      this.TryPackInt8((long) value);
    }

    protected bool TryPackInt8(long value)
    {
      if (value > (long) sbyte.MaxValue || value < (long) sbyte.MinValue)
        return false;
      this.WriteByte((byte) 208);
      this.WriteByte((byte) value);
      return true;
    }

    public Packer Pack(byte value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(byte value)
    {
      if (this.TryPackTinyUnsignedInteger((ulong) value))
        return;
      this.TryPackUInt8((ulong) value);
    }

    private bool TryPackUInt8(ulong value)
    {
      if (value > (ulong) byte.MaxValue)
        return false;
      this.WriteByte((byte) 204);
      this.WriteByte((byte) value);
      return true;
    }

    public Packer Pack(short value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(short value)
    {
      if (this.TryPackTinySignedInteger((long) value) || this.TryPackInt8((long) value))
        return;
      this.TryPackInt16((long) value);
    }

    protected bool TryPackInt16(long value)
    {
      if (value < (long) short.MinValue || value > (long) short.MaxValue)
        return false;
      this.WriteByte((byte) 209);
      this.WriteByte((byte) ((ulong) (value >> 8) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) value & (ulong) byte.MaxValue));
      return true;
    }

    [CLSCompliant(false)]
    public Packer Pack(ushort value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(ushort value)
    {
      if (this.TryPackTinyUnsignedInteger((ulong) value) || this.TryPackUInt8((ulong) value))
        return;
      this.TryPackUInt16((ulong) value);
    }

    [CLSCompliant(false)]
    protected bool TryPackUInt16(ulong value)
    {
      if (value > (ulong) ushort.MaxValue)
        return false;
      this.WriteByte((byte) 205);
      this.WriteByte((byte) (value >> 8 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value & (ulong) byte.MaxValue));
      return true;
    }

    public Packer Pack(int value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(int value)
    {
      if (this.TryPackTinySignedInteger((long) value) || this.TryPackInt8((long) value) || this.TryPackInt16((long) value))
        return;
      this.TryPackInt32((long) value);
    }

    protected bool TryPackInt32(long value)
    {
      if (value > (long) int.MaxValue || value < (long) int.MinValue)
        return false;
      this.WriteByte((byte) 210);
      this.WriteByte((byte) ((ulong) (value >> 24) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 16) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 8) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) value & (ulong) byte.MaxValue));
      return true;
    }

    [CLSCompliant(false)]
    public Packer Pack(uint value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(uint value)
    {
      if (this.TryPackTinyUnsignedInteger((ulong) value) || this.TryPackUInt8((ulong) value) || this.TryPackUInt16((ulong) value))
        return;
      this.TryPackUInt32((ulong) value);
    }

    [CLSCompliant(false)]
    protected bool TryPackUInt32(ulong value)
    {
      if (value > (ulong) uint.MaxValue)
        return false;
      this.WriteByte((byte) 206);
      this.WriteByte((byte) (value >> 24 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 16 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 8 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value & (ulong) byte.MaxValue));
      return true;
    }

    public Packer Pack(long value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(long value)
    {
      if (this.TryPackTinySignedInteger(value) || this.TryPackInt8(value) || (this.TryPackInt16(value) || this.TryPackInt32(value)))
        return;
      this.TryPackInt64(value);
    }

    protected bool TryPackInt64(long value)
    {
      this.WriteByte((byte) 211);
      this.WriteByte((byte) ((ulong) (value >> 56) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 48) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 40) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 32) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 24) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 16) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (value >> 8) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) value & (ulong) byte.MaxValue));
      return true;
    }

    [CLSCompliant(false)]
    public Packer Pack(ulong value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(ulong value)
    {
      if (this.TryPackTinyUnsignedInteger(value) || this.TryPackUInt8(value) || (this.TryPackUInt16(value) || this.TryPackUInt32(value)))
        return;
      this.TryPackUInt64(value);
    }

    [CLSCompliant(false)]
    protected bool TryPackUInt64(ulong value)
    {
      this.WriteByte((byte) 207);
      this.WriteByte((byte) (value >> 56 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 48 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 40 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 32 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 24 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 16 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value >> 8 & (ulong) byte.MaxValue));
      this.WriteByte((byte) (value & (ulong) byte.MaxValue));
      return true;
    }

    public Packer Pack(float value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(float value)
    {
      this.WriteByte((byte) 202);
      Float32Bits float32Bits = new Float32Bits(value);
      if (BitConverter.IsLittleEndian)
      {
        this.WriteByte(float32Bits.Byte3);
        this.WriteByte(float32Bits.Byte2);
        this.WriteByte(float32Bits.Byte1);
        this.WriteByte(float32Bits.Byte0);
      }
      else
      {
        this.WriteByte(float32Bits.Byte0);
        this.WriteByte(float32Bits.Byte1);
        this.WriteByte(float32Bits.Byte2);
        this.WriteByte(float32Bits.Byte3);
      }
    }

    public Packer Pack(double value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(double value)
    {
      this.WriteByte((byte) 203);
      long int64Bits = BitConverter.DoubleToInt64Bits(value);
      this.WriteByte((byte) ((ulong) (int64Bits >> 56) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (int64Bits >> 48) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (int64Bits >> 40) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (int64Bits >> 32) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (int64Bits >> 24) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (int64Bits >> 16) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) (int64Bits >> 8) & (ulong) byte.MaxValue));
      this.WriteByte((byte) ((ulong) int64Bits & (ulong) byte.MaxValue));
    }

    public Packer Pack(bool value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackCore(value);
      return this;
    }

    private void PrivatePackCore(bool value)
    {
      this.WriteByte(value ? (byte) 195 : (byte) 194);
    }

    public Packer PackArrayHeader(int count)
    {
      this.PackArrayHeaderCore(count);
      return this;
    }

    protected void PackArrayHeaderCore(int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is negative.", (object) nameof (count)));
      this.VerifyNotDisposed();
      this.PrivatePackArrayHeaderCore(count);
    }

    private void PrivatePackArrayHeaderCore(int count)
    {
      if (count < 16)
        this.WriteByte((byte) (144 | count));
      else if (count <= (int) ushort.MaxValue)
      {
        this.WriteByte((byte) 220);
        this.WriteByte((byte) (count >> 8 & (int) byte.MaxValue));
        this.WriteByte((byte) (count & (int) byte.MaxValue));
      }
      else
      {
        this.WriteByte((byte) 221);
        this.WriteByte((byte) (count >> 24 & (int) byte.MaxValue));
        this.WriteByte((byte) (count >> 16 & (int) byte.MaxValue));
        this.WriteByte((byte) (count >> 8 & (int) byte.MaxValue));
        this.WriteByte((byte) (count & (int) byte.MaxValue));
      }
    }

    public Packer PackMapHeader(int count)
    {
      this.PackMapHeaderCore(count);
      return this;
    }

    protected void PackMapHeaderCore(int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is negative.", (object) nameof (count)));
      this.VerifyNotDisposed();
      this.PrivatePackMapHeaderCore(count);
    }

    private void PrivatePackMapHeaderCore(int count)
    {
      if (count < 16)
        this.WriteByte((byte) (128 | count));
      else if (count <= (int) ushort.MaxValue)
      {
        this.WriteByte((byte) 222);
        this.WriteByte((byte) (count >> 8 & (int) byte.MaxValue));
        this.WriteByte((byte) (count & (int) byte.MaxValue));
      }
      else
      {
        this.WriteByte((byte) 223);
        this.WriteByte((byte) (count >> 24 & (int) byte.MaxValue));
        this.WriteByte((byte) (count >> 16 & (int) byte.MaxValue));
        this.WriteByte((byte) (count >> 8 & (int) byte.MaxValue));
        this.WriteByte((byte) (count & (int) byte.MaxValue));
      }
    }

    [Obsolete("Use PackStringHeader(Int32) or Use PackBinaryHeader(Int32) instead.")]
    public Packer PackRawHeader(int length)
    {
      this.PackRawHeaderCore(length);
      return this;
    }

    public Packer PackStringHeader(int length)
    {
      this.PackStringHeaderCore(length);
      return this;
    }

    public Packer PackBinaryHeader(int length)
    {
      this.PackBinaryHeaderCore(length);
      return this;
    }

    [Obsolete("Use PackStringHeaderCore(Int32) or Use PackBinaryHeaderCore(Int32) instead.")]
    protected void PackRawHeaderCore(int length)
    {
      this.PackStringHeaderCore(length);
    }

    protected void PackStringHeaderCore(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is negative.", (object) nameof (length)));
      this.VerifyNotDisposed();
      this.PrivatePackRawHeaderCore(length, true);
    }

    protected void PackBinaryHeaderCore(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is negative.", (object) nameof (length)));
      this.VerifyNotDisposed();
      this.PrivatePackRawHeaderCore(length, false);
    }

    private void PrivatePackRawHeaderCore(int length, bool isString)
    {
      if (isString || (this._compatibilityOptions & PackerCompatibilityOptions.PackBinaryAsRaw) != PackerCompatibilityOptions.None)
      {
        if (length < 32)
          this.WriteByte((byte) (160 | length));
        else if (length <= (int) byte.MaxValue && (this._compatibilityOptions & PackerCompatibilityOptions.PackBinaryAsRaw) == PackerCompatibilityOptions.None)
        {
          this.WriteByte((byte) 217);
          this.WriteByte((byte) (length & (int) byte.MaxValue));
        }
        else if (length <= (int) ushort.MaxValue)
        {
          this.WriteByte((byte) 218);
          this.WriteByte((byte) (length >> 8 & (int) byte.MaxValue));
          this.WriteByte((byte) (length & (int) byte.MaxValue));
        }
        else
        {
          this.WriteByte((byte) 219);
          this.WriteByte((byte) (length >> 24 & (int) byte.MaxValue));
          this.WriteByte((byte) (length >> 16 & (int) byte.MaxValue));
          this.WriteByte((byte) (length >> 8 & (int) byte.MaxValue));
          this.WriteByte((byte) (length & (int) byte.MaxValue));
        }
      }
      else if (length <= (int) byte.MaxValue)
      {
        this.WriteByte((byte) 196);
        this.WriteByte((byte) (length & (int) byte.MaxValue));
      }
      else if (length <= (int) ushort.MaxValue)
      {
        this.WriteByte((byte) 197);
        this.WriteByte((byte) (length >> 8 & (int) byte.MaxValue));
        this.WriteByte((byte) (length & (int) byte.MaxValue));
      }
      else
      {
        this.WriteByte((byte) 198);
        this.WriteByte((byte) (length >> 24 & (int) byte.MaxValue));
        this.WriteByte((byte) (length >> 16 & (int) byte.MaxValue));
        this.WriteByte((byte) (length >> 8 & (int) byte.MaxValue));
        this.WriteByte((byte) (length & (int) byte.MaxValue));
      }
    }

    public Packer PackRaw(IEnumerable<byte> value)
    {
      this.VerifyNotDisposed();
      if (!(value is ICollection<byte> bytes))
        this.PrivatePackRaw(value);
      else
        this.PrivatePackRaw(bytes);
      return this;
    }

    public Packer PackRaw(IList<byte> value)
    {
      this.VerifyNotDisposed();
      if (!(value is byte[] numArray))
        this.PrivatePackRaw((ICollection<byte>) value);
      else
        this.PrivatePackRaw(numArray);
      return this;
    }

    public Packer PackRaw(byte[] value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackRaw(value);
      return this;
    }

    private void PrivatePackRaw(byte[] value)
    {
      if (value == null)
        this.PrivatePackNullCore();
      else
        this.PrivatePackRawCore(value, false);
    }

    private void PrivatePackRawCore(byte[] value, bool isImmutable)
    {
      this.PrivatePackRawHeaderCore(value.Length, true);
      this.WriteBytes(value, isImmutable);
    }

    private void PrivatePackRaw(ICollection<byte> value)
    {
      if (value == null)
      {
        this.PrivatePackNullCore();
      }
      else
      {
        this.PrivatePackRawHeaderCore(value.Count, true);
        this.WriteBytes(value);
      }
    }

    private void PrivatePackRaw(IEnumerable<byte> value)
    {
      if (value == null)
        this.PrivatePackNullCore();
      else
        this.PrivatePackRawCore(value);
    }

    private void PrivatePackRawCore(IEnumerable<byte> value)
    {
      if (!this.CanSeek)
      {
        this.PrivatePackRawCore(value.ToArray<byte>(), true);
      }
      else
      {
        this.WriteByte((byte) 219);
        this.StreamWrite<byte>(value, (Action<IEnumerable<byte>, PackingOptions>) ((items, _) => this.PrivatePackRawBodyCore(items)), (PackingOptions) null);
      }
    }

    public Packer PackRawBody(byte[] value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.VerifyNotDisposed();
      this.WriteBytes(value, false);
      return this;
    }

    public Packer PackRawBody(IEnumerable<byte> value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.VerifyNotDisposed();
      this.PrivatePackRawBodyCore(value);
      return this;
    }

    private int PrivatePackRawBodyCore(IEnumerable<byte> value)
    {
      if (value is ICollection<byte> bytes)
        return this.PrivatePackRawBodyCore(bytes, bytes.IsReadOnly);
      int num1 = 0;
      foreach (byte num2 in value)
      {
        this.WriteByte(num2);
        ++num1;
      }
      return num1;
    }

    private int PrivatePackRawBodyCore(ICollection<byte> value, bool isImmutable)
    {
      if (value is byte[] numArray)
        this.WriteBytes(numArray, isImmutable);
      else
        this.WriteBytes(value);
      return value.Count;
    }

    public Packer PackString(IEnumerable<char> value)
    {
      this.PackStringCore(value, Encoding.UTF8);
      return this;
    }

    public Packer PackString(string value)
    {
      this.PackStringCore(value, Encoding.UTF8);
      return this;
    }

    public Packer PackString(IEnumerable<char> value, Encoding encoding)
    {
      this.PackStringCore(value, encoding);
      return this;
    }

    public Packer PackString(string value, Encoding encoding)
    {
      this.PackStringCore(value, encoding);
      return this;
    }

    protected virtual void PackStringCore(IEnumerable<char> value, Encoding encoding)
    {
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      this.VerifyNotDisposed();
      this.PrivatePackString(value, encoding);
    }

    private void PrivatePackString(IEnumerable<char> value, Encoding encoding)
    {
      if (value == null)
        this.PrivatePackNullCore();
      else
        this.PrivatePackStringCore(value, encoding);
    }

    private void PrivatePackStringCore(IEnumerable<char> value, Encoding encoding)
    {
      byte[] bytes = encoding.GetBytes(value.ToArray<char>());
      this.PrivatePackRawHeaderCore(bytes.Length, true);
      this.WriteBytes(bytes, true);
    }

    protected virtual void PackStringCore(string value, Encoding encoding)
    {
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      this.VerifyNotDisposed();
      this.PrivatePackString(value, encoding);
    }

    private void PrivatePackString(string value, Encoding encoding)
    {
      if (value == null)
        this.PrivatePackNullCore();
      else
        this.PrivatePackStringCore(value, encoding);
    }

    private void PrivatePackStringCore(string value, Encoding encoding)
    {
      byte[] bytes = encoding.GetBytes(value);
      this.PrivatePackRawHeaderCore(bytes.Length, true);
      this.WriteBytes(bytes, true);
    }

    public Packer PackBinary(IEnumerable<byte> value)
    {
      this.VerifyNotDisposed();
      if (!(value is ICollection<byte> bytes))
        this.PrivatePackBinary(value);
      else
        this.PrivatePackBinary(bytes);
      return this;
    }

    public Packer PackBinary(IList<byte> value)
    {
      this.VerifyNotDisposed();
      if (!(value is byte[] numArray))
        this.PrivatePackBinary((ICollection<byte>) value);
      else
        this.PrivatePackBinary(numArray);
      return this;
    }

    public Packer PackBinary(byte[] value)
    {
      this.VerifyNotDisposed();
      this.PrivatePackBinary(value);
      return this;
    }

    private void PrivatePackBinary(byte[] value)
    {
      if (value == null)
        this.PrivatePackNullCore();
      else
        this.PrivatePackBinaryCore(value, false);
    }

    private void PrivatePackBinaryCore(byte[] value, bool isImmutable)
    {
      this.PrivatePackRawHeaderCore(value.Length, false);
      this.WriteBytes(value, isImmutable);
    }

    private void PrivatePackBinary(ICollection<byte> value)
    {
      if (value == null)
      {
        this.PrivatePackNullCore();
      }
      else
      {
        this.PrivatePackRawHeaderCore(value.Count, false);
        this.WriteBytes(value);
      }
    }

    private void PrivatePackBinary(IEnumerable<byte> value)
    {
      if (value == null)
        this.PrivatePackNullCore();
      else
        this.PrivatePackBinaryCore(value);
    }

    private void PrivatePackBinaryCore(IEnumerable<byte> value)
    {
      if (!this.CanSeek)
      {
        this.PrivatePackBinaryCore(value.ToArray<byte>(), true);
      }
      else
      {
        if ((this._compatibilityOptions & PackerCompatibilityOptions.PackBinaryAsRaw) != PackerCompatibilityOptions.None)
          this.WriteByte((byte) 219);
        else
          this.WriteByte((byte) 198);
        this.StreamWrite<byte>(value, (Action<IEnumerable<byte>, PackingOptions>) ((items, _) => this.PrivatePackRawBodyCore(items)), (PackingOptions) null);
      }
    }

    public Packer PackArrayHeader<TItem>(IList<TItem> array)
    {
      return array != null ? this.PackArrayHeader(array.Count) : this.PackNull();
    }

    public Packer PackMapHeader<TKey, TValue>(IDictionary<TKey, TValue> map)
    {
      return map != null ? this.PackMapHeader(map.Count) : this.PackNull();
    }

    protected bool TryPackTinySignedInteger(long value)
    {
      if (value >= 0L && value < 128L)
      {
        this.WriteByte((byte) value);
        return true;
      }
      if (value < -32L || value > -1L)
        return false;
      this.WriteByte((byte) value);
      return true;
    }

    [CLSCompliant(false)]
    protected bool TryPackTinyUnsignedInteger(ulong value)
    {
      if (value >= 128UL)
        return false;
      this.WriteByte((byte) value);
      return true;
    }

    public Packer PackNull()
    {
      this.VerifyNotDisposed();
      this.PrivatePackNullCore();
      return this;
    }

    private void PrivatePackNullCore()
    {
      this.WriteByte((byte) 192);
    }

    public Packer PackExtendedTypeValue(byte typeCode, byte[] body)
    {
      if (body == null)
        throw new ArgumentNullException(nameof (body));
      this.VerifyNotDisposed();
      this.PrivatePackExtendedTypeValueCore(typeCode, body);
      return this;
    }

    public Packer PackExtendedTypeValue(MessagePackExtendedTypeObject mpeto)
    {
      if (!mpeto.IsValid)
        throw new ArgumentException("MessagePackExtendedTypeObject must have body.", nameof (mpeto));
      this.PrivatePackExtendedTypeValueCore(mpeto.TypeCode, mpeto.Body);
      return this;
    }

    private void PrivatePackExtendedTypeValueCore(byte typeCode, byte[] body)
    {
      if ((this._compatibilityOptions & PackerCompatibilityOptions.ProhibitExtendedTypeObjects) != PackerCompatibilityOptions.None)
        throw new InvalidOperationException("ExtendedTypeObject is prohibited in this packer.");
      switch (body.Length)
      {
        case 1:
          this.WriteByte((byte) 212);
          break;
        case 2:
          this.WriteByte((byte) 213);
          break;
        case 4:
          this.WriteByte((byte) 214);
          break;
        case 8:
          this.WriteByte((byte) 215);
          break;
        case 16:
          this.WriteByte((byte) 216);
          break;
        default:
          if (body.Length < 256)
          {
            this.WriteByte((byte) 199);
            this.WriteByte((byte) (body.Length & (int) byte.MaxValue));
            break;
          }
          if (body.Length < 65536)
          {
            this.WriteByte((byte) 200);
            this.WriteByte((byte) (body.Length >> 8 & (int) byte.MaxValue));
            this.WriteByte((byte) (body.Length & (int) byte.MaxValue));
            break;
          }
          this.WriteByte((byte) 201);
          this.WriteByte((byte) (body.Length >> 24 & (int) byte.MaxValue));
          this.WriteByte((byte) (body.Length >> 16 & (int) byte.MaxValue));
          this.WriteByte((byte) (body.Length >> 8 & (int) byte.MaxValue));
          this.WriteByte((byte) (body.Length & (int) byte.MaxValue));
          break;
      }
      this.WriteByte(typeCode);
      this.WriteBytes(body, true);
    }

    [CLSCompliant(false)]
    public Packer Pack(sbyte? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    public Packer Pack(byte? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    public Packer Pack(short? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    [CLSCompliant(false)]
    public Packer Pack(ushort? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    public Packer Pack(int? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    [CLSCompliant(false)]
    public Packer Pack(uint? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    public Packer Pack(long? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    [CLSCompliant(false)]
    public Packer Pack(ulong? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    public Packer Pack(float? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    public Packer Pack(double? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }

    public Packer Pack(bool? value)
    {
      return !value.HasValue ? this.PackNull() : this.Pack(value.Value);
    }
  }
}
