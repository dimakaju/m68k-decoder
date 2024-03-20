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
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Addressing
{
  public class DnDirect : Operand
  {
    internal DnDirect(BinaryReader reader)
      : base(reader)
    {
      Register = reader.ReadInt32();
    }

    internal DnDirect(int register)
      => Register = register;

    public int Register { get; }

    public override OperandInfo Type => OperandInfo.DnDirect;

    internal static bool Match(int eamode)
      => eamode == 0b000;

    internal override string Render(RenderingSettings settings)
    {
      string result = $"{Registers.Dn[Register]}";

      if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
        return result.ToLowerInvariant();
      else
        return result.ToUpperInvariant();
    }

    internal override void Serialize(BinaryWriter writer)
    {
      base.Serialize(writer);
      writer.Write(Register);
    }
  }
}
