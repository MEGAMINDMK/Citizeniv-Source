// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Polymorphic.TypeInfoEncoder
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.Polymorphic
{
  internal static class TypeInfoEncoder
  {
    private const string Elipsis = ".";

    public static void Encode(Packer packer, string typeCode)
    {
      packer.PackArrayHeader(2);
      packer.PackString(typeCode);
    }

    public static void Encode(Packer packer, Type type)
    {
      AssemblyName name = type.GetAssembly().GetName();
      packer.PackArrayHeader(2);
      packer.PackArrayHeader(6);
      packer.Pack((byte) 1);
      string str = type.Namespace.StartsWith(name.Name, StringComparison.Ordinal) ? "." + type.FullName.Substring(name.Name.Length + 1) : type.FullName;
      byte[] numArray = new byte[16];
      Buffer.BlockCopy((Array) BitConverter.GetBytes(name.Version.Major), 0, (Array) numArray, 0, 4);
      Buffer.BlockCopy((Array) BitConverter.GetBytes(name.Version.Minor), 0, (Array) numArray, 4, 4);
      Buffer.BlockCopy((Array) BitConverter.GetBytes(name.Version.Build), 0, (Array) numArray, 8, 4);
      Buffer.BlockCopy((Array) BitConverter.GetBytes(name.Version.Revision), 0, (Array) numArray, 12, 4);
      packer.PackString(str).PackString(name.Name).PackBinary(numArray).PackString(name.GetCultureName()).PackBinary(name.GetPublicKeyToken());
    }

    public static T Decode<T>(
      Unpacker unpacker,
      Func<Unpacker, Type> typeFinder,
      Func<Type, Unpacker, T> unpacking)
    {
      if (!unpacker.IsArrayHeader || UnpackHelpers.GetItemsCount(unpacker) != 2)
        throw SerializationExceptions.NewUnknownTypeEmbedding();
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        if (!unpacker1.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        Type type = typeFinder(unpacker1);
        if (!unpacker1.Read())
          throw SerializationExceptions.NewUnexpectedEndOfStream();
        return unpacking(type, unpacker1);
      }
    }

    public static Type DecodeRuntimeTypeInfo(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw new SerializationException("Type info must be non-nil array.");
      if (unpacker.ItemsCount != 6L)
        throw new SerializationException("Components count of type info is not valid.");
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        byte result1;
        if (!unpacker1.ReadByte(out result1))
          throw new SerializationException("Failed to decode encode type component.");
        if (result1 != (byte) 1)
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown encoded type : {0}", (object) result1));
        string result2;
        if (!unpacker1.ReadString(out result2))
          throw new SerializationException("Failed to decode type name component.");
        string result3;
        if (!unpacker1.ReadString(out result3))
          throw new SerializationException("Failed to decode assembly name component.");
        byte[] result4;
        if (!unpacker1.ReadBinary(out result4))
          throw new SerializationException("Failed to decode version component.");
        string result5;
        if (!unpacker1.ReadString(out result5))
          throw new SerializationException("Failed to decode culture component.");
        byte[] result6;
        if (!unpacker1.ReadBinary(out result6))
          throw new SerializationException("Failed to decode public key token component.");
        AssemblyName assemblyRef = new AssemblyName()
        {
          Name = result3,
          Version = new Version(BitConverter.ToInt32(result4, 0), BitConverter.ToInt32(result4, 4), BitConverter.ToInt32(result4, 8), BitConverter.ToInt32(result4, 12)),
          CultureInfo = string.IsNullOrEmpty(result5) ? (CultureInfo) null : CultureInfo.GetCultureInfo(result5)
        };
        assemblyRef.SetPublicKeyToken(result6);
        return Assembly.Load(assemblyRef).GetType(result2.StartsWith(".", StringComparison.Ordinal) ? result3 + result2 : result2, true);
      }
    }
  }
}
