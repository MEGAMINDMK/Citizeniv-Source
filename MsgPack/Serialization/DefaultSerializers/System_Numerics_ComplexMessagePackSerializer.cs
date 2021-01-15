// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Numerics_ComplexMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Numerics;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Numerics_ComplexMessagePackSerializer : MessagePackSerializer<Complex>
  {
    public System_Numerics_ComplexMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, Complex objectTree)
    {
      packer.PackArrayHeader(2);
      packer.Pack(objectTree.Real);
      packer.Pack(objectTree.Imaginary);
    }

    protected internal override Complex UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      double real = unpacker.LastReadData.AsDouble();
      if (!unpacker.Read())
        throw SerializationExceptions.NewUnexpectedEndOfStream();
      double imaginary = unpacker.LastReadData.AsDouble();
      return new Complex(real, imaginary);
    }
  }
}
