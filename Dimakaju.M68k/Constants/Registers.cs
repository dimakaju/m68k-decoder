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

namespace Dimakaju.M68k.Constants
{
  public enum RegisterTypes
  {
    Dn = 0,
    An = 1,
  }

  public static class Registers
  {
    public static readonly string D0 = "d0";
    public static readonly string D1 = "d1";
    public static readonly string D2 = "d2";
    public static readonly string D3 = "d3";
    public static readonly string D4 = "d4";
    public static readonly string D5 = "d5";
    public static readonly string D6 = "d6";
    public static readonly string D7 = "d7";
    public static readonly string[] Dn = new string[] { D0, D1, D2, D3, D4, D5, D6, D7 };

    public static readonly string A0 = "a0";
    public static readonly string A1 = "a1";
    public static readonly string A2 = "a2";
    public static readonly string A3 = "a3";
    public static readonly string A4 = "a4";
    public static readonly string A5 = "a5";
    public static readonly string A6 = "a6";
    public static readonly string A7 = "a7";
    public static readonly string[] An = new string[] { A0, A1, A2, A3, A4, A5, A6, A7 };

    public static readonly string PC = "pc";
    public static readonly string CCR = "ccr";
    public static readonly string SR = "sr";
    public static readonly string USP = "usp";
  }
}
