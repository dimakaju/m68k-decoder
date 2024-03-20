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
using System.Linq;
using System.Text;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Instructions
{
  public class Dcw : DecodedData
  {
    internal Dcw(BinaryReader reader)
      : base(reader)
    {
    }

    internal Dcw(byte[] data)
    {
      Data = new byte[data.Length];
      Array.Copy(data, Data, data.Length);
    }

    public override byte[] Data { get; internal set; } = Array.Empty<byte>();

    public override int Length
      => Data.Length;

    public DataInfo Type => DataInfo.Dcw;

    internal override ushort Identifier => Type.Id;

    public override string RenderMnemonic(RenderingSettings settings)
      => settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Type.Text.ToLowerInvariant() : Type.Text.ToUpperInvariant();

    public override string RenderOperands(RenderingSettings settings)
    {
      StringBuilder sb = new StringBuilder(Data.Length * 2);
      for (int i = 0; i < Data.Length; i += 2)
      {
        ushort data = BitConverter.ToUInt16(Data.Skip(i).Take(2).Reverse().ToArray(), 0);
        sb.Append($"{Formatting.Data16(data, settings)},");
      }

      var result = sb.ToString().Substring(0, sb.Length - 1);
      return settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? result.ToLowerInvariant() : result.ToUpperInvariant();
    }
  }
}
