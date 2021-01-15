// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Formats.MarkedBinaryWriter
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Collections.Generic;
using System.IO;

namespace CitizenMP.Server.Formats
{
  internal class MarkedBinaryWriter : BinaryWriter
  {
    private Dictionary<string, long> m_markOffsets;

    public MarkedBinaryWriter(Stream stream)
      : base(stream)
    {
      this.m_markOffsets = new Dictionary<string, long>();
      if (!stream.CanSeek)
        throw new ArgumentException("This stream does not support seeking.");
    }

    public int WriteIdx { get; set; }

    public int BaseNamePosition { get; set; }

    public void Mark(string markName)
    {
      this.m_markOffsets[markName] = this.BaseStream.Position;
    }

    public void WriteMark(string markName, uint value)
    {
      long position = this.BaseStream.Position;
      this.BaseStream.Position = this.m_markOffsets[markName];
      this.Write(value);
      this.BaseStream.Position = position;
      this.m_markOffsets.Remove(markName);
    }

    public void Align(int alignment)
    {
      long position = this.BaseStream.Position;
      this.Write(new byte[position % (long) alignment == 0L ? IntPtr.Zero : checked ((IntPtr) unchecked ((long) alignment - position % (long) alignment))]);
    }

    public override void Close()
    {
      if (this.m_markOffsets.Count > 0)
        throw new InvalidOperationException("Can't close when there's open marks...");
      base.Close();
    }
  }
}
