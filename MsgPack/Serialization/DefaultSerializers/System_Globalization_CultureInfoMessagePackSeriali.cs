// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.System_Globalization_CultureInfoMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Globalization;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class System_Globalization_CultureInfoMessagePackSerializer : MessagePackSerializer<CultureInfo>
  {
    public System_Globalization_CultureInfoMessagePackSerializer(SerializationContext ownerContext)
      : base(ownerContext)
    {
    }

    protected internal override void PackToCore(Packer packer, CultureInfo objectTree)
    {
      packer.PackString(objectTree.Name);
    }

    protected internal override CultureInfo UnpackFromCore(Unpacker unpacker)
    {
      return CultureInfo.GetCultureInfo(unpacker.LastReadData.AsString());
    }
  }
}
