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

using System;
using System.IO;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Addressing
{
  public abstract class RegisterList : Operand
  {
    internal RegisterList(BinaryReader reader)
      : base(reader)
    {
      for (int i = 0; i < Dn.Length; i++)
        Dn[i] = reader.ReadBoolean();

      for (int i = 0; i < An.Length; i++)
        An[i] = reader.ReadBoolean();
    }

    internal RegisterList()
      : base()
    {
    }

    public bool[] Dn { get; } = new bool[8];

    public bool[] An { get; } = new bool[8];

    internal override string Render(RenderingSettings settings)
    {
      string dnl = Formatting.RegisterList(Dn, RegisterTypes.Dn, settings);
      string anl = Formatting.RegisterList(An, RegisterTypes.An, settings);
      string result = string.Empty;

      if (string.IsNullOrWhiteSpace(dnl) == false && string.IsNullOrWhiteSpace(anl) == false)
        result = $"{dnl}/{anl}";
      else if (string.IsNullOrWhiteSpace(dnl) == true && string.IsNullOrWhiteSpace(anl) == false)
        result = $"{anl}";
      else if (string.IsNullOrWhiteSpace(dnl) == false && string.IsNullOrWhiteSpace(anl) == true)
        result = $"{dnl}";
      else
        throw new Exception($"Rregisterlist An and Dn is empty, cannot render this movem instruction.");

      if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
        return result.ToLowerInvariant();
      else
        return result.ToUpperInvariant();
    }

    internal override void Serialize(BinaryWriter writer)
    {
      base.Serialize(writer);

      for (int i = 0; i < Dn.Length; i++)
        writer.Write(Dn[i]);

      for (int i = 0; i < An.Length; i++)
        writer.Write(An[i]);
    }
  }
}
