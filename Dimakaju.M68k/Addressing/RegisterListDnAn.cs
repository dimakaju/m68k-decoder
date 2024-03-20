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

using System.IO;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Addressing
{
  public class RegisterListDnAn : RegisterList
  {
    internal RegisterListDnAn(BinaryReader reader)
      : base(reader)
    {
    }

    internal RegisterListDnAn(int mask)
      : base()
    {
      Dn[0] = (mask & 1 << 15) != 0;
      Dn[1] = (mask & 1 << 14) != 0;
      Dn[2] = (mask & 1 << 13) != 0;
      Dn[3] = (mask & 1 << 12) != 0;
      Dn[4] = (mask & 1 << 11) != 0;
      Dn[5] = (mask & 1 << 10) != 0;
      Dn[6] = (mask & 1 << 9) != 0;
      Dn[7] = (mask & 1 << 8) != 0;

      An[0] = (mask & 1 << 7) != 0;
      An[1] = (mask & 1 << 6) != 0;
      An[2] = (mask & 1 << 5) != 0;
      An[3] = (mask & 1 << 4) != 0;
      An[4] = (mask & 1 << 3) != 0;
      An[5] = (mask & 1 << 2) != 0;
      An[6] = (mask & 1 << 1) != 0;
      An[7] = (mask & 1 << 0) != 0;
    }

    public override OperandInfo Type => OperandInfo.RegisterListDnAn;
  }
}
