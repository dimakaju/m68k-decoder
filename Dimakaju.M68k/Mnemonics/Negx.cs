﻿/*
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

using Dimakaju.M68k.Constants;
using Dimakaju.M68k.Instructions;
using Dimakaju.M68k.Tools;

namespace Dimakaju.M68k.Mnemonics
{
  internal class Negx : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Negx;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111111111000000) == 0b01000000_00_000000
      || (data & 0b1111111111000000) == 0b01000000_01_000000
      || (data & 0b1111111111000000) == 0b01000000_10_000000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(8) != 0b01000000)
        return null;

      int size = reader.Read(2);
      int ea = reader.Read(6);

      Instruction instruction = new Instruction(Type);

      if (size == 0b00)
      {
        instruction.Size = Constants.Attributes.Sizes.Byte;
        instruction.Operand1 = DecodeOperand(reader, ea, Constants.Attributes.Sizes.Byte, 0b101111111000);
        return instruction.Check(dst: false);
      }
      else if (size == 0b01)
      {
        instruction.Size = Constants.Attributes.Sizes.Word;
        instruction.Operand1 = DecodeOperand(reader, ea, Constants.Attributes.Sizes.Word, 0b101111111000);
        return instruction.Check(dst: false);
      }
      else if (size == 0b10)
      {
        instruction.Size = Constants.Attributes.Sizes.Long;
        instruction.Operand1 = DecodeOperand(reader, ea, Constants.Attributes.Sizes.Long, 0b101111111000);
        return instruction.Check(dst: false);
      }

      return null;
    }
  }
}
