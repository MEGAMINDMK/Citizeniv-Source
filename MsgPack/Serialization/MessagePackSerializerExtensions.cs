// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.MessagePackSerializerExtensions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.IO;

namespace MsgPack.Serialization
{
  public static class MessagePackSerializerExtensions
  {
    public static void Pack(this IMessagePackSerializer source, Stream stream, object objectTree)
    {
      source.Pack(stream, objectTree, SerializationContext.Default.CompatibilityOptions.PackerCompatibilityOptions);
    }

    public static void Pack(
      this IMessagePackSerializer source,
      Stream stream,
      object objectTree,
      PackerCompatibilityOptions packerCompatibilityOptions)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      source.PackTo(Packer.Create(stream, packerCompatibilityOptions), objectTree);
    }

    public static object Unpack(this IMessagePackSerializer source, Stream stream)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      Unpacker unpacker = Unpacker.Create(stream);
      if (!unpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      return source.UnpackFrom(unpacker);
    }
  }
}
