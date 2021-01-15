// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Formats.RPFFile
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System.IO;

namespace CitizenMP.Server.Formats
{
  public class RPFFile
  {
    private RPFEntry RootEntry { get; set; }

    public RPFFile()
    {
      this.RootEntry = new RPFEntry("", true);
    }

    public void AddFile(string name, byte[] data)
    {
      this.RootEntry.AddFile(name, data);
    }

    public void Write(string path)
    {
      FileStream fileStream = File.Open(path, FileMode.Create);
      MarkedBinaryWriter writer = new MarkedBinaryWriter((Stream) fileStream);
      writer.Write(843468882);
      writer.Mark("tocSize");
      writer.Write(0);
      writer.Mark("numEntries");
      writer.Write(0);
      writer.Write(0);
      writer.Write(0);
      writer.Align(2048);
      this.RootEntry.Write(writer);
      this.RootEntry.WriteSubEntries(writer);
      this.RootEntry.WriteNames(writer);
      writer.WriteMark("numEntries", (uint) writer.WriteIdx);
      writer.Align(2048);
      writer.WriteMark("tocSize", (uint) fileStream.Position - 2048U);
      this.RootEntry.WriteFiles(writer);
      writer.Close();
    }
  }
}
