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

using System.IO;
using Dimakaju.M68k.Addressing;
using Dimakaju.M68k.Constants;
using Dimakaju.M68k.Instructions;
using Dimakaju.M68k.Tools;

namespace Dimakaju.M68k
{
    internal abstract class MnemonicDecoder
  {
    public abstract MnemonicInfo Type { get; }

    public Instruction? Decode(BitStreamReader reader, DecoderSettings settings)
    {
      var snapshot = reader.TakeSnapshot();
      var streamPosition = reader.AddressPosition;
      try
      {
        var result = TryDecoding(reader);
        if (result == null)
        {
          reader.RevertToSnapshot();
          return result;
        }
        else
        {
          result.SetOpcode(snapshot.ReadData);
          result.Address = settings.BaseAddress + streamPosition;
          reader.RemoveSnapshot();
          return result;
        }
      }
      catch (EndOfStreamException)
      {
        reader.RevertToSnapshot();
        return null;
      }
    }

    public virtual bool IsRelevant(ushort data)
    {
      return true;
    }

    protected Operand? DecodeOperand(BitStreamReader reader, int ea, uint checkmask)
    {
      // Checkmask = Dn,An,(An),(An)+,-(An),(d16,An),(d8,An,Xn),(xxx).w,(xxx).l,imm,(d16,pc),(d8,pc,Xn)
      int eamode = (ea & 0b00111000) >> 3;
      int earegister = ea & 0b00000111;

      if ((checkmask & 1 << 11) != 0 && DnDirect.Match(eamode) == true)
        return new DnDirect(earegister);

      if ((checkmask & 1 << 10) != 0 && AnDirect.Match(eamode) == true)
        return new AnDirect(earegister);

      if ((checkmask & 1 << 9) != 0 && AnIndirect.Match(eamode) == true)
        return new AnIndirect(earegister);

      if ((checkmask & 1 << 8) != 0 && AnIndirectPostincrement.Match(eamode) == true)
        return new AnIndirectPostincrement(earegister);

      if ((checkmask & 1 << 7) != 0 && AnIndirectPredecrement.Match(eamode) == true)
        return new AnIndirectPredecrement(earegister);

      if ((checkmask & 1 << 6) != 0 && AnIndirectDisplacement.Match(eamode) == true)
        return new AnIndirectDisplacement(earegister, reader.DecodeSigned16());

      if ((checkmask & 1 << 5) != 0 && AnIndirectIndexDisplacement.Match(eamode) == true)
      {
        int ixmask = reader.Read(8);
        if ((ixmask & 0b00000111) != 0)
          return null;
        else
          return new AnIndirectIndexDisplacement(earegister, ixmask, reader.DecodeSigned8());
      }

      if ((checkmask & 1 << 4) != 0 && AbsShort.Match(eamode, earegister) == true)
        return new AbsShort(reader.DecodeSigned16());

      if ((checkmask & 1 << 3) != 0 && AbsLong.Match(eamode, earegister) == true)
        return new AbsLong(reader.DecodeSigned32());

      if ((checkmask & 1 << 1) != 0 && PcIndirectDisplacement.Match(eamode, earegister) == true)
        return new PcIndirectDisplacement(reader.DecodeSigned16());

      if ((checkmask & 1) != 0 && PcIndirectIndexDisplacement.Match(eamode, earegister) == true)
      {
        int ixmask = reader.Read(8);
        if ((ixmask & 0b00000111) != 0)
          return null;
        else
          return new PcIndirectIndexDisplacement(ixmask, reader.DecodeSigned8());
      }

      return null;
    }

    protected Operand? DecodeOperand(BitStreamReader reader, int ea, Attributes.Sizes size, uint checkmask)
    {
      var result = DecodeOperand(reader, ea, checkmask);
      if (result != null)
        return result;

      int eamode = (ea & 0b00111000) >> 3;
      int earegister = ea & 0b00000111;

      if ((checkmask & 1 << 2) != 0 && ImmediateData.Match(eamode, earegister) == true)
      {
        if (size == Attributes.Sizes.Byte)
          return reader.Immediate8(reader, true);
        else if (size == Attributes.Sizes.Word)
          return reader.Immediate16(reader);
        else if (size == Attributes.Sizes.Long)
          return reader.Immediate32(reader);
        else
          return null;
      }

      return null;
    }

    protected abstract Instruction? TryDecoding(BitStreamReader reader);
  }
}
