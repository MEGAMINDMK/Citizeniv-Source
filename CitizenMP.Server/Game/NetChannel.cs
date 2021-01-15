// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Game.NetChannel
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Collections;
using System.IO;

namespace CitizenMP.Server.Game
{
  public class NetChannel
  {
    private Client m_client;
    private const int FRAGMENT_SIZE = 1300;
    private uint m_fragmentSequence;
    private int m_fragmentLength;
    private byte[] m_fragmentBuffer;
    private BitArray m_fragmentValidSet;
    private int m_fragmentLastBit;
    private uint m_inSequence;
    private uint m_outSequence;

    public NetChannel(Client client)
    {
      this.m_client = client;
      this.m_fragmentSequence = uint.MaxValue;
    }

    public bool Process(byte[] buffer, int length, ref BinaryReader reader)
    {
      uint uint32 = BitConverter.ToUInt32(buffer, 0);
      bool flag = (uint32 & 2147483648U) > 0U;
      int num1 = 0;
      int num2 = 0;
      if (flag)
      {
        num1 = (int) BitConverter.ToInt16(buffer, 4);
        num2 = (int) BitConverter.ToInt16(buffer, 6);
        uint32 &= (uint) int.MaxValue;
      }
      if (uint32 <= this.m_inSequence && this.m_inSequence != 0U)
      {
        this.Log<NetChannel>(nameof (Process), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\NetChannel.cs", 49).Debug("out-of-order packet");
        return false;
      }
      if (uint32 > this.m_inSequence + 1U)
        this.Log<NetChannel>(nameof (Process), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\NetChannel.cs", 55).Debug("dropped packet");
      if (flag)
      {
        if ((int) uint32 != (int) this.m_fragmentSequence)
        {
          this.m_fragmentLength = 0;
          this.m_fragmentSequence = uint32;
          this.m_fragmentBuffer = new byte[65536];
          this.m_fragmentValidSet = new BitArray(50);
          this.m_fragmentLastBit = -1;
        }
        int index1 = num1 / 1300;
        if (index1 > 49 || this.m_fragmentValidSet.Get(index1))
          return false;
        this.m_fragmentValidSet.Set(index1, true);
        Array.Copy((Array) buffer, 8, (Array) this.m_fragmentBuffer, index1 * 1300, length - 8);
        this.m_fragmentLength += length - 8;
        if (num2 != 1300)
          this.m_fragmentLastBit = index1;
        if (this.m_fragmentLastBit == -1)
          return false;
        for (int index2 = 0; index2 <= this.m_fragmentLastBit; ++index2)
        {
          if (!this.m_fragmentValidSet.Get(index2))
            return false;
        }
        this.m_inSequence = uint32;
        reader = new BinaryReader((Stream) new MemoryStream(this.m_fragmentBuffer, 0, this.m_fragmentLength));
        this.m_fragmentLength = 0;
        return true;
      }
      this.m_inSequence = uint32;
      return true;
    }

    public void Send(byte[] buffer)
    {
      if (buffer.Length > 1300)
      {
        this.SendFragmented(buffer);
      }
      else
      {
        byte[] buffer1 = new byte[buffer.Length + 4];
        Array.Copy((Array) BitConverter.GetBytes(this.m_outSequence), (Array) buffer1, 4);
        Array.Copy((Array) buffer, 0, (Array) buffer1, 4, buffer.Length);
        this.m_client.SendRaw(buffer1);
        ++this.m_outSequence;
      }
    }

    private void SendFragmented(byte[] buffer)
    {
      uint num = this.m_outSequence | 2147483648U;
      int length1 = buffer.Length;
      int sourceIndex = 0;
      while (length1 >= 0)
      {
        int length2 = length1;
        if (length2 > 1300)
          length2 = 1300;
        byte[] buffer1 = new byte[length2 + 8];
        Array.Copy((Array) BitConverter.GetBytes(num), (Array) buffer1, 4);
        Array.Copy((Array) BitConverter.GetBytes((ushort) sourceIndex), 0, (Array) buffer1, 4, 2);
        Array.Copy((Array) BitConverter.GetBytes((ushort) length2), 0, (Array) buffer1, 6, 2);
        Array.Copy((Array) buffer, sourceIndex, (Array) buffer1, 8, length2);
        this.m_client.SendRaw(buffer1);
        length1 -= length2;
        sourceIndex += length2;
        if (length1 == 0 && length2 != 1300)
          break;
      }
      ++this.m_outSequence;
    }
  }
}
