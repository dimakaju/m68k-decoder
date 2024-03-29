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
  internal class Or : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Or;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111000111000000) == 0b1000000_000_000000
      || (data & 0b1111000111000000) == 0b1000000_001_000000
      || (data & 0b1111000111000000) == 0b1000000_010_000000
      || (data & 0b1111000111000000) == 0b1000000_100_000000
      || (data & 0b1111000111000000) == 0b1000000_101_000000
      || (data & 0b1111000111000000) == 0b1000000_110_000000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b1000)
        return null;

      int register = reader.Read(3);
      int opmode = reader.Read(3);
      int ea = reader.Read(6);

      Instruction instruction = new Instruction(Type);

      if (opmode == 0b100)
      {
        instruction.Size = Attributes.Sizes.Byte;
        instruction.Operand1 = new DnDirect(register);
        instruction.Operand2 = DecodeOperand(reader, ea, Attributes.Sizes.Byte, 0b001111111000);
        return instruction.Check();
      }
      else if (opmode == 0b101)
      {
        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand1 = new DnDirect(register);
        instruction.Operand2 = DecodeOperand(reader, ea, Attributes.Sizes.Word, 0b001111111000);
        return instruction.Check();
      }
      else if (opmode == 0b110)
      {
        instruction.Size = Attributes.Sizes.Long;
        instruction.Operand1 = new DnDirect(register);
        instruction.Operand2 = DecodeOperand(reader, ea, Attributes.Sizes.Long, 0b001111111000);
        return instruction.Check();
      }
      else if (opmode == 0b000)
      {
        instruction.Size = Attributes.Sizes.Byte;
        instruction.Operand1 = DecodeOperand(reader, ea, Attributes.Sizes.Byte, 0b101111111111);
        instruction.Operand2 = new DnDirect(register);
        return instruction.Check();
      }
      else if (opmode == 0b001)
      {
        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand1 = DecodeOperand(reader, ea, Attributes.Sizes.Word, 0b101111111111);
        instruction.Operand2 = new DnDirect(register);
        return instruction.Check();
      }
      else if (opmode == 0b010)
      {
        instruction.Size = Attributes.Sizes.Long;
        instruction.Operand1 = DecodeOperand(reader, ea, Attributes.Sizes.Long, 0b101111111111);
        instruction.Operand2 = new DnDirect(register);
        return instruction.Check();
      }

      return null;
    }
  }
}
