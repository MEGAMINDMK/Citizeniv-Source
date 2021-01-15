// Decompiled with JetBrains decompiler
// Type: MsgPack.MessagePackString
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;

namespace MsgPack
{
  [SecuritySafeCritical]
  [DebuggerDisplay("{DebuggerDisplayString}")]
  [DebuggerTypeProxy(typeof (MessagePackString.MessagePackStringDebuggerProxy))]
  [Serializable]
  internal sealed class MessagePackString
  {
    private static readonly DecoderFallbackException IsBinary = new DecoderFallbackException("This value is not string.");
    private byte[] _encoded;
    private string _decoded;
    private DecoderFallbackException _decodingError;
    private MessagePackString.BinaryType _type;
    private static int _isFastEqualsDisabled;

    private string DebuggerDisplayString
    {
      get
      {
        return new MessagePackString.MessagePackStringDebuggerProxy(this).Value;
      }
    }

    public MessagePackString(string decoded)
    {
      this._decoded = decoded;
      this._type = MessagePackString.BinaryType.String;
    }

    public MessagePackString(byte[] encoded, bool isBinary)
    {
      this._encoded = encoded;
      this._type = isBinary ? MessagePackString.BinaryType.Blob : MessagePackString.BinaryType.Unknwon;
      if (!isBinary)
        return;
      this._decodingError = MessagePackString.IsBinary;
    }

    private MessagePackString(MessagePackString other)
    {
      this._encoded = other._encoded;
      this._decoded = other._decoded;
      this._decodingError = other._decodingError;
      this._type = other._type;
    }

    private void EncodeIfNeeded()
    {
      if (this._encoded != null || this._decoded == null)
        return;
      this._encoded = MessagePackConvert.Utf8NonBom.GetBytes(this._decoded);
    }

    private void DecodeIfNeeded()
    {
      if (this._decoded != null || this._encoded == null)
        return;
      if (this._type != MessagePackString.BinaryType.Unknwon)
        return;
      try
      {
        this._decoded = MessagePackConvert.DecodeStringStrict(this._encoded);
        this._type = MessagePackString.BinaryType.String;
      }
      catch (DecoderFallbackException ex)
      {
        this._decodingError = ex;
        this._type = MessagePackString.BinaryType.Blob;
      }
    }

    public string TryGetString()
    {
      this.DecodeIfNeeded();
      return this._decoded;
    }

    public string GetString()
    {
      this.DecodeIfNeeded();
      if (this._decodingError != null)
        throw new InvalidOperationException("This bytes is not UTF-8 string.", this._decodingError == MessagePackString.IsBinary ? (Exception) null : (Exception) this._decodingError);
      return this._decoded;
    }

    public byte[] UnsafeGetBuffer()
    {
      return this._encoded;
    }

    public string UnsafeGetString()
    {
      return this._decoded;
    }

    public byte[] GetBytes()
    {
      this.EncodeIfNeeded();
      return this._encoded;
    }

    public Type GetUnderlyingType()
    {
      this.DecodeIfNeeded();
      return this._type != MessagePackString.BinaryType.String ? typeof (byte[]) : typeof (string);
    }

    public override string ToString()
    {
      if (this._decoded != null)
        return this._decoded;
      return this._encoded != null ? Binary.ToHexString(this._encoded) : string.Empty;
    }

    public override int GetHashCode()
    {
      if (this._decoded != null)
        return this._decoded.GetHashCode();
      this.DecodeIfNeeded();
      if (this._decoded != null)
        return this._decoded.GetHashCode();
      if (this._encoded == null)
        return 0;
      int num1 = 0;
      for (int index = 0; index < this._encoded.Length; ++index)
      {
        int num2 = (int) this._encoded[index] << index % 4 * 8;
        num1 ^= num2;
      }
      return num1;
    }

