// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ISerializerGeneratorConfiguration
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization
{
  internal interface ISerializerGeneratorConfiguration
  {
    string OutputDirectory { get; set; }

    SerializationMethod SerializationMethod { get; set; }

    EnumSerializationMethod EnumSerializationMethod { get; set; }

    bool IsRecursive { get; set; }

    bool PreferReflectionBasedSerializer { get; set; }

    bool WithNullableSerializers { get; set; }

    void Validate();
  }
}
