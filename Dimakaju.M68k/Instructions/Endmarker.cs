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

namespace Dimakaju.M68k.Instructions
{
  public class Endmarker : DecodedData
  {
    internal Endmarker(BinaryReader reader)
      : base(reader)
    {
    }

    internal Endmarker()
    {
    }

    public override byte[] Data { get; internal set; } = Array.Empty<byte>();

    public override int Length
      => 0;

    public DataInfo Type => DataInfo.Endmarker;

    internal override ushort Identifier => Type.Id;

    public override string RenderMnemonic(RenderingSettings settings)
      => settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Type.Text.ToLowerInvariant() : Type.Text.ToLowerInvariant();

    public override string RenderOperands(RenderingSettings settings)
      => string.Empty;

    public override string RenderHexString(RenderingSettings settings)
      => string.Empty;

    public override string RenderAscii(RenderingSettings settings)
      => string.Empty;
  }
}
