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

using Dimakaju.M68k.Addressing;
using Dimakaju.M68k.Constants;
using Dimakaju.M68k.Instructions;
using Dimakaju.M68k.Tools;

namespace Dimakaju.M68k.Mnemonics
{
  internal class AndiCcr : MnemonicDecoder
  {
    public override MnemonicInfo Type => MnemonicInfo.Andi;

    public override bool IsRelevant(ushort data)
      => (data & 0b1111111111111111) == 0b0000001000111100;

    protected override Instruction? TryDecoding(BitStreamReader reader)
    {
      if (reader.Read(24) != 0b000000100011110000000000)
        return null;

      int data = reader.DecodeSigned8();

      Instruction instruction = new Instruction(Type);
      instruction.Size = Attributes.Sizes.Byte;
      instruction.Operand1 = new ImmediateData(data);
      instruction.Operand2 = new CcrDirect();
      return instruction.Check();
    }
  }
}
