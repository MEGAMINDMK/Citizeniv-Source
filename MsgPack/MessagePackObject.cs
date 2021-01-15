// Decompiled with JetBrains decompiler
// Type: MsgPack.MessagePackObject
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace MsgPack
{
  [Serializable]
  [StructLayout(LayoutKind.Auto)]
  public struct MessagePackObject : IEquatable<MessagePackObject>, IPackable
  {
    private static readonly MessagePackObject.ValueTypeCode _sbyteTypeCode = new MessagePackObject.ValueTypeCode(typeof (sbyte), MessagePackObject.MessagePackValueTypeCode.Int8);
    private static readonly MessagePackObject.ValueTypeCode _byteTypeCode = new MessagePackObject.ValueTypeCode(typeof (byte), MessagePackObject.MessagePackValueTypeCode.UInt8);
    private static readonly MessagePackObject.ValueTypeCode _int16TypeCode = new MessagePackObject.ValueTypeCode(typeof (short), MessagePackObject.MessagePackValueTypeCode.Int16);
    private static readonly MessagePackObject.ValueTypeCode _uint16TypeCode = new MessagePackObject.ValueTypeCode(typeof (ushort), MessagePackObject.MessagePackValueTypeCode.UInt16);
    private static readonly MessagePackObject.ValueTypeCode _int32TypeCode = new MessagePackObject.ValueTypeCode(typeof (int), MessagePackObject.MessagePackValueTypeCode.Int32);
    private static readonly MessagePackObject.ValueTypeCode _uint32TypeCode = new MessagePackObject.ValueTypeCode(typeof (uint), MessagePackObject.MessagePackValueTypeCode.UInt32);
    private static readonly MessagePackObject.ValueTypeCode _int64TypeCode = new MessagePackObject.ValueTypeCode(typeof (long), MessagePackObject.MessagePackValueTypeCode.Int64);
    private static readonly MessagePackObject.ValueTypeCode _uint64TypeCode = new MessagePackObject.ValueTypeCode(typeof (ulong), MessagePackObject.MessagePackValueTypeCode.UInt64);
    private static readonly MessagePackObject.ValueTypeCode _singleTypeCode = new MessagePackObject.ValueTypeCode(typeof (float), MessagePackObject.MessagePackValueTypeCode.Single);
    private static readonly MessagePackObject.ValueTypeCode _doubleTypeCode = new MessagePackObject.ValueTypeCode(typeof (double), MessagePackObject.MessagePackValueTypeCode.Double);
    private static readonly MessagePackObject.ValueTypeCode _booleanTypeCode = new MessagePackObject.ValueTypeCode(typeof (bool), MessagePackObject.MessagePackValueTypeCode.Boolean);
    public static readonly MessagePackObject Nil = new MessagePackObject();
    private object _handleOrTypeCode;
    private ulong _value;

    public bool IsNil
    {
      get
      {
        return this._handleOrTypeCode == null;
      }
    }

    public MessagePackObject(IList<MessagePackObject> value)
      : this(value, false)
    {
    }

    public MessagePackObject(IList<MessagePackObject> value, bool isImmutable)
      : this()
    {
      if (isImmutable)
        this._handleOrTypeCode = (object) value;
      else if (value == null)
      {
        this._handleOrTypeCode = (object) null;
      }
      else
      {
        MessagePackObject[] array = new MessagePackObject[value.Count];
        value.CopyTo(array, 0);
        this._handleOrTypeCode = (object) array;
      }
    }

    public MessagePackObject(MessagePackObjectDictionary value)
      : this(value, false)
    {
    }

    public MessagePackObject(MessagePackObjectDictionary value, bool isImmutable)
      : this()
    {
      if (isImmutable)
        this._handleOrTypeCode = (object) value;
      else if (value == null)
        this._handleOrTypeCode = (object) null;
      else
        this._handleOrTypeCode = (object) new MessagePackObjectDictionary((IDictionary<MessagePackObject, MessagePackObject>) value);
    }

    internal MessagePackObject(MessagePackString messagePackString)
      : this()
    {
      this._handleOrTypeCode = (object) messagePackString;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(obj, (object) null))
        return this.IsNil;
      switch (obj)
      {
        case MessagePackObjectDictionary objectDictionary:
          return this.Equals(new MessagePackObject(objectDictionary));
        case MessagePackObject other:
          return this.Equals(other);
        default:
          return false;
      }
    }

    public bool Equals(MessagePackObject other)
    {
      if (this._handleOrTypeCode == null)
        return other._handleOrTypeCode == null;
      if (other._handleOrTypeCode == null)
        return false;
      if (this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode)
        return other._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode && this.EqualsWhenValueType(other, handleOrTypeCode, handleOrTypeCode);
      if (this._handleOrTypeCode is MessagePackString handleOrTypeCode)
        return handleOrTypeCode.Equals((object) (other._handleOrTypeCode as MessagePackString));
      if (this._handleOrTypeCode is IList<MessagePackObject> handleOrTypeCode)
        return other._handleOrTypeCode is IList<MessagePackObject> handleOrTypeCode && handleOrTypeCode.SequenceEqual<MessagePackObject>((IEnumerable<MessagePackObject>) handleOrTypeCode, (IEqualityComparer<MessagePackObject>) MessagePackObjectEqualityComparer.Instance);
      if (this._handleOrTypeCode is IDictionary<MessagePackObject, MessagePackObject> handleOrTypeCode)
      {
        if (!(other._handleOrTypeCode is IDictionary<MessagePackObject, MessagePackObject> handleOrTypeCode) || handleOrTypeCode.Count != handleOrTypeCode.Count)
          return false;
        foreach (KeyValuePair<MessagePackObject, MessagePackObject> keyValuePair in (IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>) handleOrTypeCode)
        {
          MessagePackObject messagePackObject;
          if (!handleOrTypeCode.TryGetValue(keyValuePair.Key, out messagePackObject) || keyValuePair.Value != messagePackObject)
            return false;
        }
        return true;
      }
      if (!(this._handleOrTypeCode is byte[] handleOrTypeCode))
        return this._handleOrTypeCode.Equals(other._handleOrTypeCode);
      return other._handleOrTypeCode is byte[] handleOrTypeCode && MessagePackExtendedTypeObject.Unpack((byte) this._value, handleOrTypeCode) == MessagePackExtendedTypeObject.Unpack((byte) other._value, handleOrTypeCode);
    }

    private bool EqualsWhenValueType(
      MessagePackObject other,
      MessagePackObject.ValueTypeCode valueTypeCode,
      MessagePackObject.ValueTypeCode otherValuetypeCode)
    {
      if (valueTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Boolean)
        return otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Boolean && (bool) this == (bool) other;
      if (otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Boolean)
        return false;
      if (valueTypeCode.IsInteger)
      {
        if (otherValuetypeCode.IsInteger)
          return MessagePackObject.IntegerIntegerEquals(this._value, valueTypeCode, other._value, otherValuetypeCode);
        if (otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
          return MessagePackObject.IntegerSingleEquals(this, other);
        if (otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
          return MessagePackObject.IntegerDoubleEquals(this, other);
      }
      else if (valueTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
      {
        if (otherValuetypeCode.IsInteger)
          return MessagePackObject.IntegerDoubleEquals(other, this);
        if (otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
          return (double) this == (double) (float) other;
        if (otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
          return (double) this == (double) other;
      }
      else if (valueTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
      {
        if (otherValuetypeCode.IsInteger)
          return MessagePackObject.IntegerSingleEquals(other, this);
        if (otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
          return (double) (float) this == (double) (float) other;
        if (otherValuetypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
          return (double) (float) this == (double) other;
      }
      return false;
    }

    private static bool IntegerIntegerEquals(
      ulong left,
      MessagePackObject.ValueTypeCode leftTypeCode,
      ulong right,
      MessagePackObject.ValueTypeCode rightTypeCode)
    {
      return leftTypeCode.IsSigned ? (rightTypeCode.IsSigned || (long) left >= 0L) && (long) left == (long) right : (!rightTypeCode.IsSigned || (long) right >= 0L) && (long) left == (long) right;
    }

    private static bool IntegerSingleEquals(MessagePackObject integer, MessagePackObject real)
    {
      return (integer._handleOrTypeCode as MessagePackObject.ValueTypeCode).IsSigned ? (double) (long) integer._value == (double) (float) real : (double) integer._value == (double) (float) real;
    }

    private static bool IntegerDoubleEquals(MessagePackObject integer, MessagePackObject real)
    {
      return (integer._handleOrTypeCode as MessagePackObject.ValueTypeCode).IsSigned ? (double) (long) integer._value == (double) real : (double) integer._value == (double) real;
    }

    public override int GetHashCode()
    {
      if (this._handleOrTypeCode == null)
        return 0;
      if (this._handleOrTypeCode is MessagePackObject.ValueTypeCode)
        return this._value.GetHashCode();
      if (this._handleOrTypeCode is MessagePackString handleOrTypeCode)
        return handleOrTypeCode.GetHashCode();
      if (this._handleOrTypeCode is IList<MessagePackObject> handleOrTypeCode)
      {
        Func<int, MessagePackObject, int> func = (Func<int, MessagePackObject, int>) ((hash, item) => hash ^ item.GetHashCode());
        return handleOrTypeCode.Aggregate<MessagePackObject, int>(0, func);
      }
      if (this._handleOrTypeCode is IDictionary<MessagePackObject, MessagePackObject> handleOrTypeCode)
      {
        Func<int, KeyValuePair<MessagePackObject, MessagePackObject>, int> func = (Func<int, KeyValuePair<MessagePackObject, MessagePackObject>, int>) ((hash, item) => hash ^ item.GetHashCode());
        return handleOrTypeCode.Aggregate<KeyValuePair<MessagePackObject, MessagePackObject>, int>(0, func);
      }
      return this._handleOrTypeCode is byte[] handleOrTypeCode ? MessagePackExtendedTypeObject.Unpack((byte) this._value, handleOrTypeCode).GetHashCode() : 0;
    }

    public override string ToString()
    {
      StringBuilder buffer = new StringBuilder();
      this.ToString(buffer, false);
      return buffer.ToString();
    }

    private void ToString(StringBuilder buffer, bool isJson)
    {
      if (this._handleOrTypeCode == null)
      {
        if (!isJson)
          return;
        buffer.Append("null");
      }
      else if (this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode)
      {
        switch (handleOrTypeCode.TypeCode)
        {
          case MessagePackObject.MessagePackValueTypeCode.Boolean:
            if (isJson)
            {
              buffer.Append(this.AsBoolean() ? "true" : "false");
              break;
            }
            buffer.Append(this.AsBoolean());
            break;
          case MessagePackObject.MessagePackValueTypeCode.Single:
            buffer.Append(this.AsSingle().ToString((IFormatProvider) CultureInfo.InvariantCulture));
            break;
          case MessagePackObject.MessagePackValueTypeCode.Double:
            buffer.Append(this.AsDouble().ToString((IFormatProvider) CultureInfo.InvariantCulture));
            break;
          default:
            if (handleOrTypeCode.IsSigned)
            {
              buffer.Append(((long) this._value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
              break;
            }
            buffer.Append(this._value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            break;
        }
      }
      else if (this._handleOrTypeCode is IList<MessagePackObject> handleOrTypeCode)
      {
        buffer.Append('[');
        if (handleOrTypeCode.Count > 0)
        {
          for (int index = 0; index < handleOrTypeCode.Count; ++index)
          {
            if (index > 0)
              buffer.Append(',');
            buffer.Append(' ');
            handleOrTypeCode[index].ToString(buffer, true);
          }
          buffer.Append(' ');
        }
        buffer.Append(']');
      }
      else if (this._handleOrTypeCode is IDictionary<MessagePackObject, MessagePackObject> handleOrTypeCode)
      {
        buffer.Append('{');
        if (handleOrTypeCode.Count > 0)
        {
          bool flag = true;
          foreach (KeyValuePair<MessagePackObject, MessagePackObject> keyValuePair in (IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>) handleOrTypeCode)
          {
            if (flag)
              flag = false;
            else
              buffer.Append(',');
            buffer.Append(' ');
            keyValuePair.Key.ToString(buffer, true);
            buffer.Append(' ').Append(':').Append(' ');
            keyValuePair.Value.ToString(buffer, true);
          }
          buffer.Append(' ');
        }
        buffer.Append('}');
      }
      else if (this._handleOrTypeCode is MessagePackString handleOrTypeCode)
        MessagePackObject.ToStringBinary(buffer, isJson, handleOrTypeCode);
      else if (this._handleOrTypeCode is byte[] handleOrTypeCode)
        MessagePackExtendedTypeObject.Unpack((byte) this._value, handleOrTypeCode).ToString(buffer, isJson);
      else if (isJson)
        buffer.Append('"').Append(this._handleOrTypeCode).Append('"');
      else
        buffer.Append(this._handleOrTypeCode);
    }

    private static void ToStringBinary(
      StringBuilder buffer,
      bool isJson,
      MessagePackString asBinary)
    {
      string str = asBinary.TryGetString();
      if (str != null)
      {
        if (isJson)
        {
          buffer.Append('"');
          foreach (char ch in str)
          {
            switch (ch)
            {
              case '\b':
                buffer.Append('\\').Append('b');
                break;
              case '\t':
                buffer.Append('\\').Append('t');
                break;
              case '\n':
                buffer.Append('\\').Append('n');
                break;
              case '\f':
                buffer.Append('\\').Append('f');
                break;
              case '\r':
                buffer.Append('\\').Append('r');
                break;
              case ' ':
                buffer.Append(' ');
                break;
              case '"':
                buffer.Append('\\').Append('"');
                break;
              case '/':
                buffer.Append('\\').Append('/');
                break;
              case '\\':
                buffer.Append('\\').Append('\\');
                break;
              default:
                switch (CharUnicodeInfo.GetUnicodeCategory(ch))
                {
                  case UnicodeCategory.SpaceSeparator:
                  case UnicodeCategory.LineSeparator:
                  case UnicodeCategory.ParagraphSeparator:
                  case UnicodeCategory.Control:
                  case UnicodeCategory.Format:
                  case UnicodeCategory.Surrogate:
                  case UnicodeCategory.PrivateUse:
                  case UnicodeCategory.OtherNotAssigned:
                    buffer.Append('\\').Append('u').Append(((ushort) ch).ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
                    continue;
                  default:
                    buffer.Append(ch);
                    continue;
                }
            }
          }
          buffer.Append('"');
        }
        else
          buffer.Append(str);
      }
      else
      {
        byte[] buffer1 = asBinary.UnsafeGetBuffer();
        if (buffer1 == null)
          return;
        if (isJson)
        {
          buffer.Append('"');
          Binary.ToHexString(buffer1, buffer);
          buffer.Append('"');
        }
        else
          Binary.ToHexString(buffer1, buffer);
      }
    }

    public bool? IsTypeOf<T>()
    {
      return this.IsTypeOf(typeof (T));
    }

    public bool? IsTypeOf(Type type)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      if (this._handleOrTypeCode == null)
        return !type.IsValueType || !(Nullable.GetUnderlyingType(type) == (Type) null) ? new bool?() : new bool?(false);
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
      {
        if (type == typeof (MessagePackExtendedTypeObject))
          return new bool?(this._handleOrTypeCode is byte[]);
        if (type == typeof (string) || type == typeof (IList<char>) || type == typeof (IEnumerable<char>))
          return new bool?(this._handleOrTypeCode is MessagePackString handleOrTypeCode && handleOrTypeCode.GetUnderlyingType() == typeof (string));
        if (type == typeof (byte[]) || type == typeof (IList<byte>) || type == typeof (IEnumerable<byte>))
          return new bool?(this._handleOrTypeCode is MessagePackString);
        return typeof (IEnumerable<MessagePackObject>).IsAssignableFrom(type) && this._handleOrTypeCode is MessagePackString ? new bool?(false) : new bool?(type.IsAssignableFrom(this._handleOrTypeCode.GetType()));
      }
      switch (Type.GetTypeCode(type))
      {
        case TypeCode.SByte:
          return new bool?(handleOrTypeCode.IsInteger && (this._value < 128UL || 18446744073709551488UL <= this._value && handleOrTypeCode.IsSigned));
        case TypeCode.Byte:
          return new bool?(handleOrTypeCode.IsInteger && this._value < 256UL);
        case TypeCode.Int16:
          return new bool?(handleOrTypeCode.IsInteger && (this._value < 32768UL || 18446744073709518848UL <= this._value && handleOrTypeCode.IsSigned));
        case TypeCode.UInt16:
          return new bool?(handleOrTypeCode.IsInteger && this._value < 65536UL);
        case TypeCode.Int32:
          return new bool?(handleOrTypeCode.IsInteger && (this._value < 2147483648UL || 18446744071562067968UL <= this._value && handleOrTypeCode.IsSigned));
        case TypeCode.UInt32:
          return new bool?(handleOrTypeCode.IsInteger && this._value < 4294967296UL);
        case TypeCode.Int64:
          return new bool?(handleOrTypeCode.IsInteger && (this._value < 9223372036854775808UL || handleOrTypeCode.IsSigned));
        case TypeCode.UInt64:
          return new bool?(handleOrTypeCode.IsInteger && (this._value < 9223372036854775808UL || !handleOrTypeCode.IsSigned));
        case TypeCode.Double:
          return new bool?(handleOrTypeCode.Type == typeof (float) || handleOrTypeCode.Type == typeof (double));
        default:
          return new bool?(handleOrTypeCode.Type == type);
      }
    }

    public bool IsRaw
    {
      get
      {
        return !this.IsNil && this._handleOrTypeCode is MessagePackString;
      }
    }

    public bool IsList
    {
      get
      {
        return !this.IsNil && this._handleOrTypeCode is IList<MessagePackObject>;
      }
    }

    public bool IsArray
    {
      get
      {
        return this.IsList;
      }
    }

    public bool IsDictionary
    {
      get
      {
        return !this.IsNil && this._handleOrTypeCode is IDictionary<MessagePackObject, MessagePackObject>;
      }
    }

    public bool IsMap
    {
      get
      {
        return this.IsDictionary;
      }
    }

    public Type UnderlyingType
    {
      get
      {
        if (this._handleOrTypeCode == null)
          return (Type) null;
        if (this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode)
          return handleOrTypeCode.Type;
        return this._handleOrTypeCode is MessagePackString handleOrTypeCode ? handleOrTypeCode.GetUnderlyingType() : this._handleOrTypeCode.GetType();
      }
    }

    public void PackToMessage(Packer packer, PackingOptions options)
    {
      if (packer == null)
        throw new ArgumentNullException(nameof (packer));
      if (this._handleOrTypeCode == null)
        packer.PackNull();
      else if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
      {
        if (this._handleOrTypeCode is MessagePackString handleOrTypeCode)
          packer.PackRaw(handleOrTypeCode.GetBytes());
        else if (this._handleOrTypeCode is IList<MessagePackObject> handleOrTypeCode)
        {
          packer.PackArrayHeader(handleOrTypeCode.Count);
          foreach (MessagePackObject messagePackObject in (IEnumerable<MessagePackObject>) handleOrTypeCode)
            messagePackObject.PackToMessage(packer, options);
        }
        else if (this._handleOrTypeCode is IDictionary<MessagePackObject, MessagePackObject> handleOrTypeCode)
        {
          packer.PackMapHeader(handleOrTypeCode.Count);
          foreach (KeyValuePair<MessagePackObject, MessagePackObject> keyValuePair in (IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>) handleOrTypeCode)
          {
            keyValuePair.Key.PackToMessage(packer, options);
            keyValuePair.Value.PackToMessage(packer, options);
          }
        }
        else
        {
          if (!(this._handleOrTypeCode is byte[] handleOrTypeCode))
            throw new SerializationException("Failed to pack this object.");
          packer.PackExtendedTypeValue((byte) this._value, handleOrTypeCode);
        }
      }
      else
      {
        switch (handleOrTypeCode.TypeCode)
        {
          case MessagePackObject.MessagePackValueTypeCode.Int8:
            packer.Pack((sbyte) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.UInt8:
            packer.Pack((byte) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.Int16:
            packer.Pack((short) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.UInt16:
            packer.Pack((ushort) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.Int32:
            packer.Pack((int) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.UInt32:
            packer.Pack((uint) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.Int64:
            packer.Pack((long) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.UInt64:
            packer.Pack(this._value);
            break;
          case MessagePackObject.MessagePackValueTypeCode.Boolean:
            packer.Pack(this._value != 0UL);
            break;
          case MessagePackObject.MessagePackValueTypeCode.Single:
            packer.Pack((float) this);
            break;
          case MessagePackObject.MessagePackValueTypeCode.Double:
            packer.Pack((double) this);
            break;
          default:
            throw new SerializationException("Failed to pack this object.");
        }
      }
    }

    public string AsString(Encoding encoding)
    {
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      if (this.IsNil)
        return (string) null;
      MessagePackObject.VerifyUnderlyingType<MessagePackString>(this, (string) null);
      try
      {
        MessagePackString handleOrTypeCode = this._handleOrTypeCode as MessagePackString;
        return encoding is UTF8Encoding ? handleOrTypeCode.GetString() : encoding.GetString(handleOrTypeCode.UnsafeGetBuffer(), 0, handleOrTypeCode.UnsafeGetBuffer().Length);
      }
      catch (ArgumentException ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Not '{0}' string.", (object) encoding.WebName), (Exception) ex);
      }
    }

    public string AsStringUtf8()
    {
      return this.AsString(MessagePackConvert.Utf8NonBomStrict);
    }

    public string AsStringUtf16()
    {
      MessagePackObject.VerifyUnderlyingType<byte[]>(this, (string) null);
      if (this.IsNil)
        return (string) null;
      try
      {
        MessagePackString handleOrTypeCode = this._handleOrTypeCode as MessagePackString;
        if (handleOrTypeCode.UnsafeGetString() != null)
          return handleOrTypeCode.UnsafeGetString();
        byte[] buffer = handleOrTypeCode.UnsafeGetBuffer();
        if (buffer.Length == 0)
          return string.Empty;
        if (buffer.Length % 2 != 0)
          throw new InvalidOperationException("Not UTF-16 string.");
        if (buffer[0] == byte.MaxValue && buffer[1] == (byte) 254)
          return Encoding.Unicode.GetString(buffer, 2, buffer.Length - 2);
        return buffer[0] == (byte) 254 && buffer[1] == byte.MaxValue ? Encoding.BigEndianUnicode.GetString(buffer, 2, buffer.Length - 2) : Encoding.BigEndianUnicode.GetString(buffer, 0, buffer.Length);
      }
      catch (ArgumentException ex)
      {
        throw new InvalidOperationException("Not UTF-16 string.", (Exception) ex);
      }
    }

    public char[] AsCharArray()
    {
      return this.AsString().ToCharArray();
    }

    public IEnumerable<MessagePackObject> AsEnumerable()
    {
      if (this.IsNil)
        return (IEnumerable<MessagePackObject>) null;
      MessagePackObject.VerifyUnderlyingType<IEnumerable<MessagePackObject>>(this, (string) null);
      return this._handleOrTypeCode as IEnumerable<MessagePackObject>;
    }

    public IList<MessagePackObject> AsList()
    {
      if (this.IsNil)
        return (IList<MessagePackObject>) null;
      MessagePackObject.VerifyUnderlyingType<IList<MessagePackObject>>(this, (string) null);
      return this._handleOrTypeCode as IList<MessagePackObject>;
    }

    public MessagePackObjectDictionary AsDictionary()
    {
      MessagePackObject.VerifyUnderlyingType<MessagePackObjectDictionary>(this, (string) null);
      return this._handleOrTypeCode as MessagePackObjectDictionary;
    }

    private static void VerifyUnderlyingType<T>(MessagePackObject instance, string parameterName)
    {
      if (instance.IsNil)
      {
        if (!typeof (T).IsValueType || Nullable.GetUnderlyingType(typeof (T)) != (Type) null)
          return;
        if (parameterName != null)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Do not convert nil MessagePackObject to {0}.", (object) typeof (T)), parameterName);
        MessagePackObject.ThrowCannotBeNilAs<T>();
      }
      if (instance.IsTypeOf<T>().GetValueOrDefault())
        return;
      if (parameterName != null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Do not convert {0} MessagePackObject to {1}.", (object) instance.UnderlyingType, (object) typeof (T)), parameterName);
      MessagePackObject.ThrowInvalidTypeAs<T>(instance);
    }

    private static void ThrowCannotBeNilAs<T>()
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Do not convert nil MessagePackObject to {0}.", (object) typeof (T)));
    }

    private static void ThrowInvalidTypeAs<T>(MessagePackObject instance)
    {
      if (instance._handleOrTypeCode is MessagePackObject.ValueTypeCode)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Do not convert {0} (binary:0x{2:x}) MessagePackObject to {1}.", (object) instance.UnderlyingType, (object) typeof (T), (object) instance._value));
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Do not convert {0} MessagePackObject to {1}.", (object) instance.UnderlyingType, (object) typeof (T)));
    }

    public static MessagePackObject FromObject(object boxedValue)
    {
      switch (boxedValue)
      {
        case null:
          return MessagePackObject.Nil;
        case MessagePackObject messagePackObject:
          return messagePackObject;
        case sbyte num:
          return (MessagePackObject) num;
        case byte num:
          return (MessagePackObject) num;
        case short num:
          return (MessagePackObject) num;
        case ushort num:
          return (MessagePackObject) num;
        case int num:
          return (MessagePackObject) num;
        case uint num:
          return (MessagePackObject) num;
        case long num:
          return (MessagePackObject) num;
        case ulong num:
          return (MessagePackObject) num;
        case float num:
          return (MessagePackObject) num;
        case double num:
          return (MessagePackObject) num;
        case bool flag:
          return (MessagePackObject) flag;
        case byte[] numArray:
          return new MessagePackObject(numArray);
        case string str:
          return new MessagePackObject(str);
        case IEnumerable<byte> source:
          return new MessagePackObject(source.ToArray<byte>());
        case IEnumerable<char> source:
          return new MessagePackObject(new string(source.ToArray<char>()));
        case IEnumerable<MessagePackObject> source:
          return boxedValue is IList<MessagePackObject> messagePackObjectList ? new MessagePackObject(messagePackObjectList, false) : new MessagePackObject((IList<MessagePackObject>) source.ToArray<MessagePackObject>(), true);
        case MessagePackObjectDictionary objectDictionary:
          return new MessagePackObject(objectDictionary, false);
        case MessagePackExtendedTypeObject extendedTypeObject:
          return new MessagePackObject(extendedTypeObject);
        default:
          throw new MessageTypeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not supported.", (object) boxedValue.GetType()));
      }
    }

    public object ToObject()
    {
      if (this._handleOrTypeCode == null)
        return (object) null;
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
      {
        if (this._handleOrTypeCode is MessagePackString handleOrTypeCode)
          return (object) handleOrTypeCode.TryGetString() ?? (object) handleOrTypeCode.UnsafeGetBuffer();
        if (this._handleOrTypeCode is IList<MessagePackObject> handleOrTypeCode)
          return (object) handleOrTypeCode;
        if (this._handleOrTypeCode is IDictionary<MessagePackObject, MessagePackObject> handleOrTypeCode)
          return (object) handleOrTypeCode;
        return this._handleOrTypeCode is byte[] handleOrTypeCode ? (object) MessagePackExtendedTypeObject.Unpack((byte) this._value, handleOrTypeCode) : (object) null;
      }
      switch (handleOrTypeCode.TypeCode)
      {
        case MessagePackObject.MessagePackValueTypeCode.Int8:
          return (object) this.AsSByte();
        case MessagePackObject.MessagePackValueTypeCode.UInt8:
          return (object) this.AsByte();
        case MessagePackObject.MessagePackValueTypeCode.Int16:
          return (object) this.AsInt16();
        case MessagePackObject.MessagePackValueTypeCode.UInt16:
          return (object) this.AsUInt16();
        case MessagePackObject.MessagePackValueTypeCode.Int32:
          return (object) this.AsInt32();
        case MessagePackObject.MessagePackValueTypeCode.UInt32:
          return (object) this.AsUInt32();
        case MessagePackObject.MessagePackValueTypeCode.Int64:
          return (object) this.AsInt64();
        case MessagePackObject.MessagePackValueTypeCode.UInt64:
          return (object) this.AsUInt64();
        case MessagePackObject.MessagePackValueTypeCode.Boolean:
          return (object) this.AsBoolean();
        case MessagePackObject.MessagePackValueTypeCode.Single:
          return (object) this.AsSingle();
        case MessagePackObject.MessagePackValueTypeCode.Double:
          return (object) this.AsDouble();
        default:
          return (object) null;
      }
    }

    public static bool operator ==(MessagePackObject left, MessagePackObject right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(MessagePackObject left, MessagePackObject right)
    {
      return !left.Equals(right);
    }

    public static implicit operator MessagePackObject(MessagePackObject[] value)
    {
      return new MessagePackObject((IList<MessagePackObject>) value, false);
    }

    public MessagePackObject(bool value)
      : this()
    {
      this._value = value ? 1UL : 0UL;
      this._handleOrTypeCode = (object) MessagePackObject._booleanTypeCode;
    }

    public MessagePackObject(byte value)
      : this()
    {
      this._value = (ulong) value;
      this._handleOrTypeCode = (object) MessagePackObject._byteTypeCode;
    }

    [CLSCompliant(false)]
    public MessagePackObject(sbyte value)
      : this()
    {
      this._value = (ulong) value;
      this._handleOrTypeCode = (object) MessagePackObject._sbyteTypeCode;
    }

    public MessagePackObject(short value)
      : this()
    {
      this._value = (ulong) value;
      this._handleOrTypeCode = (object) MessagePackObject._int16TypeCode;
    }

    [CLSCompliant(false)]
    public MessagePackObject(ushort value)
      : this()
    {
      this._value = (ulong) value;
      this._handleOrTypeCode = (object) MessagePackObject._uint16TypeCode;
    }

    public MessagePackObject(int value)
      : this()
    {
      this._value = (ulong) value;
      this._handleOrTypeCode = (object) MessagePackObject._int32TypeCode;
    }

    [CLSCompliant(false)]
    public MessagePackObject(uint value)
      : this()
    {
      this._value = (ulong) value;
      this._handleOrTypeCode = (object) MessagePackObject._uint32TypeCode;
    }

    public MessagePackObject(long value)
      : this()
    {
      this._value = (ulong) value;
      this._handleOrTypeCode = (object) MessagePackObject._int64TypeCode;
    }

    [CLSCompliant(false)]
    public MessagePackObject(ulong value)
      : this()
    {
      this._value = value;
      this._handleOrTypeCode = (object) MessagePackObject._uint64TypeCode;
    }

    public MessagePackObject(float value)
      : this()
    {
      byte[] bytes = BitConverter.GetBytes(value);
      if (BitConverter.IsLittleEndian)
      {
        this._value |= (ulong) ((int) bytes[3] << 24);
        this._value |= (ulong) ((int) bytes[2] << 16);
        this._value |= (ulong) ((int) bytes[1] << 8);
        this._value |= (ulong) bytes[0];
      }
      else
      {
        this._value |= (ulong) ((int) bytes[0] << 24);
        this._value |= (ulong) ((int) bytes[1] << 16);
        this._value |= (ulong) ((int) bytes[2] << 8);
        this._value |= (ulong) bytes[3];
      }
      this._handleOrTypeCode = (object) MessagePackObject._singleTypeCode;
    }

    public MessagePackObject(double value)
      : this()
    {
      this._value = (ulong) BitConverter.DoubleToInt64Bits(value);
      this._handleOrTypeCode = (object) MessagePackObject._doubleTypeCode;
    }

    public MessagePackObject(string value)
      : this()
    {
      if (value == null)
        this._handleOrTypeCode = (object) null;
      else
        this._handleOrTypeCode = (object) new MessagePackString(value);
    }

    public MessagePackObject(byte[] value)
      : this(value, false)
    {
    }

    public MessagePackObject(byte[] value, bool isBinary)
      : this()
    {
      if (value == null)
        this._handleOrTypeCode = (object) null;
      else
        this._handleOrTypeCode = (object) new MessagePackString(value, isBinary);
    }

    public MessagePackObject(MessagePackExtendedTypeObject value)
      : this()
    {
      this._value = (ulong) value.TypeCode;
      this._handleOrTypeCode = (object) value.Body;
    }

    public bool AsBoolean()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<bool>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode) || handleOrTypeCode.TypeCode != MessagePackObject.MessagePackValueTypeCode.Boolean)
        MessagePackObject.ThrowInvalidTypeAs<bool>(this);
      return this._value != 0UL;
    }

    public byte AsByte()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<byte>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<byte>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) this._value;
          if (num < 0L || (long) byte.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<byte>(this);
          return (byte) num;
        }
        if ((ulong) byte.MaxValue < this._value)
          MessagePackObject.ThrowInvalidTypeAs<byte>(this);
        return (byte) this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (byte) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (byte) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<byte>(this);
      return 0;
    }

    [CLSCompliant(false)]
    public sbyte AsSByte()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<sbyte>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<sbyte>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) this._value;
          if (num < (long) sbyte.MinValue || (long) sbyte.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<sbyte>(this);
          return (sbyte) num;
        }
        if ((ulong) sbyte.MaxValue < this._value)
          MessagePackObject.ThrowInvalidTypeAs<sbyte>(this);
        return (sbyte) this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (sbyte) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (sbyte) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<sbyte>(this);
      return 0;
    }

    public short AsInt16()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<short>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<short>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) this._value;
          if (num < (long) short.MinValue || (long) short.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<short>(this);
          return (short) num;
        }
        if ((ulong) short.MaxValue < this._value)
          MessagePackObject.ThrowInvalidTypeAs<short>(this);
        return (short) this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (short) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (short) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<short>(this);
      return 0;
    }

    [CLSCompliant(false)]
    public ushort AsUInt16()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<ushort>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<ushort>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if ((ulong) ushort.MaxValue < this._value)
          MessagePackObject.ThrowInvalidTypeAs<ushort>(this);
        return (ushort) this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (ushort) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (ushort) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<ushort>(this);
      return 0;
    }

    public int AsInt32()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<int>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<int>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) this._value;
          if (num < (long) int.MinValue || (long) int.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<int>(this);
          return (int) num;
        }
        if ((ulong) int.MaxValue < this._value)
          MessagePackObject.ThrowInvalidTypeAs<int>(this);
        return (int) this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (int) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (int) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<int>(this);
      return 0;
    }

    [CLSCompliant(false)]
    public uint AsUInt32()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<uint>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<uint>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if ((ulong) uint.MaxValue < this._value)
          MessagePackObject.ThrowInvalidTypeAs<uint>(this);
        return (uint) this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (uint) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (uint) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<uint>(this);
      return 0;
    }

    public long AsInt64()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<long>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<long>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned || (ulong) long.MaxValue >= this._value)
          return (long) this._value;
        MessagePackObject.ThrowInvalidTypeAs<long>(this);
        return (long) this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (long) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (long) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<long>(this);
      return 0;
    }

    [CLSCompliant(false)]
    public ulong AsUInt64()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<ulong>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<ulong>(this);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned && (ulong) long.MaxValue < this._value)
          MessagePackObject.ThrowInvalidTypeAs<ulong>(this);
        return this._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (ulong) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (ulong) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<ulong>(this);
      return 0;
    }

    public float AsSingle()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<float>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<float>(this);
      if (handleOrTypeCode.IsInteger)
        return handleOrTypeCode.IsSigned ? (float) (long) this._value : (float) this._value;
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (float) BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<float>(this);
      return 0.0f;
    }

    public double AsDouble()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<double>();
      if (!(this._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<double>(this);
      if (handleOrTypeCode.IsInteger)
        return handleOrTypeCode.IsSigned ? (double) (long) this._value : (double) this._value;
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return BitConverter.Int64BitsToDouble((long) this._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (double) BitConverter.ToSingle(BitConverter.GetBytes(this._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<double>(this);
      return 0.0;
    }

    public string AsString()
    {
      MessagePackObject.VerifyUnderlyingType<MessagePackString>(this, (string) null);
      return this._handleOrTypeCode == null ? (string) null : (this._handleOrTypeCode as MessagePackString).GetString();
    }

    public byte[] AsBinary()
    {
      MessagePackObject.VerifyUnderlyingType<MessagePackString>(this, (string) null);
      return this._handleOrTypeCode == null ? (byte[]) null : (this._handleOrTypeCode as MessagePackString).GetBytes();
    }

    public MessagePackExtendedTypeObject AsMessagePackExtendedTypeObject()
    {
      if (this.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<MessagePackExtendedTypeObject>();
      MessagePackObject.VerifyUnderlyingType<MessagePackExtendedTypeObject>(this, (string) null);
      return MessagePackExtendedTypeObject.Unpack((byte) this._value, this._handleOrTypeCode as byte[]);
    }

    public static implicit operator MessagePackObject(bool value)
    {
      return new MessagePackObject()
      {
        _value = value ? 1UL : 0UL,
        _handleOrTypeCode = (object) MessagePackObject._booleanTypeCode
      };
    }

    public static implicit operator MessagePackObject(byte value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value,
        _handleOrTypeCode = (object) MessagePackObject._byteTypeCode
      };
    }

    [CLSCompliant(false)]
    public static implicit operator MessagePackObject(sbyte value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value,
        _handleOrTypeCode = (object) MessagePackObject._sbyteTypeCode
      };
    }

    public static implicit operator MessagePackObject(short value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value,
        _handleOrTypeCode = (object) MessagePackObject._int16TypeCode
      };
    }

    [CLSCompliant(false)]
    public static implicit operator MessagePackObject(ushort value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value,
        _handleOrTypeCode = (object) MessagePackObject._uint16TypeCode
      };
    }

    public static implicit operator MessagePackObject(int value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value,
        _handleOrTypeCode = (object) MessagePackObject._int32TypeCode
      };
    }

    [CLSCompliant(false)]
    public static implicit operator MessagePackObject(uint value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value,
        _handleOrTypeCode = (object) MessagePackObject._uint32TypeCode
      };
    }

    public static implicit operator MessagePackObject(long value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value,
        _handleOrTypeCode = (object) MessagePackObject._int64TypeCode
      };
    }

    [CLSCompliant(false)]
    public static implicit operator MessagePackObject(ulong value)
    {
      return new MessagePackObject()
      {
        _value = value,
        _handleOrTypeCode = (object) MessagePackObject._uint64TypeCode
      };
    }

    public static implicit operator MessagePackObject(float value)
    {
      MessagePackObject messagePackObject = new MessagePackObject();
      byte[] bytes = BitConverter.GetBytes(value);
      if (BitConverter.IsLittleEndian)
      {
        messagePackObject._value |= (ulong) ((int) bytes[3] << 24);
        messagePackObject._value |= (ulong) ((int) bytes[2] << 16);
        messagePackObject._value |= (ulong) ((int) bytes[1] << 8);
        messagePackObject._value |= (ulong) bytes[0];
      }
      else
      {
        messagePackObject._value |= (ulong) ((int) bytes[0] << 24);
        messagePackObject._value |= (ulong) ((int) bytes[1] << 16);
        messagePackObject._value |= (ulong) ((int) bytes[2] << 8);
        messagePackObject._value |= (ulong) bytes[3];
      }
      messagePackObject._handleOrTypeCode = (object) MessagePackObject._singleTypeCode;
      return messagePackObject;
    }

    public static implicit operator MessagePackObject(double value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) BitConverter.DoubleToInt64Bits(value),
        _handleOrTypeCode = (object) MessagePackObject._doubleTypeCode
      };
    }

    public static implicit operator MessagePackObject(string value)
    {
      return new MessagePackObject()
      {
        _handleOrTypeCode = value != null ? (object) new MessagePackString(value) : (object) null
      };
    }

    public static implicit operator MessagePackObject(byte[] value)
    {
      return new MessagePackObject()
      {
        _handleOrTypeCode = value != null ? (object) new MessagePackString(value, false) : (object) null
      };
    }

    public static implicit operator MessagePackObject(
      MessagePackExtendedTypeObject value)
    {
      return new MessagePackObject()
      {
        _value = (ulong) value.TypeCode,
        _handleOrTypeCode = (object) value.Body
      };
    }

    public static explicit operator bool(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<bool>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode) || handleOrTypeCode.TypeCode != MessagePackObject.MessagePackValueTypeCode.Boolean)
        MessagePackObject.ThrowInvalidTypeAs<bool>(value);
      return value._value != 0UL;
    }

    public static explicit operator byte(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<byte>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<byte>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) value._value;
          if (num < 0L || (long) byte.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<byte>(value);
          return (byte) num;
        }
        if ((ulong) byte.MaxValue < value._value)
          MessagePackObject.ThrowInvalidTypeAs<byte>(value);
        return (byte) value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (byte) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (byte) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<byte>(value);
      return 0;
    }

    [CLSCompliant(false)]
    public static explicit operator sbyte(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<sbyte>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<sbyte>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) value._value;
          if (num < (long) sbyte.MinValue || (long) sbyte.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<sbyte>(value);
          return (sbyte) num;
        }
        if ((ulong) sbyte.MaxValue < value._value)
          MessagePackObject.ThrowInvalidTypeAs<sbyte>(value);
        return (sbyte) value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (sbyte) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (sbyte) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<sbyte>(value);
      return 0;
    }

    public static explicit operator short(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<short>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<short>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) value._value;
          if (num < (long) short.MinValue || (long) short.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<short>(value);
          return (short) num;
        }
        if ((ulong) short.MaxValue < value._value)
          MessagePackObject.ThrowInvalidTypeAs<short>(value);
        return (short) value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (short) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (short) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<short>(value);
      return 0;
    }

    [CLSCompliant(false)]
    public static explicit operator ushort(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<ushort>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<ushort>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if ((ulong) ushort.MaxValue < value._value)
          MessagePackObject.ThrowInvalidTypeAs<ushort>(value);
        return (ushort) value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (ushort) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (ushort) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<ushort>(value);
      return 0;
    }

    public static explicit operator int(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<int>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<int>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned)
        {
          long num = (long) value._value;
          if (num < (long) int.MinValue || (long) int.MaxValue < num)
            MessagePackObject.ThrowInvalidTypeAs<int>(value);
          return (int) num;
        }
        if ((ulong) int.MaxValue < value._value)
          MessagePackObject.ThrowInvalidTypeAs<int>(value);
        return (int) value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (int) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (int) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<int>(value);
      return 0;
    }

    [CLSCompliant(false)]
    public static explicit operator uint(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<uint>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<uint>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if ((ulong) uint.MaxValue < value._value)
          MessagePackObject.ThrowInvalidTypeAs<uint>(value);
        return (uint) value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (uint) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (uint) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<uint>(value);
      return 0;
    }

    public static explicit operator long(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<long>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<long>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned || (ulong) long.MaxValue >= value._value)
          return (long) value._value;
        MessagePackObject.ThrowInvalidTypeAs<long>(value);
        return (long) value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (long) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (long) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<long>(value);
      return 0;
    }

    [CLSCompliant(false)]
    public static explicit operator ulong(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<ulong>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<ulong>(value);
      if (handleOrTypeCode.IsInteger)
      {
        if (handleOrTypeCode.IsSigned && (ulong) long.MaxValue < value._value)
          MessagePackObject.ThrowInvalidTypeAs<ulong>(value);
        return value._value;
      }
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (ulong) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (ulong) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<ulong>(value);
      return 0;
    }

    public static explicit operator float(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<float>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<float>(value);
      if (handleOrTypeCode.IsInteger)
        return handleOrTypeCode.IsSigned ? (float) (long) value._value : (float) value._value;
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return (float) BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<float>(value);
      return 0.0f;
    }

    public static explicit operator double(MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<double>();
      if (!(value._handleOrTypeCode is MessagePackObject.ValueTypeCode handleOrTypeCode))
        MessagePackObject.ThrowInvalidTypeAs<double>(value);
      if (handleOrTypeCode.IsInteger)
        return handleOrTypeCode.IsSigned ? (double) (long) value._value : (double) value._value;
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Double)
        return BitConverter.Int64BitsToDouble((long) value._value);
      if (handleOrTypeCode.TypeCode == MessagePackObject.MessagePackValueTypeCode.Single)
        return (double) BitConverter.ToSingle(BitConverter.GetBytes(value._value), 0);
      MessagePackObject.ThrowInvalidTypeAs<double>(value);
      return 0.0;
    }

    public static explicit operator string(MessagePackObject value)
    {
      MessagePackObject.VerifyUnderlyingType<MessagePackString>(value, nameof (value));
      return value._handleOrTypeCode == null ? (string) null : (value._handleOrTypeCode as MessagePackString).GetString();
    }

    public static explicit operator byte[](MessagePackObject value)
    {
      MessagePackObject.VerifyUnderlyingType<MessagePackString>(value, nameof (value));
      return value._handleOrTypeCode == null ? (byte[]) null : (value._handleOrTypeCode as MessagePackString).GetBytes();
    }

    public static explicit operator MessagePackExtendedTypeObject(
      MessagePackObject value)
    {
      if (value.IsNil)
        MessagePackObject.ThrowCannotBeNilAs<MessagePackExtendedTypeObject>();
      MessagePackObject.VerifyUnderlyingType<MessagePackExtendedTypeObject>(value, nameof (value));
      return MessagePackExtendedTypeObject.Unpack((byte) value._value, value._handleOrTypeCode as byte[]);
    }

    [Serializable]
    private enum MessagePackValueTypeCode
    {
      Nil = 0,
      Int8 = 1,
      UInt8 = 2,
      Int16 = 3,
      UInt16 = 4,
      Int32 = 5,
      UInt32 = 6,
      Int64 = 7,
      UInt64 = 8,
      Boolean = 10, // 0x0000000A
      Single = 11, // 0x0000000B
      Double = 13, // 0x0000000D
      Object = 16, // 0x00000010
    }

    [Serializable]
    private sealed class ValueTypeCode
    {
      private readonly MessagePackObject.MessagePackValueTypeCode _typeCode;
      private readonly Type _type;

      public MessagePackObject.MessagePackValueTypeCode TypeCode
      {
        get
        {
          return this._typeCode;
        }
      }

      public bool IsSigned
      {
        get
        {
          return (int) this._typeCode % 2 != 0;
        }
      }

      public bool IsInteger
      {
        get
        {
          return this._typeCode < MessagePackObject.MessagePackValueTypeCode.Boolean;
        }
      }

      public Type Type
      {
        get
        {
          return this._type;
        }
      }

      internal ValueTypeCode(
        Type type,
        MessagePackObject.MessagePackValueTypeCode typeCode)
      {
        this._type = type;
        this._typeCode = typeCode;
      }

      public override string ToString()
      {
        return this._typeCode != MessagePackObject.MessagePackValueTypeCode.Object ? this._typeCode.ToString() : this._type.FullName;
      }
    }
  }
}
