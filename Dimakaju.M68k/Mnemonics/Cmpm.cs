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
  internal class Cmpm : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Cmpm;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b1011)
        return null;

      int ax = reader.Read(3);

      if (reader.Read(1) != 1)
        return null;

      int size = reader.Read(2);

      if (reader.Read(3) != 0b001)
        return null;

      int ay = reader.Read(3);

      Instruction instruction = new Instruction(Type);

      if (size == 0b00)
      {
        instruction.Size = Attributes.Sizes.Byte;
        instruction.Operand1 = new AnIndirectPostincrement(ay);
        instruction.Operand2 = new AnIndirectPostincrement(ax);
        return instruction.Check();
      }
      else if (size == 0b01)
      {
        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand1 = new AnIndirectPostincrement(ay);
        instruction.Operand2 = new AnIndirectPostincrement(ax);
        return instruction.Check();
      }
      else if (size == 0b10)
      {
        instruction.Size = Attributes.Sizes.Long;
        instruction.Operand1 = new AnIndirectPostincrement(ay);
        instruction.Operand2 = new AnIndirectPostincrement(ax);
        return instruction.Check();
      }

      return null;
    }
  }
}
