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

using System.Collections.Generic;
using System.IO;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Addressing
{
  public class Label : ReferenceOperand
  {
    internal Label(BinaryReader reader, IReadOnlyDictionary<long, AddressLabel> labels)
      : base(reader)
    {
      Position = reader.ReadInt32();
      if (reader.ReadByte() == 0xFF)
        Label = labels[reader.ReadInt64()];
    }

    internal Label(int position)
      => Position = position + 2;

    public int Position { get; }

    public override OperandInfo Type => OperandInfo.Label;

    internal override string Render(RenderingSettings settings)
    {
      string result;
      if (Label != null && Label.IsOutOfRange == false)
        return $"{Label.DisplayLabel(settings)}";

      result = Formatting.Label(Position, settings);
      if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
        return result.ToLowerInvariant();
      else
        return result.ToUpperInvariant();
    }

    internal override AddressLabel GenerateLabel(long address)
    {
      var labeladdress = address + Position;
      return new AddressLabel(labeladdress, $"lbl{Formatting.Hex((uint)labeladdress)}");
    }

    internal override void Serialize(BinaryWriter writer)
    {
      base.Serialize(writer);
      writer.Write(Position);
      writer.Write(Label == null ? (byte)0x00 : (byte)0xFF);
      if (Label != null)
        writer.Write(Label.Address);
    }
  }
}
