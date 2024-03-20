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
  internal abstract class Bxx : MnemonicDecoder
  {
    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(4) != 0b0110)
        return null;

      if (Condition(reader) == false)
        return null;

      int d8 = reader.DecodeSigned8();
      reader.TakeSnapshot();

      Instruction instruction = new Instruction(Type);

      if (d8 == 0)
      {
        instruction.Size = Attributes.Sizes.Word;
        reader.RevertToSnapshot();
        instruction.Operand1 = new Label(reader.DecodeSigned16());
        return instruction.Check(dst: false);
      }
      else
      {
        instruction.Size = Attributes.Sizes.Short;
        reader.RemoveSnapshot();
        instruction.Operand1 = new Label(d8);
        return instruction.Check(dst: false);
      }
    }

    protected abstract bool Condition(BitStreamReader reader);
  }
}
