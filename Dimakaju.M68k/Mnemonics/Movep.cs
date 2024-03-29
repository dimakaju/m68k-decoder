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
  internal class Movep : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Movep;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111000111111000) == 0b0000000_100_001000
      || (data & 0b1111000111111000) == 0b0000000_101_001000
      || (data & 0b1111000111111000) == 0b0000000_110_001000
      || (data & 0b1111000111111000) == 0b0000000_111_001000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b0000)
        return null;

      int dnregister = reader.Read(3);
      int opmode = reader.Read(3);

      if (reader.Read(3) != 0b001)
        return null;

      int anregister = reader.Read(3);

      Instruction instruction = new Instruction(Type);

      if (opmode == 0b100)
      {
        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand1 = new AnIndirectDisplacement(anregister, reader.DecodeSigned16());
        instruction.Operand2 = new DnDirect(dnregister);
        return instruction.Check();
      }
      else if (opmode == 0b101)
      {
        instruction.Size = Attributes.Sizes.Long;
        instruction.Operand1 = new AnIndirectDisplacement(anregister, reader.DecodeSigned16());
        instruction.Operand2 = new DnDirect(dnregister);
        return instruction.Check();
      }
      else if (opmode == 0b110)
      {
        instruction.Size = Attributes.Sizes.Word;
        instruction.Operand1 = new DnDirect(dnregister);
        instruction.Operand2 = new AnIndirectDisplacement(anregister, reader.DecodeSigned16());
        return instruction.Check();
      }
      else if (opmode == 0b111)
      {
        instruction.Size = Attributes.Sizes.Long;
        instruction.Operand1 = new DnDirect(dnregister);
        instruction.Operand2 = new AnIndirectDisplacement(anregister, reader.DecodeSigned16());
        return instruction.Check();
      }

      return null;
    }
  }
}
