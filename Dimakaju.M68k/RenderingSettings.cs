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

namespace Dimakaju.M68k
{
  public class RenderingSettings
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderingSettings"/> class.
    /// </summary>
    /// <param name="useHex">Render immediates in hex instead of dec</param>
    /// <param name="characterCasing">Character casing of the data</param>
    public RenderingSettings(bool useHex = true, InstructionCase characterCasing = InstructionCase.Lower)
    {
      UseHex = useHex;
      CharacterCasing = characterCasing;
    }

    public enum InstructionCase
    {
      /// <summary>
      /// Data will be rendered in upper-case like e.g. MOVE.L A0,D0
      /// </summary>
      Lower = 0,

      /// <summary>
      /// Data will be rendered in upper-case like e.g. move.l a0,d0
      /// </summary>
      Upper = 1
    }

    public bool UseHex { get; }

    public InstructionCase CharacterCasing { get; }
  }
}
