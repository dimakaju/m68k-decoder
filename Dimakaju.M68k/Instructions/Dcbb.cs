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
using System.IO;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Instructions
{
  public class Dcbb : DecodedData
  {
    internal Dcbb(BinaryReader reader)
      : base(reader)
    {
      Item = reader.ReadByte();
    }

    internal Dcbb(byte[] data, byte item)
    {
      Data = new byte[data.Length];
      Array.Copy(data, Data, data.Length);
      Item = item;
    }

    public override byte[] Data { get; internal set; } = Array.Empty<byte>();

    public override int Length
      => Data.Length;

    public byte Item { get; }

    public DataInfo Type => DataInfo.Dcbb;

    internal override ushort Identifier => Type.Id;

    public override string RenderMnemonic(RenderingSettings settings)
      => settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Type.Text.ToLowerInvariant() : Type.Text.ToUpperInvariant();

    public override string RenderOperands(RenderingSettings settings)
    {
      var result = $"{Formatting.Data32((uint)Length, settings)},{Formatting.Data8(Item, settings)}";
      return settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? result.ToLowerInvariant() : result.ToUpperInvariant();
    }

    protected override void Serialize(BinaryWriter writer)
      => writer.Write(Item);

    internal class Entry
    {
      internal long Address { get; set; }
      internal int Count { get; set; }
      internal byte Item { get; set; }
    }
  }
}
