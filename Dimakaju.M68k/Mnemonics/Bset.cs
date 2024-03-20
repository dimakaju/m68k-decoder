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
  internal class Bset : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Bset;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b0000)
        return null;

      int register = reader.Read(3);
      int constant = reader.Read(3);
      int ea = reader.Read(6);

      Instruction instruction = new Instruction(Type);

      if (constant == 0b111)
      {
        instruction.Operand1 = new DnDirect(register);
        instruction.Operand2 = DecodeOperand(reader, ea, 0b101111111000);
        instruction.Size = instruction.Operand2 is DnDirect ? Attributes.Sizes.Long : Attributes.Sizes.Byte;
        instruction.DoNotRenderSize = true;
        return instruction.Check();
      }
      else if (register == 0b100 && constant == 0b011)
      {
        if (reader.Read(8) != 0b00000000)
          return null;

        instruction.Operand1 = new ImmediateData(reader.DecodeSigned8());
        instruction.Operand2 = DecodeOperand(reader, ea, 0b101111111000);
        instruction.Size = instruction.Operand2 is DnDirect ? Attributes.Sizes.Long : Attributes.Sizes.Byte;
        instruction.DoNotRenderSize = true;
        return instruction.Check();
      }

      return null;
    }
  }
}
