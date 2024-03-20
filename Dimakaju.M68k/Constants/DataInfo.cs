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
  public sealed class DataInfo
  {
    public static readonly DataInfo Dcb = new DataInfo(0x1001, "dc.b");
    public static readonly DataInfo Dcbb = new DataInfo(0x1002, "dcb.b");
    public static readonly DataInfo Dcbl = new DataInfo(0x1003, "dcb.l");
    public static readonly DataInfo Dcbs = new DataInfo(0x1004, "dc.b");
    public static readonly DataInfo Dcbw = new DataInfo(0x1005, "dcb.w");
    public static readonly DataInfo Dcl = new DataInfo(0x1006, "dc.l");
    public static readonly DataInfo Dcw = new DataInfo(0x1007, "dc.w");
    public static readonly DataInfo Endmarker = new DataInfo(0x1008, "<end>");
    public static readonly IReadOnlyList<DataInfo> Items = DataInfo.Populate();
    public static readonly int Count = Items.Count;

    private DataInfo(ushort id, string text)
    {
      Id = id;
      Text = text;
    }

    public ushort Id { get; set; }

    public string Text { get; }

    public static DataInfo FromId(ushort id)
    {
      var result = Items.FirstOrDefault(x => id == x.Id);
      return result ?? throw new KeyNotFoundException($"Cannot find data with id \"{id}\".");
    }

    public static bool IsData(ushort id)
      => id >= 0x1001 && id <= 0x1008;

    public override string ToString()
      => $"{Text}.{Id:X4}";

    private static DataInfo[] Populate()
      => typeof(DataInfo).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(DataInfo)).Select(x => x.GetValue(null)).Cast<DataInfo>().ToArray();
  }
}
