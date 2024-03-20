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
using System.Linq;
using System.Text;

namespace Dimakaju.M68k.Constants
{
  public static class Formatting
  {
    public static string Data8(byte data, RenderingSettings settings)
    {
      if (settings.UseHex == true)
        return $"{Hex(data)}";
      else
        return $"{data}";
    }

    public static string Data16(ushort data, RenderingSettings settings)
    {
      if (settings.UseHex == true)
        return $"{Hex(data)}";
      else
        return $"{data}";
    }

    public static string Data32(uint data, RenderingSettings settings)
    {
      if (settings.UseHex == true)
        return $"{Hex(data)}";
      else
        return $"{data}";
    }

    public static string Address16(int address, RenderingSettings settings)
    {
      if (settings.UseHex == true)
        return $"{Hex(address)}.{Attributes.Size.Word}";
      else
        return $"{address}.{Attributes.Size.Word}";
    }

    public static string Address32(uint address, RenderingSettings settings)
    {
      if (settings.UseHex == true)
        return $"{Hex(address)}";
      else
        return $"{address}";
    }

    public static string Address32(int address, RenderingSettings settings)
    {
      if (settings.UseHex == true)
        return $"{Hex(address)}";
      else
        return $"{address}";
    }

    public static string Displacement(int displacement, RenderingSettings settings)
    {
      if (displacement < 0 && settings.UseHex == true)
        return $"-{Hex(Math.Abs(displacement))}";
      if (displacement >= 0 && settings.UseHex == true)
        return $"{Hex(displacement)}";
      else
        return $"{displacement}";
    }

    public static string DisplacementPC(int displacement, RenderingSettings settings)
    {
      if (displacement == 0)
        return $"*";
      if (displacement < 0 && settings.UseHex == true)
        return $"*-{Hex(Math.Abs(displacement))}";
      if (displacement > 0 && settings.UseHex == true)
        return $"*+{Hex(displacement)}";
      else
        return $"+{displacement}";
    }

    public static string Label(int position, RenderingSettings settings)
    {
      if (position == 0)
        return $"*";
      if (position < 0 && settings.UseHex == true)
        return $"*-{Hex(Math.Abs(position))}";
      if (position > 0 && settings.UseHex == true)
        return $"*+{Hex(position)}";
      else
        return $"*+{position}";
    }

    public static string Immediate(int imm, RenderingSettings settings)
    {
      if (settings.UseHex)
        return $"#{Hex(imm)}";
      else
        return $"#{imm}";
    }

    public static string RegisterList(bool[] list, RegisterTypes registerType, RenderingSettings settings)
    {
      if (list.Length != 8)
        throw new InvalidOperationException($"Register list has to be of size 8.");

      if (list.All(x => x == false) == true)
        return string.Empty;

      bool[] rl = new bool[9];
      Array.Copy(list, rl, list.Length);
      StringBuilder sb = new StringBuilder(64);
      string rt = registerType == RegisterTypes.Dn ? "d" : "a";
      sb.Append(rl[0] ? $"/{rt}0" : string.Empty);
      for (int i = 1; i < rl.Length; i++)
      {
        if (rl[i] == true && rl[i - 1] == false)
          sb.Append($"/{rt}{i}");
        else if (rl[i] == false && rl[i - 1] == true && sb[sb.Length - 1] != $"{i - 1}"[0])
          sb.Append($"-{rt}{i - 1}");
      }

      return sb.ToString().Substring(1);
    }

    public static string Hex(long input)
    {
      string result = $"{input:X8}".TrimStart('0');
      return string.IsNullOrWhiteSpace(result) ? "$0" : "$" + result;
    }

    public static string Hex(uint input)
    {
      string result = $"{input:X8}".TrimStart('0');
      return string.IsNullOrWhiteSpace(result) ? "$0" : "$" + result;
    }

    public static string Hex(int input)
    {
      string result = $"{input:X8}".TrimStart('0');
      return string.IsNullOrWhiteSpace(result) ? "$0" : "$" + result;
    }
  }
}
