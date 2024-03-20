/*
 * Motorola 68000 Decoder
 * Copyright (c) 2024 Alexander Dimitriadis
 *
 * This file is part of Motorola 68000 Decoder.
 *
 * Motorola 68000 Decoder is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or
 * (at your option) any later version.
 *
 * Motorola 68000 Decoder is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Motorola 68000 Decoder; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dimakaju.M68k.Addressing;

namespace Dimakaju.M68k.Tools
{
  internal class BitStreamReader
  {
    public BitStreamReader(Stream input, long addressOffset = 0)
    {
      Reader = new BinaryReader(input);
      AddressOffset = addressOffset;
    }

    public long AddressPosition
      => Reader.BaseStream.Position + AddressOffset;

    public long Length
      => Reader.BaseStream.Length;

    private BinaryReader Reader { get; }

    private long AddressOffset { get; }

    private long CurrentOrder { get; set; }

    private int Position { get; set; } = 8;

    private uint Data { get; set; } = 0;

    private List<Snapshot> Snapshots { get; } = new List<Snapshot>();

    public int DecodeSigned8()
    {
      int data = Read(8);
      if ((data & 0b10000000) != 0)
        return -((data ^ 0b11111111) + 1);
      else
        return data;
    }

    public int DecodeSigned16()
    {
      int data = Read(16);
      if ((data & 0b10000000_00000000) != 0)
        return -((data ^ 0b11111111_11111111) + 1);
      else
        return data;
    }

    public int DecodeSigned32()
    {
      int data = Read(32);
      if ((data & 0b10000000_00000000_00000000_00000000) != 0)
        return (int)-((data ^ 0b11111111_11111111_11111111_11111111) + 1);
      else
        return data;
    }

    public ImmediateData? Immediate8(BitStreamReader reader, bool gap)
    {
      if (gap == true && reader.Read(8) != 0b00000000)
        return null;

      return new ImmediateData(reader.Read(8));
    }

    public ImmediateData? Immediate16(BitStreamReader reader)
      => new ImmediateData(reader.Read(16));

    public ImmediateData? Immediate32(BitStreamReader reader)
      => new ImmediateData(reader.Read(32));

    public int Read(int bits)
    {
      if (bits == 0)
        return 0;

      if (bits < 0)
        throw new ArgumentException($"Must not be negative", nameof(bits));

      int data = 0;
      for (int i = 0; i < bits; i++)
      {
        data |= ReadBit() ? 1 : 0;
        if (i < bits - 1)
          data = data << 1;
      }

      return data;
    }

    public Snapshot TakeSnapshot()
    {
      var snapshot = new Snapshot(Position, Data, Reader.BaseStream.Position, CurrentOrder++);
      Snapshots.Add(snapshot);
      return snapshot;
    }

    public void RevertToSnapshot()
    {
      Snapshot? snapshot = Snapshots.OrderBy(x => x.Order).LastOrDefault();

      if (snapshot == null)
        throw new InvalidOperationException($"No more snapshots to revert.");
      else
        Snapshots.Remove(snapshot);

      Position = snapshot.Position;
      Data = snapshot.Data;
      Reader.BaseStream.Seek(snapshot.StreamPosition, SeekOrigin.Begin);
    }

    public void RemoveSnapshot()
    {
      Snapshot? snapshot = Snapshots.OrderBy(x => x.Order).LastOrDefault();

      if (snapshot == null)
        throw new InvalidOperationException($"No more snapshots to remove.");
      else
        Snapshots.Remove(snapshot);
    }

    public bool IsFinished()
      => Reader.BaseStream.Length == Reader.BaseStream.Position;

    private bool ReadBit()
    {
      if (Position == 8)
      {
        Position = 0;
        Data = Reader.ReadByte();
      }

      var bit = (Data & 1 << (7 - Position++)) == 0 ? false : true;
      Snapshots.OrderBy(x => x.Order).LastOrDefault()?.AddBit(bit);
      return bit;
    }

    public class Snapshot
    {
      public Snapshot(int position, uint data, long streamPosition, long order)
      {
        Position = position;
        Data = data;
        StreamPosition = streamPosition;
        Order = order;
      }

      public long Order { get; }

      public int Position { get; }

      public uint Data { get; }

      public long StreamPosition { get; }

      public IReadOnlyList<byte> ReadData { get; } = new List<byte>(8);

      private int DataBitsPtr { get; set; } = 7;

      private byte DataBits { get; set; }

      public void AddBit(bool bit)
      {
        DataBits = (byte)(DataBits | (bit ? 1 : 0) << DataBitsPtr--);

        if (DataBitsPtr < 0)
        {
          ((List<byte>)ReadData).Add(DataBits);
          DataBits = 0;
          DataBitsPtr = 7;
        }
      }
    }
  }
}
