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
using System.Text;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k
{
  public class AddressLabel
  {
    internal AddressLabel(BinaryReader reader)
    {
      var addresses = (List<long>)InstructionAddresses;
      Address = reader.ReadInt64();
      Name = reader.ReadString();

      var count = reader.ReadInt32();
      for (int i = 0; i < count; i++)
        addresses.Add(reader.ReadInt64());

      IsOutOfRange = reader.ReadBoolean();

      if (reader.ReadByte() == 0xFF)
        DisplacementLabel = new AddressLabel(reader);
    }

    internal AddressLabel(long address, string name)
    {
      Address = address;
      Name = name;
    }

    public long Address { get; }

    public string Name { get; internal set; }

    public IReadOnlyList<long> InstructionAddresses { get; } = new List<long>();

    public bool IsOutOfRange { get; internal set; }

    public AddressLabel? DisplacementLabel { get; internal set; }

    public string DisplayLabel(RenderingSettings settings)
    {
      if (DisplacementLabel == null)
      {
        return $"{Name}";
      }
      else
      {
        var displacement = Address - DisplacementLabel.Address;
        if (displacement < 0 && settings.UseHex == true)
          return $"{DisplacementLabel.Name}-{Formatting.Hex(Math.Abs(displacement))}";
        else
          return $"{DisplacementLabel.Name}+{Formatting.Hex(displacement)}";
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append($"{Formatting.Hex(Address):X8} {Name} [{InstructionAddresses.Count}]");
      return sb.ToString();
    }

    internal void AddInstructionAddress(long address)
    {
      var list = (List<long>)InstructionAddresses;
      if (list.Any(x => x == address) == false)
        list.Add(address);
    }

    internal void RemoveInstructionAddressRange(long start, long end)
    {
      var list = (List<long>)InstructionAddresses;
      list.RemoveAll(x => x >= start && x < end);
    }

    internal byte[] Serialize()
    {
      using (var ms = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
          Serialize(writer);
          return ms.ToArray();
        }
      }
    }

    internal void Serialize(BinaryWriter writer)
    {
      writer.Write(Address);
      writer.Write(Name);
      writer.Write(InstructionAddresses.Count);
      for (int i = 0; i < InstructionAddresses.Count; i++)
        writer.Write(InstructionAddresses[i]);
      writer.Write(IsOutOfRange);
      writer.Write(DisplacementLabel == null ? (byte)0x00 : (byte)0xFF);
      DisplacementLabel?.Serialize(writer);
    }
  }
}
