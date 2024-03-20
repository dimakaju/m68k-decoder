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
  internal class Movem : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Movem;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(5) != 0b01001)
        return null;

      int dr = reader.Read(1);

      if (reader.Read(3) != 0b001)
        return null;

      int size = reader.Read(1);
      int ea = reader.Read(6);
      int registerlist = reader.Read(16);

      if (registerlist == 0)
        return null;

      Instruction instruction = new Instruction(Type);

      if (dr == 0b0 && size == 0b0)
      {
        // Register to Memory: Word
        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand2 = DecodeOperand(reader, ea, Attributes.Sizes.Word, 0b001011111000);
        if (instruction.Operand2 is AnIndirectPredecrement)
          instruction.Operand1 = new RegisterListDnAn(registerlist);
        else
          instruction.Operand1 = new RegisterListAnDn(registerlist);

        return instruction.Check();
      }
      else if (dr == 0b0 && size == 0b1)
      {
        // Register to Memory: Long
        instruction.Size = Attributes.Sizes.Long;
        instruction.Operand2 = DecodeOperand(reader, ea, Attributes.Sizes.Long, 0b001011111000);
        if (instruction.Operand2 is AnIndirectPredecrement)
          instruction.Operand1 = new RegisterListDnAn(registerlist);
        else
          instruction.Operand1 = new RegisterListAnDn(registerlist);

        return instruction.Check();
      }
      else if (dr == 0b1 && size == 0b0)
      {
        // Memory to Register: Word
        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand1 = DecodeOperand(reader, ea, Attributes.Sizes.Word, 0b001101111011);
        instruction.Operand2 = new RegisterListAnDn(registerlist);
        return instruction.Check();
      }
      else if (dr == 0b1 && size == 0b1)
      {
        // Memory to Register: Long
        instruction.Size = Attributes.Sizes.Long;
        instruction.Operand1 = DecodeOperand(reader, ea, Attributes.Sizes.Long, 0b001101111011);
        instruction.Operand2 = new RegisterListAnDn(registerlist);
        return instruction.Check();
      }

      return null;
    }
  }
}
