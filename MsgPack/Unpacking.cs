// Decompiled with JetBrains decompiler
// Type: MsgPack.Unpacking
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
  public static class Unpacking
  {
    internal static readonly MessagePackObject[] PositiveIntegers = Enumerable.Range(0, 128).Select<int, MessagePackObject>((Func<int, MessagePackObject>) (i => new MessagePackObject((byte) i))).ToArray<MessagePackObject>();
    internal static readonly MessagePackObject[] NegativeIntegers = Enumerable.Range(0, 32).Select<int, MessagePackObject>((Func<int, MessagePackObject>) (i => new MessagePackObject((sbyte) (i - 32)))).ToArray<MessagePackObject>();
    internal static readonly MessagePackObject TrueValue = (MessagePackObject) true;
    internal static readonly MessagePackObject FalseValue = (MessagePackObject) false;

    private static void ValidateByteArray(byte[] source, int offset)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (source.Length == 0)
        throw new ArgumentException("Source array is empty.", nameof (source));
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset), "The offset cannot be negative.");
      if (source.Length <= offset)
        throw new ArgumentException("Source array is too small to the offset.", nameof (source));
    }

    private static void ValidateStream(Stream source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (!source.CanRead)
        throw new ArgumentException("Stream is not readable.", nameof (source));
    }

    private static void UnpackOne(Unpacker unpacker)
    {
      if (!unpacker.Read())
        throw new UnpackException("Cannot unpack MesssagePack object from the stream.");
    }

    private static void VerifyIsScalar(Unpacker unpacker)
    {
      if (unpacker.IsCollectionHeader)
        throw new MessageTypeException("The underlying stream is not scalar type.");
    }

    private static bool UnpackBooleanCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (bool) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (bool), ex);
        }
      }
    }

    private static object UnpackNullCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        if (!unpacker.LastReadData.IsNil)
          throw new MessageTypeException("The underlying stream is not nil.");
        return (object) null;
      }
    }

    private static uint? UnpackArrayLengthCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        if (Unpacking.IsNil(unpacker))
          return new uint?();
        if (!unpacker.IsArrayHeader)
          throw new MessageTypeException("The underlying stream is not array type.");
        return new uint?((uint) unpacker.LastReadData);
      }
    }

    private static IList<MessagePackObject> UnpackArrayCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        if (!Unpacking.IsNil(unpacker) && !unpacker.IsArrayHeader)
          throw new MessageTypeException("The underlying stream is not array type.");
        return Unpacking.UnpackArrayCore(unpacker);
      }
    }

    private static IList<MessagePackObject> UnpackArrayCore(Unpacker unpacker)
    {
      if (Unpacking.IsNil(unpacker))
        return (IList<MessagePackObject>) null;
      uint lastReadData = (uint) unpacker.LastReadData;
      if (lastReadData > (uint) int.MaxValue)
        throw new MessageNotSupportedException("The array which length is greater than Int32.MaxValue is not supported.");
      MessagePackObject[] messagePackObjectArray = new MessagePackObject[(int) lastReadData];
      for (int index = 0; index < messagePackObjectArray.Length; ++index)
        messagePackObjectArray[index] = Unpacking.UnpackObjectCore(unpacker);
      return (IList<MessagePackObject>) messagePackObjectArray;
    }

    private static uint? UnpackDictionaryCountCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        if (Unpacking.IsNil(unpacker))
          return new uint?();
        if (!unpacker.IsMapHeader)
          throw new MessageTypeException("The underlying stream is not map type.");
        return new uint?((uint) unpacker.LastReadData);
      }
    }

    private static MessagePackObjectDictionary UnpackDictionaryCore(
      Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        if (!Unpacking.IsNil(unpacker) && !unpacker.IsMapHeader)
          throw new MessageTypeException("The underlying stream is not map type.");
        return Unpacking.UnpackDictionaryCore(unpacker);
      }
    }

    private static MessagePackObjectDictionary UnpackDictionaryCore(
      Unpacker unpacker)
    {
      if (Unpacking.IsNil(unpacker))
        return (MessagePackObjectDictionary) null;
      uint lastReadData = (uint) unpacker.LastReadData;
      if (lastReadData > (uint) int.MaxValue)
        throw new MessageNotSupportedException("The map which count is greater than Int32.MaxValue is not supported.");
      MessagePackObjectDictionary objectDictionary = new MessagePackObjectDictionary((int) lastReadData);
      for (int index = 0; (long) index < (long) lastReadData; ++index)
      {
        MessagePackObject key = Unpacking.UnpackObjectCore(unpacker);
        MessagePackObject messagePackObject = Unpacking.UnpackObjectCore(unpacker);
        try
        {
          objectDictionary.Add(key, messagePackObject);
        }
        catch (ArgumentException ex)
        {
          throw new InvalidMessagePackStreamException("The dicationry key is duplicated in the stream.", (Exception) ex);
        }
      }
      return objectDictionary;
    }

    private static uint UnpackRawLengthCore(Stream source)
    {
      int num = source.ReadByte();
      if (num < 0)
        throw new UnpackException("Stream is end.");
      if (num == 192)
        return 0;
      if (160 <= num && num <= 191)
        return (uint) (num - 160);
      if (num == 217 || num == 196)
        return (uint) Unpacking.ReadBytes(source, 1)[0];
      if (num == 218 || num == 197)
      {
        byte[] numArray = Unpacking.ReadBytes(source, 2);
        return (uint) (ushort) ((uint) (ushort) numArray[1] | (uint) (ushort) ((uint) numArray[0] << 8));
      }
      if (num != 219 && num != 198)
        throw new MessageTypeException("The underlying stream is not raw type.");
      byte[] numArray1 = Unpacking.ReadBytes(source, 4);
      return (uint) numArray1[3] | (uint) numArray1[2] << 8 | (uint) numArray1[1] << 16 | (uint) numArray1[0] << 24;
    }

    private static byte[] ReadBytes(Stream source, int length)
    {
      byte[] buffer = new byte[length];
      if (source.Read(buffer, 0, length) < length)
        throw new UnpackException("The underlying stream unexpectedly ends.");
      return buffer;
    }

    private static byte[] UnpackBinaryCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        try
        {
          return unpacker.LastReadData.AsBinary();
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (byte[]), ex);
        }
      }
    }

    private static MessagePackObject UnpackObjectCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
        return Unpacking.UnpackObjectCore(unpacker);
    }

    private static MessagePackObject UnpackObjectCore(Unpacker unpacker)
    {
      Unpacking.UnpackOne(unpacker);
      if (unpacker.IsArrayHeader)
        return new MessagePackObject(Unpacking.UnpackArrayCore(unpacker), true);
      return unpacker.IsMapHeader ? new MessagePackObject(Unpacking.UnpackDictionaryCore(unpacker), true) : unpacker.LastReadData;
    }

    private static MessagePackExtendedTypeObject UnpackExtendedTypeObjectCore(
      Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
        return Unpacking.UnpackObjectCore(unpacker).AsMessagePackExtendedTypeObject();
    }

    private static bool IsNil(Unpacker unpacker)
    {
      return unpacker.Data.HasValue && unpacker.LastReadData.IsNil;
    }

    private static Exception NewTypeMismatchException(
      Type requestedType,
      InvalidOperationException innerException)
    {
      return (Exception) new MessageTypeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Message type is not compatible to {0}.", (object) requestedType), (Exception) innerException);
    }

    public static UnpackingResult<byte> UnpackByte(byte[] source)
    {
      return Unpacking.UnpackByte(source, 0);
    }

    public static UnpackingResult<byte> UnpackByte(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<byte>(Unpacking.UnpackByteCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static byte UnpackByte(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackByteCore(source);
    }

    private static byte UnpackByteCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (byte) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (byte), ex);
        }
      }
    }

    [CLSCompliant(false)]
    public static UnpackingResult<sbyte> UnpackSByte(byte[] source)
    {
      return Unpacking.UnpackSByte(source, 0);
    }

    [CLSCompliant(false)]
    public static UnpackingResult<sbyte> UnpackSByte(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<sbyte>(Unpacking.UnpackSByteCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    [CLSCompliant(false)]
    public static sbyte UnpackSByte(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackSByteCore(source);
    }

    private static sbyte UnpackSByteCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (sbyte) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (sbyte), ex);
        }
      }
    }

    public static UnpackingResult<short> UnpackInt16(byte[] source)
    {
      return Unpacking.UnpackInt16(source, 0);
    }

    public static UnpackingResult<short> UnpackInt16(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<short>(Unpacking.UnpackInt16Core((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static short UnpackInt16(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackInt16Core(source);
    }

    private static short UnpackInt16Core(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (short) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (short), ex);
        }
      }
    }

    [CLSCompliant(false)]
    public static UnpackingResult<ushort> UnpackUInt16(byte[] source)
    {
      return Unpacking.UnpackUInt16(source, 0);
    }

    [CLSCompliant(false)]
    public static UnpackingResult<ushort> UnpackUInt16(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<ushort>(Unpacking.UnpackUInt16Core((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    [CLSCompliant(false)]
    public static ushort UnpackUInt16(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackUInt16Core(source);
    }

    private static ushort UnpackUInt16Core(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (ushort) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (ushort), ex);
        }
      }
    }

    public static UnpackingResult<int> UnpackInt32(byte[] source)
    {
      return Unpacking.UnpackInt32(source, 0);
    }

    public static UnpackingResult<int> UnpackInt32(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<int>(Unpacking.UnpackInt32Core((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static int UnpackInt32(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackInt32Core(source);
    }

    private static int UnpackInt32Core(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (int) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (int), ex);
        }
      }
    }

    [CLSCompliant(false)]
    public static UnpackingResult<uint> UnpackUInt32(byte[] source)
    {
      return Unpacking.UnpackUInt32(source, 0);
    }

    [CLSCompliant(false)]
    public static UnpackingResult<uint> UnpackUInt32(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<uint>(Unpacking.UnpackUInt32Core((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    [CLSCompliant(false)]
    public static uint UnpackUInt32(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackUInt32Core(source);
    }

    private static uint UnpackUInt32Core(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (uint) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (uint), ex);
        }
      }
    }

    public static UnpackingResult<long> UnpackInt64(byte[] source)
    {
      return Unpacking.UnpackInt64(source, 0);
    }

    public static UnpackingResult<long> UnpackInt64(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<long>(Unpacking.UnpackInt64Core((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static long UnpackInt64(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackInt64Core(source);
    }

    private static long UnpackInt64Core(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (long) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (long), ex);
        }
      }
    }

    [CLSCompliant(false)]
    public static UnpackingResult<ulong> UnpackUInt64(byte[] source)
    {
      return Unpacking.UnpackUInt64(source, 0);
    }

    [CLSCompliant(false)]
    public static UnpackingResult<ulong> UnpackUInt64(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<ulong>(Unpacking.UnpackUInt64Core((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    [CLSCompliant(false)]
    public static ulong UnpackUInt64(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackUInt64Core(source);
    }

    private static ulong UnpackUInt64Core(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (ulong) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (ulong), ex);
        }
      }
    }

    public static UnpackingResult<float> UnpackSingle(byte[] source)
    {
      return Unpacking.UnpackSingle(source, 0);
    }

    public static UnpackingResult<float> UnpackSingle(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<float>(Unpacking.UnpackSingleCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static float UnpackSingle(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackSingleCore(source);
    }

    private static float UnpackSingleCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (float) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (float), ex);
        }
      }
    }

    public static UnpackingResult<double> UnpackDouble(byte[] source)
    {
      return Unpacking.UnpackDouble(source, 0);
    }

    public static UnpackingResult<double> UnpackDouble(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<double>(Unpacking.UnpackDoubleCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static double UnpackDouble(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackDoubleCore(source);
    }

    private static double UnpackDoubleCore(Stream source)
    {
      using (Unpacker unpacker = Unpacker.Create(source, false))
      {
        Unpacking.UnpackOne(unpacker);
        Unpacking.VerifyIsScalar(unpacker);
        try
        {
          return (double) unpacker.LastReadData;
        }
        catch (InvalidOperationException ex)
        {
          throw Unpacking.NewTypeMismatchException(typeof (double), ex);
        }
      }
    }

    public static UnpackingResult<IList<MessagePackObject>> UnpackArray(
      byte[] source)
    {
      return Unpacking.UnpackArray(source, 0);
    }

    public static UnpackingResult<IList<MessagePackObject>> UnpackArray(
      byte[] source,
      int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<IList<MessagePackObject>>(Unpacking.UnpackArrayCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static IList<MessagePackObject> UnpackArray(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackArrayCore(source);
    }

    public static UnpackingResult<long?> UnpackArrayLength(byte[] source)
    {
      return Unpacking.UnpackArrayLength(source, 0);
    }

    public static UnpackingResult<long?> UnpackArrayLength(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        uint? nullable = Unpacking.UnpackArrayLengthCore((Stream) memoryStream);
        return new UnpackingResult<long?>(nullable.HasValue ? new long?((long) nullable.GetValueOrDefault()) : new long?(), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static long? UnpackArrayLength(Stream source)
    {
      Unpacking.ValidateStream(source);
      uint? nullable = Unpacking.UnpackArrayLengthCore(source);
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }

    public static UnpackingResult<MessagePackObjectDictionary> UnpackDictionary(
      byte[] source)
    {
      return Unpacking.UnpackDictionary(source, 0);
    }

    public static UnpackingResult<MessagePackObjectDictionary> UnpackDictionary(
      byte[] source,
      int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<MessagePackObjectDictionary>(Unpacking.UnpackDictionaryCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static MessagePackObjectDictionary UnpackDictionary(
      Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackDictionaryCore(source);
    }

    public static UnpackingResult<long?> UnpackDictionaryCount(byte[] source)
    {
      return Unpacking.UnpackDictionaryCount(source, 0);
    }

    public static UnpackingResult<long?> UnpackDictionaryCount(
      byte[] source,
      int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        uint? nullable = Unpacking.UnpackDictionaryCountCore((Stream) memoryStream);
        return new UnpackingResult<long?>(nullable.HasValue ? new long?((long) nullable.GetValueOrDefault()) : new long?(), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static long? UnpackDictionaryCount(Stream source)
    {
      Unpacking.ValidateStream(source);
      uint? nullable = Unpacking.UnpackDictionaryCountCore(source);
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }

    public static UnpackingResult<byte[]> UnpackBinary(byte[] source)
    {
      return Unpacking.UnpackBinary(source, 0);
    }

    public static UnpackingResult<byte[]> UnpackBinary(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<byte[]>(Unpacking.UnpackBinaryCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static byte[] UnpackBinary(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackBinaryCore(source);
    }

    public static UnpackingResult<bool> UnpackBoolean(byte[] source)
    {
      return Unpacking.UnpackBoolean(source, 0);
    }

    public static UnpackingResult<bool> UnpackBoolean(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<bool>(Unpacking.UnpackBooleanCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static bool UnpackBoolean(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackBooleanCore(source);
    }

    public static UnpackingResult<object> UnpackNull(byte[] source)
    {
      return Unpacking.UnpackNull(source, 0);
    }

    public static UnpackingResult<object> UnpackNull(byte[] source, int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<object>(Unpacking.UnpackNullCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static object UnpackNull(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackNullCore(source);
    }

    public static UnpackingResult<MessagePackObject> UnpackObject(
      byte[] source)
    {
      return Unpacking.UnpackObject(source, 0);
    }

    public static UnpackingResult<MessagePackObject> UnpackObject(
      byte[] source,
      int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<MessagePackObject>(Unpacking.UnpackObjectCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static MessagePackObject UnpackObject(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackObjectCore(source);
    }

    public static UnpackingResult<MessagePackExtendedTypeObject> UnpackExtendedTypeObject(
      byte[] source)
    {
      return Unpacking.UnpackExtendedTypeObject(source, 0);
    }

    public static UnpackingResult<MessagePackExtendedTypeObject> UnpackExtendedTypeObject(
      byte[] source,
      int offset)
    {
      Unpacking.ValidateByteArray(source, offset);
      using (MemoryStream memoryStream = new MemoryStream(source))
      {
        memoryStream.Position = (long) offset;
        return new UnpackingResult<MessagePackExtendedTypeObject>(Unpacking.UnpackExtendedTypeObjectCore((Stream) memoryStream), (int) (memoryStream.Position - (long) offset));
      }
    }

    public static MessagePackExtendedTypeObject UnpackExtendedTypeObject(
      Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackExtendedTypeObjectCore(source);
    }

    public static UnpackingStream UnpackByteStream(Stream source)
    {
      Unpacking.ValidateStream(source);
      return Unpacking.UnpackByteStreamCore(source);
    }

    private static UnpackingStream UnpackByteStreamCore(Stream source)
    {
      uint num = Unpacking.UnpackRawLengthCore(source);
      return source.CanSeek ? (UnpackingStream) new Unpacking.SeekableUnpackingStream(source, (long) num) : (UnpackingStream) new Unpacking.UnseekableUnpackingStream(source, (long) num);
    }

    public static UnpackingStreamReader UnpackCharStream(Stream source)
    {
      return Unpacking.UnpackCharStream(source, MessagePackConvert.Utf8NonBomStrict);
    }

    public static UnpackingStreamReader UnpackCharStream(
      Stream source,
      Encoding encoding)
    {
      Unpacking.ValidateStream(source);
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      UnpackingStream unpackingStream = Unpacking.UnpackByteStreamCore(source);
      return (UnpackingStreamReader) new Unpacking.DefaultUnpackingStreamReader((Stream) unpackingStream, encoding, unpackingStream.RawLength);
    }

    public static UnpackingResult<string> UnpackString(byte[] source)
    {
      return Unpacking.UnpackString(source, 0);
    }

    public static UnpackingResult<string> UnpackString(
      byte[] source,
      Encoding encoding)
    {
      return Unpacking.UnpackString(source, 0, encoding);
    }

    public static UnpackingResult<string> UnpackString(byte[] source, int offset)
    {
      return Unpacking.UnpackString(source, offset, MessagePackConvert.Utf8NonBomStrict);
    }

    public static UnpackingResult<string> UnpackString(
      byte[] source,
      int offset,
      Encoding encoding)
    {
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      try
      {
        UnpackingResult<byte[]> unpackingResult = Unpacking.UnpackBinary(source, offset);
        return new UnpackingResult<string>(encoding.GetString(unpackingResult.Value, 0, unpackingResult.Value.Length), unpackingResult.ReadCount);
      }
      catch (DecoderFallbackException ex)
      {
        throw Unpacking.NewInvalidEncodingException(encoding, (Exception) ex);
      }
    }

    public static string UnpackString(Stream source)
    {
      return Unpacking.UnpackString(source, MessagePackConvert.Utf8NonBomStrict);
    }

    public static string UnpackString(Stream source, Encoding encoding)
    {
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      try
      {
        byte[] bytes = Unpacking.UnpackBinary(source);
        return encoding.GetString(bytes, 0, bytes.Length);
      }
      catch (DecoderFallbackException ex)
      {
        throw Unpacking.NewInvalidEncodingException(encoding, (Exception) ex);
      }
    }

    private static Exception NewInvalidEncodingException(
      Encoding encoding,
      Exception innerException)
    {
      return (Exception) new MessageTypeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The stream cannot be decoded as {0} string.", (object) encoding.WebName), innerException);
    }

    private sealed class SeekableUnpackingStream : UnpackingStream
    {
      private readonly long _initialPosition;

      public override bool CanSeek
      {
        get
        {
          return true;
        }
      }

      public override long Position
      {
        get
        {
          return this.CurrentOffset;
        }
        set
        {
          this.SeekTo(value);
        }
      }

      public SeekableUnpackingStream(Stream underlying, long rawLength)
        : base(underlying, rawLength)
      {
        this._initialPosition = underlying.Position;
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        switch (origin)
        {
          case SeekOrigin.Begin:
            this.SeekTo(offset);
            break;
          case SeekOrigin.Current:
            this.SeekTo(this.Position + offset);
            break;
          case SeekOrigin.End:
            this.SeekTo(this.RawLength + offset);
            break;
        }
        return this.Position;
      }

      private void SeekTo(long position)
      {
        if (position < 0L)
          throw new IOException("An attempt was made to move the position before the beginning of the stream.");
        if (position > this.RawLength)
          this.SetLength(position);
        this.CurrentOffset = position;
        this.Underlying.Position = position + this._initialPosition;
      }
    }

    private sealed class UnseekableUnpackingStream : UnpackingStream
    {
      public override bool CanSeek
      {
        get
        {
          return false;
        }
      }

      public override long Position
      {
        get
        {
          throw new NotSupportedException();
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      public UnseekableUnpackingStream(Stream underlying, long rawLength)
        : base(underlying, rawLength)
      {
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        throw new NotSupportedException();
      }
    }

    private sealed class DefaultUnpackingStreamReader : UnpackingStreamReader
    {
      public DefaultUnpackingStreamReader(Stream stream, Encoding encoding, long byteLength)
        : base(stream, encoding, byteLength)
      {
      }
    }
  }
}
