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
  public sealed class MnemonicInfo
  {
    public static readonly MnemonicInfo Abcd = new MnemonicInfo(0x0001, "abcd");
    public static readonly MnemonicInfo Add = new MnemonicInfo(0x0002, "add");
    public static readonly MnemonicInfo Adda = new MnemonicInfo(0x0003, "adda");
    public static readonly MnemonicInfo Addi = new MnemonicInfo(0x0004, "addi");
    public static readonly MnemonicInfo Addq = new MnemonicInfo(0x0005, "addq");
    public static readonly MnemonicInfo Addx = new MnemonicInfo(0x0006, "addx");
    public static readonly MnemonicInfo And = new MnemonicInfo(0x0007, "and");
    public static readonly MnemonicInfo Andi = new MnemonicInfo(0x0008, "andi");
    public static readonly MnemonicInfo AndiCcr = new MnemonicInfo(0x0009, "andi");
    public static readonly MnemonicInfo AndiSr = new MnemonicInfo(0x000A, "andi");
    public static readonly MnemonicInfo Asl = new MnemonicInfo(0x000B, "asl");
    public static readonly MnemonicInfo Asr = new MnemonicInfo(0x000C, "asr");
    public static readonly MnemonicInfo Bcc = new MnemonicInfo(0x000D, "bcc");
    public static readonly MnemonicInfo Bchg = new MnemonicInfo(0x000E, "bchg");
    public static readonly MnemonicInfo Bclr = new MnemonicInfo(0x000F, "bclr");
    public static readonly MnemonicInfo Bcs = new MnemonicInfo(0x0010, "bcs");
    public static readonly MnemonicInfo Beq = new MnemonicInfo(0x0011, "beq");
    public static readonly MnemonicInfo Bge = new MnemonicInfo(0x0012, "bge");
    public static readonly MnemonicInfo Bgt = new MnemonicInfo(0x0013, "bgt");
    public static readonly MnemonicInfo Bhi = new MnemonicInfo(0x0014, "bhi");
    public static readonly MnemonicInfo Ble = new MnemonicInfo(0x0015, "ble");
    public static readonly MnemonicInfo Bls = new MnemonicInfo(0x0016, "bls");
    public static readonly MnemonicInfo Blt = new MnemonicInfo(0x0017, "blt");
    public static readonly MnemonicInfo Bmi = new MnemonicInfo(0x0018, "bmi");
    public static readonly MnemonicInfo Bne = new MnemonicInfo(0x0019, "bne");
    public static readonly MnemonicInfo Bpl = new MnemonicInfo(0x001A, "bpl");
    public static readonly MnemonicInfo Bra = new MnemonicInfo(0x001B, "bra");
    public static readonly MnemonicInfo Bset = new MnemonicInfo(0x001C, "bset");
    public static readonly MnemonicInfo Bsr = new MnemonicInfo(0x001D, "bsr");
    public static readonly MnemonicInfo Btst = new MnemonicInfo(0x001E, "btst");
    public static readonly MnemonicInfo Bvc = new MnemonicInfo(0x001F, "bvc");
    public static readonly MnemonicInfo Bvs = new MnemonicInfo(0x0020, "bvs");
    public static readonly MnemonicInfo Chk = new MnemonicInfo(0x0021, "chk");
    public static readonly MnemonicInfo Clr = new MnemonicInfo(0x0022, "clr");
    public static readonly MnemonicInfo Cmp = new MnemonicInfo(0x0023, "cmp");
    public static readonly MnemonicInfo Cmpa = new MnemonicInfo(0x0024, "cmpa");
    public static readonly MnemonicInfo Cmpi = new MnemonicInfo(0x0025, "cmpi");
    public static readonly MnemonicInfo Cmpm = new MnemonicInfo(0x0026, "cmpm");
    public static readonly MnemonicInfo Dbcc = new MnemonicInfo(0x0027, "dbcc");
    public static readonly MnemonicInfo Dbcs = new MnemonicInfo(0x0028, "dbcs");
    public static readonly MnemonicInfo Dbeq = new MnemonicInfo(0x0029, "dbeq");
    public static readonly MnemonicInfo Dbf = new MnemonicInfo(0x002A, "dbf");
    public static readonly MnemonicInfo Dbge = new MnemonicInfo(0x002B, "dbge");
    public static readonly MnemonicInfo Dbgt = new MnemonicInfo(0x002C, "dbgt");
    public static readonly MnemonicInfo Dbhi = new MnemonicInfo(0x002D, "dbhi");
    public static readonly MnemonicInfo Dble = new MnemonicInfo(0x002E, "dble");
    public static readonly MnemonicInfo Dbls = new MnemonicInfo(0x002F, "dbls");
    public static readonly MnemonicInfo Dblt = new MnemonicInfo(0x0030, "dblt");
    public static readonly MnemonicInfo Dbmi = new MnemonicInfo(0x0031, "dbmi");
    public static readonly MnemonicInfo Dbne = new MnemonicInfo(0x0032, "dbne");
    public static readonly MnemonicInfo Dbpl = new MnemonicInfo(0x0033, "dbpl");
    public static readonly MnemonicInfo Dbt = new MnemonicInfo(0x0034, "dbt");
    public static readonly MnemonicInfo Dbvc = new MnemonicInfo(0x0035, "dbvc");
    public static readonly MnemonicInfo Dbvs = new MnemonicInfo(0x0036, "dbvs");
    public static readonly MnemonicInfo Divs = new MnemonicInfo(0x0037, "divs");
    public static readonly MnemonicInfo Divu = new MnemonicInfo(0x0038, "divu");
    public static readonly MnemonicInfo Eor = new MnemonicInfo(0x0039, "eor");
    public static readonly MnemonicInfo Eori = new MnemonicInfo(0x003A, "eori");
    public static readonly MnemonicInfo EoriCcr = new MnemonicInfo(0x003B, "eori");
    public static readonly MnemonicInfo EoriSr = new MnemonicInfo(0x003C, "eori");
    public static readonly MnemonicInfo Exg = new MnemonicInfo(0x003D, "exg");
    public static readonly MnemonicInfo Ext = new MnemonicInfo(0x003E, "ext");
    public static readonly MnemonicInfo Illegal = new MnemonicInfo(0x003F, "illegal");
    public static readonly MnemonicInfo Jmp = new MnemonicInfo(0x0040, "jmp");
    public static readonly MnemonicInfo Jsr = new MnemonicInfo(0x0041, "jsr");
    public static readonly MnemonicInfo Lea = new MnemonicInfo(0x0042, "lea");
    public static readonly MnemonicInfo Link = new MnemonicInfo(0x0043, "link");
    public static readonly MnemonicInfo Lsl = new MnemonicInfo(0x0044, "lsl");
    public static readonly MnemonicInfo Lsr = new MnemonicInfo(0x0045, "lsr");
    public static readonly MnemonicInfo Move = new MnemonicInfo(0x0046, "move");
    public static readonly MnemonicInfo Movea = new MnemonicInfo(0x0047, "movea");
    public static readonly MnemonicInfo MoveCcr = new MnemonicInfo(0x0048, "move");
    public static readonly MnemonicInfo MoveFromSr = new MnemonicInfo(0x0049, "move");
    public static readonly MnemonicInfo Movem = new MnemonicInfo(0x004A, "movem");
    public static readonly MnemonicInfo Movep = new MnemonicInfo(0x004B, "movep");
    public static readonly MnemonicInfo Moveq = new MnemonicInfo(0x004C, "moveq");
    public static readonly MnemonicInfo MoveSr = new MnemonicInfo(0x004D, "move");
    public static readonly MnemonicInfo MoveUsp = new MnemonicInfo(0x004E, "move");
    public static readonly MnemonicInfo Muls = new MnemonicInfo(0x004F, "muls");
    public static readonly MnemonicInfo Mulu = new MnemonicInfo(0x0050, "mulu");
    public static readonly MnemonicInfo Nbcd = new MnemonicInfo(0x0051, "nbcd");
    public static readonly MnemonicInfo Neg = new MnemonicInfo(0x0052, "neg");
    public static readonly MnemonicInfo Negx = new MnemonicInfo(0x0053, "negx");
    public static readonly MnemonicInfo Nop = new MnemonicInfo(0x0054, "nop");
    public static readonly MnemonicInfo Not = new MnemonicInfo(0x0055, "not");
    public static readonly MnemonicInfo Or = new MnemonicInfo(0x0056, "or");
    public static readonly MnemonicInfo Ori = new MnemonicInfo(0x0057, "ori");
    public static readonly MnemonicInfo OriCcr = new MnemonicInfo(0x0058, "ori");
    public static readonly MnemonicInfo OriSr = new MnemonicInfo(0x0059, "ori");
    public static readonly MnemonicInfo Pea = new MnemonicInfo(0x005A, "pea");
    public static readonly MnemonicInfo Reset = new MnemonicInfo(0x005B, "reset");
    public static readonly MnemonicInfo Rol = new MnemonicInfo(0x005C, "rol");
    public static readonly MnemonicInfo Ror = new MnemonicInfo(0x005D, "ror");
    public static readonly MnemonicInfo Roxl = new MnemonicInfo(0x005E, "roxl");
    public static readonly MnemonicInfo Roxr = new MnemonicInfo(0x005F, "roxr");
    public static readonly MnemonicInfo Rte = new MnemonicInfo(0x0060, "rte");
    public static readonly MnemonicInfo Rtr = new MnemonicInfo(0x0061, "rtr");
    public static readonly MnemonicInfo Rts = new MnemonicInfo(0x0062, "rts");
    public static readonly MnemonicInfo Sbdc = new MnemonicInfo(0x0063, "sbdc");
    public static readonly MnemonicInfo Scc = new MnemonicInfo(0x0064, "scc");
    public static readonly MnemonicInfo Scs = new MnemonicInfo(0x0065, "scs");
    public static readonly MnemonicInfo Seq = new MnemonicInfo(0x0066, "seq");
    public static readonly MnemonicInfo Sf = new MnemonicInfo(0x0067, "sf");
    public static readonly MnemonicInfo Sge = new MnemonicInfo(0x0068, "sge");
    public static readonly MnemonicInfo Sgt = new MnemonicInfo(0x0069, "sgt");
    public static readonly MnemonicInfo Shi = new MnemonicInfo(0x006A, "shi");
    public static readonly MnemonicInfo Sle = new MnemonicInfo(0x006B, "sle");
    public static readonly MnemonicInfo Sls = new MnemonicInfo(0x006C, "sls");
    public static readonly MnemonicInfo Slt = new MnemonicInfo(0x006D, "slt");
    public static readonly MnemonicInfo Smi = new MnemonicInfo(0x006E, "smi");
    public static readonly MnemonicInfo Sne = new MnemonicInfo(0x006F, "sne");
    public static readonly MnemonicInfo Spl = new MnemonicInfo(0x0070, "spl");
    public static readonly MnemonicInfo St = new MnemonicInfo(0x0071, "st");
    public static readonly MnemonicInfo Stop = new MnemonicInfo(0x0072, "stop");
    public static readonly MnemonicInfo Sub = new MnemonicInfo(0x0073, "sub");
    public static readonly MnemonicInfo Suba = new MnemonicInfo(0x0074, "suba");
    public static readonly MnemonicInfo Subi = new MnemonicInfo(0x0075, "subi");
    public static readonly MnemonicInfo Subq = new MnemonicInfo(0x0076, "subq");
    public static readonly MnemonicInfo Subx = new MnemonicInfo(0x0077, "subx");
    public static readonly MnemonicInfo Svc = new MnemonicInfo(0x0078, "svc");
    public static readonly MnemonicInfo Svs = new MnemonicInfo(0x0079, "svs");
    public static readonly MnemonicInfo Swap = new MnemonicInfo(0x007A, "swap");
    public static readonly MnemonicInfo Tas = new MnemonicInfo(0x007B, "tas");
    public static readonly MnemonicInfo Trap = new MnemonicInfo(0x007C, "trap");
    public static readonly MnemonicInfo Trapv = new MnemonicInfo(0x007D, "trapv");
    public static readonly MnemonicInfo Tst = new MnemonicInfo(0x007E, "tst");
    public static readonly MnemonicInfo Unlk = new MnemonicInfo(0x007F, "unlk");
    public static readonly IReadOnlyList<MnemonicInfo> Items = MnemonicInfo.Populate();
    public static readonly int Count = Items.Count;

    private MnemonicInfo(ushort id, string text)
    {
      Id = id;
      Text = text;
    }

    public ushort Id { get; set; }

    public string Text { get; }

    public static MnemonicInfo FromId(ushort id)
    {
      var result = Items.FirstOrDefault(x => id == x.Id);
      return result ?? throw new KeyNotFoundException($"Cannot find mnemoinc with id \"{id}\".");
    }

    public static bool IsMnemonic(ushort id)
      => id >= 0x0001 && id <= 0x007F;

    public override string ToString()
      => $"{Text}.{Id:X4}";

    private static MnemonicInfo[] Populate()
      => typeof(MnemonicInfo).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(MnemonicInfo)).Select(x => x.GetValue(null)).Cast<MnemonicInfo>().ToArray();
  }
}
