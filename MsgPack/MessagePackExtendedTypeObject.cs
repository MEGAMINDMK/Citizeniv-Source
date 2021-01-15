// Decompiled with JetBrains decompiler
// Type: MsgPack.MessagePackExtendedTypeObject
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Text;

namespace MsgPack
{
  public struct MessagePackExtendedTypeObject : IEquatable<MessagePackExtendedTypeObject>
  {
    private readonly byte _typeCode;
    private readonly byte[] _body;

    public byte TypeCode
    {
      get
      {
        return this._typeCode;
      }
    }

    internal byte[] Body
    {
      get
      {
        return this._body ?? Binary.Empty;
      }
    }

    public byte[] GetBody()
    {
      return this._body != null ? this._body.Clone() as byte[] : Binary.Empty;
    }

    public bool IsValid
    {
      get
      {
        return this._body != null;
      }
    }

    private MessagePackExtendedTypeObject(byte[] body, byte unpackedTypeCode)
    {
      if (body == null)
        throw new ArgumentNullException(nameof (body));
      this._typeCode = unpackedTypeCode;
      this._body = body;
    }

    public MessagePackExtendedTypeObject(byte typeCode, byte[] body)
    {
      if (typeCode > (byte) 127)
        throw new ArgumentException("A typeCode must be less than 128 because higher values are reserved for MessagePack format specification.", nameof (typeCode));
      if (body == null)
        throw new ArgumentNullException(nameof (body));
      this._typeCode = typeCode;
      this._body = body;
    }

    public static MessagePackExtendedTypeObject Unpack(
      byte typeCode,
      byte[] body)
    {
      return new MessagePackExtendedTypeObject(body, typeCode);
    }

    public override string ToString()
    {
      if (this._body == null)
        return string.Empty;
      StringBuilder buffer = new StringBuilder(7 + this._body.Length * 2);
      this.ToString(buffer, false);
      return buffer.ToString();
    }

    internal void ToString(StringBuilder buffer, bool isJson)
    {
      if (isJson)
      {
        if (this._body == null)
        {
          buffer.Append("null");
        }
        else
        {
          buffer.Append("{\"TypeCode\":").Append(this._typeCode).Append(", \"Body\":\"");
          Binary.ToHexString(this._body, buffer);
          buffer.Append("\"}");
        }
      }
      else
      {
        if (this._body == null)
          return;
        buffer.Append("[").Append(this._typeCode).Append("]");
        Binary.ToHexString(this._body, buffer);
      }
    }

    public override int GetHashCode()
    {
      if (this._body == null)
        return 0;
      int num1 = (int) this._typeCode << 24 ^ this._body.Length;
      int num2 = Math.Min(this._body.Length / 4, 8);
      for (int index = 0; index < num2; ++index)
      {
        uint num3 = (uint) this._body[index] | (uint) this._body[index + 1] << 8 | (uint) this._body[index + 2] << 16 | (uint) this._body[index + 3] << 24;
        num1 ^= (int) num3;
      }
      return num1;
    }

    public override bool Equals(object obj)
    {
      return obj is MessagePackExtendedTypeObject other && this.Equals(other);
    }

    public bool Equals(MessagePackExtendedTypeObject other)
    {
      if ((int) this._typeCode != (int) other._typeCode)
        return false;
      if (object.ReferenceEquals((object) this._body, (object) other._body))
        return true;
      if (this._body.Length != other._body.Length)
        return false;
      for (int index = 0; index < this._body.Length; ++index)
      {
        if ((int) this._body[index] != (int) other._body[index])
          return false;
      }
      return true;
    }

    public static bool operator ==(
      MessagePackExtendedTypeObject left,
      MessagePackExtendedTypeObject right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(
      MessagePackExtendedTypeObject left,
      MessagePackExtendedTypeObject right)
    {
      return !left.Equals(right);
    }
  }
}
