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
using System.Text;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Addressing
{
  public class PcIndirectIndexDisplacement : ReferenceOperand
  {
    internal PcIndirectIndexDisplacement(BinaryReader reader, IReadOnlyDictionary<long, AddressLabel> labels)
      : base(reader)
    {
      Displacement = reader.ReadInt32();
      IndexregisterType = (RegisterTypes)reader.ReadInt32();
      Indexregister = reader.ReadInt32();
      Size = (Attributes.Sizes)reader.ReadInt32();
      if (reader.ReadByte() == 0xFF)
        Label = labels[reader.ReadInt64()];
    }

    internal PcIndirectIndexDisplacement(int ixmask, int displacement)
    {
      Displacement = displacement + 2;

      ixmask = ixmask & 0b11111111;

      int ixtype = (ixmask & 0b10000000) >> 7;
      IndexregisterType = ixtype == 0 ? RegisterTypes.Dn : RegisterTypes.An;

      int ixsize = (ixmask & 0b00001000) >> 3;
      Size = ixsize == 0 ? Attributes.Sizes.Word : Attributes.Sizes.Long;

      Indexregister = (ixmask & 0b01110000) >> 4;
    }

    public int Displacement { get; }

    public RegisterTypes IndexregisterType { get; }

    public int Indexregister { get; }

    public Attributes.Sizes Size { get; }

    public override OperandInfo Type => OperandInfo.PcIndirectIndexDisplacement;

    internal static bool Match(int eamode, int earegister)
      => eamode == 0b111 && earegister == 0b011;

    internal override AddressLabel GenerateLabel(long address)
    {
      var labeladdress = address + Displacement;
      return new AddressLabel(labeladdress, $"lbl{Formatting.Hex((uint)labeladdress)}");
    }

    internal override string Render(RenderingSettings settings)
    {
      var sb = new StringBuilder(16);

      if (Label != null && Label.IsOutOfRange == false)
      {
        sb.Append($"{Label.DisplayLabel(settings)}");
        sb.Append($"({Registers.PC},");
        sb.Append($"{(IndexregisterType == RegisterTypes.Dn ? $"{Registers.Dn[Indexregister]}" : $"{Registers.An[Indexregister]}")}");
        sb.Append($"{(Size == Attributes.Sizes.Word ? $".{Attributes.Size.Word}" : $".{Attributes.Size.Long}")})");

        if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
          return sb.ToString().ToLowerInvariant();
        else
          return sb.ToString().ToUpperInvariant();
      }
      else
      {
        sb.Append($"{Formatting.DisplacementPC(Displacement, settings)}");
        sb.Append($"({Registers.PC},");
        sb.Append($"{(IndexregisterType == RegisterTypes.Dn ? $"{Registers.Dn[Indexregister]}" : $"{Registers.An[Indexregister]}")}");
        sb.Append($"{(Size == Attributes.Sizes.Word ? $".{Attributes.Size.Word}" : $".{Attributes.Size.Long}")})");

        if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
          return sb.ToString().ToLowerInvariant();
        else
          return sb.ToString().ToUpperInvariant();
      }
    }

    internal override void Serialize(BinaryWriter writer)
    {
      base.Serialize(writer);
      writer.Write(Displacement);
      writer.Write((int)IndexregisterType);
      writer.Write(Indexregister);
      writer.Write((int)Size);
      writer.Write(Label == null ? (byte)0x00 : (byte)0xFF);
      if (Label != null)
        writer.Write(Label.Address);
    }
  }
}
