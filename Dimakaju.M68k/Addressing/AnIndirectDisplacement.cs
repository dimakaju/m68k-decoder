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

using System.IO;
using System.Net;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Addressing
{
  public class AnIndirectDisplacement : Operand
  {
    internal AnIndirectDisplacement(BinaryReader reader)
      : base(reader)
    {
      Register = reader.ReadInt32();
      Displacement = reader.ReadInt32();
    }

    internal AnIndirectDisplacement(int register, int displacement)
    {
      Register = register;
      Displacement = displacement;
    }

    public int Register { get; }

    public int Displacement { get; }

    public override OperandInfo Type => OperandInfo.AnIndirectDisplacement;

    internal static bool Match(int eamode)
      => eamode == 0b101;

    internal override string Render(RenderingSettings settings)
    {
      string result = $"{Formatting.Displacement(Displacement, settings)}({Registers.An[Register]})";

      if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
        return result.ToLowerInvariant();
      else
        return result.ToUpperInvariant();
    }

    internal override void Serialize(BinaryWriter writer)
    {
      base.Serialize(writer);
      writer.Write(Register);
      writer.Write(Displacement);
    }
  }
}
