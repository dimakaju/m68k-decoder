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
  internal class Lea : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Lea;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111000111000000) == 0b0100000111000000;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b0100)
        return null;

      int register = reader.Read(3);

      if (reader.Read(3) != 0b111)
        return null;

      int ea = reader.Read(6);

      Instruction instruction = new Instruction(Type);
      instruction.Size = Attributes.Sizes.Long;
      instruction.DoNotRenderSize = true;
      instruction.Operand1 = DecodeOperand(reader, ea, Attributes.Sizes.Long, 0b001001111011);
      instruction.Operand2 = new AnDirect(register);
      return instruction.Check();
    }
  }
}