    public override bool Equals(object obj)
    {
      MessagePackString right = obj as MessagePackString;
      if (object.ReferenceEquals((object) right, (object) null))
        return false;
      if (this._decoded != null && right._decoded != null)
        return this._decoded == right._decoded;
      if (this._decoded == null && right._decoded == null)
        return MessagePackString.EqualsEncoded(this, right);
      this.DecodeIfNeeded();
      right.DecodeIfNeeded();
      return this._decoded == right._decoded;
    }

    private static bool EqualsEncoded(MessagePackString left, MessagePackString right)
    {
      if (left._encoded == null)
        return right._encoded == null;
      if (left._encoded.Length == 0)
        return right._encoded.Length == 0;
      if (left._encoded.Length != right._encoded.Length)
        return false;
      if (MessagePackString._isFastEqualsDisabled == 0)
      {
        try
        {
          return MessagePackString.UnsafeFastEquals(left._encoded, right._encoded);
        }
        catch (SecurityException ex)
        {
          Interlocked.Exchange(ref MessagePackString._isFastEqualsDisabled, 1);
        }
        catch (MemberAccessException ex)
        {
          Interlocked.Exchange(ref MessagePackString._isFastEqualsDisabled, 1);
        }
      }
      return MessagePackString.SlowEquals(left._encoded, right._encoded);
    }

    private static bool SlowEquals(byte[] x, byte[] y)
    {
      for (int index = 0; index < x.Length; ++index)
      {
        if ((int) x[index] != (int) y[index])
          return false;
      }
      return true;
    }

    [SecuritySafeCritical]
    private static bool UnsafeFastEquals(byte[] x, byte[] y)
    {
      int result;
      if (UnsafeNativeMethods.TryMemCmp(x, y, new UIntPtr((uint) x.Length), out result))
        return result == 0;
      Interlocked.Exchange(ref MessagePackString._isFastEqualsDisabled, 1);
      return MessagePackString.SlowEquals(x, y);
    }

    [Serializable]
    private enum BinaryType
    {
      Unknwon,
      String,
      Blob,
    }

    internal sealed class MessagePackStringDebuggerProxy
    {
      private readonly MessagePackString _target;
      private string _asByteArray;

      public MessagePackStringDebuggerProxy(MessagePackString target)
      {
        this._target = new MessagePackString(target);
      }

      public string Value
      {
        get
        {
          string str = Interlocked.CompareExchange<string>(ref this._asByteArray, (string) null, (string) null);
          if (str != null)
            return str;
          switch (this._target._type)
          {
            case MessagePackString.BinaryType.String:
              return this.AsString ?? this.AsByteArray;
            case MessagePackString.BinaryType.Blob:
              return this.AsByteArray;
            default:
              this._target.DecodeIfNeeded();
              goto case MessagePackString.BinaryType.String;
          }
        }
      }

      public string AsString
      {
        get
        {
          string str = this._target.TryGetString();
          if (str == null)
            return (string) null;
          if (MessagePackString.MessagePackStringDebuggerProxy.MustBeString(str))
            return str;
          this.CreateByteArrayString();
          return (string) null;
        }
      }

      private static bool MustBeString(string value)
      {
        for (int index = 0; index < 128 && index < value.Length; ++index)
        {
          char ch = value[index];
          if (ch < ' ' && ch != '\t' && (ch != '\n' && ch != '\r') || '~' < ch && ch < ' ')
            return false;
        }
        return true;
      }

      public string AsByteArray
      {
        get
        {
          return Interlocked.CompareExchange<string>(ref this._asByteArray, (string) null, (string) null) ?? this.CreateByteArrayString();
        }
      }

      private string CreateByteArrayString()
      {
        byte[] bytes = this._target.GetBytes();
        StringBuilder stringBuilder = new StringBuilder((bytes.Length <= 128 ? bytes.Length * 3 : 387) + 4);
        stringBuilder.Append('[');
        foreach (byte num in ((IEnumerable<byte>) bytes).Take<byte>(128))
        {
          stringBuilder.Append(' ');
          stringBuilder.Append(num.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
        }
        stringBuilder.Append(" ]");
        string str = stringBuilder.ToString();
        Interlocked.Exchange<string>(ref this._asByteArray, str);
        return str;
      }
    }
  }
}
