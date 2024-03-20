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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dimakaju.M68k.Constants
{
  public sealed class OperandInfo
  {
    public static readonly OperandInfo AbsLong = new OperandInfo(0x2001, "AbsLong");
    public static readonly OperandInfo AbsShort = new OperandInfo(0x2002, "AbsShort");
    public static readonly OperandInfo AnDirect = new OperandInfo(0x2003, "AnDirect");
    public static readonly OperandInfo AnIndirect = new OperandInfo(0x2004, "AnIndirect");
    public static readonly OperandInfo AnIndirectDisplacement = new OperandInfo(0x2005, "AnIndirectDisplacement");
    public static readonly OperandInfo AnIndirectIndexDisplacement = new OperandInfo(0x2006, "AnIndirectIndexDisplacement");
    public static readonly OperandInfo AnIndirectPostincrement = new OperandInfo(0x2007, "AnIndirectPostincrement");
    public static readonly OperandInfo AnIndirectPredecrement = new OperandInfo(0x2008, "AnIndirectPredecrement");
    public static readonly OperandInfo CcrDirect = new OperandInfo(0x2009, "CcrDirect");
    public static readonly OperandInfo DnDirect = new OperandInfo(0x200A, "DnDirect");
    public static readonly OperandInfo ImmediateData = new OperandInfo(0x200B, "ImmediateData");
    public static readonly OperandInfo Label = new OperandInfo(0x200C, "Label");
    public static readonly OperandInfo PcIndirectDisplacement = new OperandInfo(0x200D, "PcIndirectDisplacement");
    public static readonly OperandInfo PcIndirectIndexDisplacement = new OperandInfo(0x200E, "PcIndirectIndexDisplacement");
    public static readonly OperandInfo RegisterListAnDn = new OperandInfo(0x200F, "RegisterListAnDn");
    public static readonly OperandInfo RegisterListDnAn = new OperandInfo(0x2010, "RegisterListDnAn");
    public static readonly OperandInfo SrDirect = new OperandInfo(0x2011, "SrDirect");
    public static readonly OperandInfo UspDirect = new OperandInfo(0x2012, "UspDirect");
    public static readonly IReadOnlyList<OperandInfo> Items = OperandInfo.Populate();
    public static readonly int Count = Items.Count;

    private OperandInfo(ushort id, string text)
    {
      Id = id;
      Text = text;
    }

    public ushort Id { get; set; }

    public string Text { get; set; }

    public static OperandInfo FromId(ushort id)
    {
      var result = Items.FirstOrDefault(x => id == x.Id);
      return result ?? throw new KeyNotFoundException($"Cannot find operand info with id \"{id}\".");
    }

    public static bool IsOperand(ushort id)
      => id >= 0x2001 && id <= 0x2012;

    public override string ToString()
      => $"{Text}.{Id:X4}";

    private static OperandInfo[] Populate()
      => typeof(OperandInfo).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(OperandInfo)).Select(x => x.GetValue(null)).Cast<OperandInfo>().ToArray();
  }
}
