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
using System.Text;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Instructions
{
  public class Dcbs : Dcb
  {
    internal Dcbs(BinaryReader reader)
      : base(reader)
    {
    }

    internal Dcbs(byte[] data)
      : base(data)
    {
    }

    internal override ushort Identifier => DataInfo.Dcbs.Id;

    public override string RenderOperands(RenderingSettings settings)
    {
      StringBuilder sb = new StringBuilder(Data.Length * 4);
      bool close = false;
      string token = string.Empty;

      for (int i = 0; i < Data.Length; i++)
      {
        if ((Data[i] < 0x20 || Data[i] > 0x7E) && close == true)
        {
          token = settings.UseHex ? Formatting.Hex(Data[i]) : Data[i].ToString();
          token = settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? token.ToLowerInvariant() : token.ToUpperInvariant();
          sb.Append($"\",{token},");
          close = false;
        }
        else if ((Data[i] < 0x20 || Data[i] > 0x7E) && close == false)
        {
          token = settings.UseHex ? Formatting.Hex(Data[i]) : Data[i].ToString();
          token = settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? token.ToLowerInvariant() : token.ToUpperInvariant();
          sb.Append($"{token},");
        }
        else if (close == false)
        {
          sb.Append($"\"{ASCIIEncoding.ASCII.GetString(Data, i, 1)}");
          close = true;
        }
        else
        {
          sb.Append($"{ASCIIEncoding.ASCII.GetString(Data, i, 1)}");
        }
      }

      string result = sb.ToString();
      if (close == false)
        return result.Substring(0, sb.Length - 1);
      else
        return $"{result}\"";
    }
  }
}
