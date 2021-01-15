// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializerCodeGenerationResult
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization
{
  public sealed class SerializerCodeGenerationResult
  {
    public string FilePath { get; private set; }

    public Type TargetType { get; private set; }

    public string SerializerTypeNamespace { get; private set; }

    public string SerializerTypeName { get; private set; }

    public string SerializerTypeFullName { get; private set; }

    internal SerializerCodeGenerationResult(
      Type targetType,
      string filePath,
      string serializerTypeFullName,
      string serializerTypeNamespace,
      string serializerTypeName)
    {
      this.TargetType = targetType;
      this.FilePath = filePath;
      this.SerializerTypeFullName = serializerTypeFullName;
      this.SerializerTypeNamespace = serializerTypeNamespace;
      this.SerializerTypeName = serializerTypeName;
    }
  }
}
