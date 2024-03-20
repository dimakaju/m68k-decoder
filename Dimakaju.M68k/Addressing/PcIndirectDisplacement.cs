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
  public class PcIndirectDisplacement : ReferenceOperand
  {
    internal PcIndirectDisplacement(BinaryReader reader, IReadOnlyDictionary<long, AddressLabel> labels)
      : base(reader)
    {
      Displacement = reader.ReadInt32();
      if (reader.ReadByte() == 0xFF)
        Label = labels[reader.ReadInt64()];
    }

    internal PcIndirectDisplacement(int displacement)
      => Displacement = displacement + 2;

    public int Displacement { get; }

    public override OperandInfo Type => OperandInfo.PcIndirectDisplacement;

    internal static bool Match(int eamode, int earegister)
      => eamode == 0b111 && earegister == 0b010;

    internal override string Render(RenderingSettings settings)
    {
      string result;
      if (Label != null && Label.IsOutOfRange == false)
        return $"{Label.DisplayLabel(settings)}({Registers.PC})";

      result = $"{Formatting.DisplacementPC(Displacement, settings)}({Registers.PC})";
      if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
        return result.ToLowerInvariant();
      else
        return result.ToUpperInvariant();
    }

    internal override AddressLabel GenerateLabel(long address)
    {
      var labeladdress = address + Displacement;
      return new AddressLabel(labeladdress, $"lbl{Formatting.Hex((uint)labeladdress)}");
    }

    internal override void Serialize(BinaryWriter writer)
    {
      base.Serialize(writer);
      writer.Write(Displacement);
      writer.Write(Label == null ? (byte)0x00 : (byte)0xFF);
      if (Label != null)
        writer.Write(Label.Address);
    }
  }
}
