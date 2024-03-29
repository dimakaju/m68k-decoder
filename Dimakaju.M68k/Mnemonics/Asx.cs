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
  internal abstract class Asx : MnemonicDecoder
  {
    public override bool IsRelevant(ushort data)
      => (data & 0b1111000011011000) == 0b11100000_00_000000
      || (data & 0b1111000011011000) == 0b11100000_01_000000
      || (data & 0b1111000011011000) == 0b11100000_10_000000
      || (data & 0b1111111011000000) == 0b11100000_11_000000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b1110)
        return null;

      int countOrRegister = reader.Read(3);

      if (IsCorrectType(reader) == false)
        return null;

      int size = reader.Read(2);

      Instruction instruction = new Instruction(Type);

      if (size == 0b11)
      {
        // Memory Shift
        if (countOrRegister != 0b000)
          return null;

        int ea = reader.Read(6);

        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand1 = DecodeOperand(reader, ea, Attributes.Sizes.Word, 0b001111111000);
        return instruction.Check(dst: false);
      }
      else
      {
        // Register Shift
        int ir = reader.Read(1);

        if (reader.Read(2) != 0b00)
          return null;

        int register = reader.Read(3);

        if (size == 0b00)
          instruction.Size = Attributes.Sizes.Byte;
        else if (size == 0b01)
          instruction.Size = Attributes.Sizes.Word;
        else if (size == 0b10)
          instruction.Size = Attributes.Sizes.Long;

        if (ir == 0b0)
        {
          instruction.Operand1 = new ImmediateData(countOrRegister == 0 ? 8 : countOrRegister);
          instruction.Operand2 = new DnDirect(register);
          return instruction.Check();
        }
        else
        {
          instruction.Operand1 = new DnDirect(countOrRegister);
          instruction.Operand2 = new DnDirect(register);
          return instruction.Check();
        }
      }
    }

    protected abstract bool IsCorrectType(BitStreamReader reader);
  }
}
