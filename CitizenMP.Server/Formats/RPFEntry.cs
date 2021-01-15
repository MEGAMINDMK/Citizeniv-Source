// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Formats.RPFEntry
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CitizenMP.Server.Formats
{
  public class RPFEntry
  {
    private List<RPFEntry> m_subEntries;

    public string Name { get; private set; }

    public bool IsDirectory { get; private set; }

    public byte[] FileData { get; private set; }

    public bool IsResource { get; private set; }

    public uint ResourceFlags { get; private set; }

    public byte ResourceVersion { get; set; }

    public RPFEntry()
    {
      this.m_subEntries = new List<RPFEntry>();
    }

    public RPFEntry(string name, byte[] data)
      : this()
    {
      this.Name = name;
      this.FileData = data;
      if (data.Length < 4 || data[0] != (byte) 82 || (data[1] != (byte) 83 || data[2] != (byte) 67))
        return;
      this.IsResource = true;
      this.ResourceVersion = data[4];
      this.ResourceFlags = BitConverter.ToUInt32(data, 8);
    }

    public RPFEntry(string name, bool isDirectory)
      : this()
    {
      this.Name = name;
      this.IsDirectory = isDirectory;
    }

    public void AddFile(string name, byte[] data)
    {
      Queue<string> queue = new Queue<string>((IEnumerable<string>) name.Split(new string[1]
      {
        "/"
      }, StringSplitOptions.RemoveEmptyEntries));
      if (queue.Count == 1)
        this.m_subEntries.Add(new RPFEntry(name, data));
      else
        this.FindDirectory(queue, "").m_subEntries.Add(new RPFEntry(name, data));
    }

    private RPFEntry FindDirectory(Queue<string> queue, string basePath)
    {
      string path = queue.Dequeue();
      RPFEntry rpfEntry = this.m_subEntries.Find((Predicate<RPFEntry>) (e => e.Name == basePath + path));
      if (rpfEntry != null)
      {
        if (!rpfEntry.IsDirectory)
          throw new InvalidOperationException("This already is a file, so a directory can't be made.");
      }
      else
      {
        rpfEntry = new RPFEntry(basePath + path, true);
        this.m_subEntries.Add(rpfEntry);
      }
      if (queue.Count > 1)
        rpfEntry = rpfEntry.FindDirectory(queue, basePath + path + "/");
      return rpfEntry;
    }

    internal void Write(MarkedBinaryWriter writer)
    {
      writer.Mark("nOff_" + this.Name);
      writer.Write(0);
      if (!this.IsDirectory)
      {
        writer.Write(this.FileData.Length);
        writer.Mark("fOff_" + this.Name);
        writer.Write(0);
        if (!this.IsResource)
          writer.Write(this.FileData.Length);
        else
          writer.Write(this.ResourceFlags);
      }
      else
      {
        writer.Write(this.m_subEntries.Count);
        writer.Mark("cIdx_" + this.Name);
        writer.Write(0);
        writer.Write(this.m_subEntries.Count);
      }
      ++writer.WriteIdx;
    }

    internal void WriteSubEntries(MarkedBinaryWriter writer)
    {
      if (this.IsDirectory)
        writer.WriteMark("cIdx_" + this.Name, (uint) (writer.WriteIdx | int.MinValue));
      IOrderedEnumerable<RPFEntry> orderedEnumerable = this.m_subEntries.OrderBy<RPFEntry, string>((Func<RPFEntry, string>) (e => e.Name));
      foreach (RPFEntry rpfEntry in (IEnumerable<RPFEntry>) orderedEnumerable)
        rpfEntry.Write(writer);
      foreach (RPFEntry rpfEntry in (IEnumerable<RPFEntry>) orderedEnumerable)
        rpfEntry.WriteSubEntries(writer);
    }

    internal void WriteNames(MarkedBinaryWriter writer)
    {
      if (this.Name != "")
      {
        writer.WriteMark("nOff_" + this.Name, (uint) writer.BaseStream.Position - (uint) writer.BaseNamePosition);
        writer.Write(Encoding.ASCII.GetBytes(Path.GetFileName(this.Name)));
        writer.Write((byte) 0);
      }
      else
      {
        writer.BaseNamePosition = (int) writer.BaseStream.Position;
        writer.WriteMark("nOff_", 0U);
        writer.Write(new byte[2]{ (byte) 47, (byte) 0 });
      }
      foreach (RPFEntry rpfEntry in (IEnumerable<RPFEntry>) this.m_subEntries.OrderBy<RPFEntry, string>((Func<RPFEntry, string>) (e => e.Name)))
        rpfEntry.WriteNames(writer);
    }

    internal void WriteFiles(MarkedBinaryWriter writer)
    {
      if (!this.IsDirectory)
      {
        writer.WriteMark("fOff_" + this.Name, (uint) writer.BaseStream.Position & 4294967040U | (uint) this.ResourceVersion);
        writer.Write(this.FileData);
        writer.Align(2048);
      }
      foreach (RPFEntry subEntry in this.m_subEntries)
        subEntry.WriteFiles(writer);
    }
  }
}
