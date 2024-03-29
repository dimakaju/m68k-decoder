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
  internal class Sbdc : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Sbdc;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111000111110000) == 0b1000000100000000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b1000)
        return null;

      int ry = reader.Read(3);

      if (reader.Read(5) != 0b10000)
        return null;

      int rm = reader.Read(1);
      int rx = reader.Read(3);

      Instruction instruction = new Instruction(Type);

      if (rm == 0b0)
      {
        instruction.Size = Constants.Attributes.Sizes.Byte;
        instruction.DoNotRenderSize = true;
        instruction.Operand1 = new DnDirect(rx);
        instruction.Operand2 = new DnDirect(ry);
        return instruction.Check();
      }
      else if (rm == 0b1)
      {
        instruction.Size = Constants.Attributes.Sizes.Byte;
        instruction.DoNotRenderSize = true;
        instruction.Operand1 = new AnIndirectPredecrement(rx);
        instruction.Operand2 = new AnIndirectPredecrement(ry);
        return instruction.Check();
      }

      return null;
    }
  }
}
