// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.IMessagePackSerializer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization
{
  public interface IMessagePackSerializer
  {
    void PackTo(Packer packer, object objectTree);

    object UnpackFrom(Unpacker unpacker);

    void UnpackTo(Unpacker unpacker, object collection);
  }
}
