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

using Dimakaju.M68k.Addressing;
using Dimakaju.M68k.Constants;
using Dimakaju.M68k.Instructions;
using Dimakaju.M68k.Tools;

namespace Dimakaju.M68k.Mnemonics
{
  internal class Trap : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Trap;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111111111110000) == 0b0100111001000000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(12) != 0b010011100100)
        return null;

      Instruction instruction = new Instruction(Type);
      instruction.Size = Constants.Attributes.Sizes.Unsized;
      instruction.Operand1 = new ImmediateData(reader.Read(4));
      return instruction.Check(dst: false);
    }
  }
}
