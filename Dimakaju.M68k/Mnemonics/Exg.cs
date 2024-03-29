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
  internal class Exg : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Exg;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111000111111000) == 0b11000001_01000_000
      || (data & 0b1111000111111000) == 0b11000001_01001_000
      || (data & 0b1111000111111000) == 0b11000001_10001_000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b1100)
        return null;

      int rx = reader.Read(3);

      if (reader.Read(1) != 0b1)
        return null;

      int opmode = reader.Read(5);
      int ry = reader.Read(3);

      Instruction instruction = new Instruction(Type);

      if (opmode == 0b01000)
      {
        instruction.Size = Constants.Attributes.Sizes.Long;
        instruction.Operand1 = new DnDirect(rx);
        instruction.Operand2 = new DnDirect(ry);
        return instruction.Check();
      }
      else if (opmode == 0b01001)
      {
        instruction.Size = Constants.Attributes.Sizes.Long;
        instruction.Operand1 = new AnDirect(rx);
        instruction.Operand2 = new AnDirect(ry);
        return instruction.Check();
      }
      else if (opmode == 0b10001)
      {
        instruction.Size = Constants.Attributes.Sizes.Long;
        instruction.Operand1 = new DnDirect(rx);
        instruction.Operand2 = new AnDirect(ry);
        return instruction.Check();
      }

      return null;
    }
  }
}
