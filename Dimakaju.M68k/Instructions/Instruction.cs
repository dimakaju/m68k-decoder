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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dimakaju.M68k.Addressing;
using Dimakaju.M68k.Constants;

namespace Dimakaju.M68k.Instructions
{
  public class Instruction : DecodedData
  {
    internal Instruction(BinaryReader reader, ushort id, IReadOnlyDictionary<long, AddressLabel> labels)
      : base(reader)
    {
      Type = MnemonicInfo.FromId(id);

      if (reader.ReadByte() == 0xFF)
        Operand1 = DeserializeOperand(reader, labels);

      if (reader.ReadByte() == 0xFF)
        Operand2 = DeserializeOperand(reader, labels);

      Size = (Attributes.Sizes)reader.ReadInt32();
      DoNotRenderSize = reader.ReadBoolean();
      CpuCompatibility = (CPU)reader.ReadInt32();
    }

    internal Instruction(MnemonicInfo type)
      => Type = type;

    public MnemonicInfo Type { get; internal set; }

    public Attributes.Sizes Size { get; internal set; }

    public bool DoNotRenderSize { get; internal set; }

    public Operand? Operand1 { get; internal set; }

    public Operand? Operand2 { get; internal set; }

    public CPU CpuCompatibility { get; internal set; } = CPU.All;

    public override byte[] Data { get; internal set; } = Array.Empty<byte>();

    public override int Length
      => Data == null ? 0 : Data.Length;

    internal override ushort Identifier
      => Type.Id;

    public override string RenderMnemonic(RenderingSettings settings)
    {
      var sb = new StringBuilder(32);
      sb.Append(settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Type.Text.ToLowerInvariant() : Type.Text.ToUpperInvariant());

      if (Size != Attributes.Sizes.Unsized && DoNotRenderSize == false)
      {
        sb.Append(".");
        if (Size == Attributes.Sizes.Short)
          sb.Append(settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Attributes.Size.Short.ToLowerInvariant() : Attributes.Size.Short.ToUpperInvariant());
        else if (Size == Attributes.Sizes.Byte)
          sb.Append(settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Attributes.Size.Byte.ToLowerInvariant() : Attributes.Size.Byte.ToUpperInvariant());
        else if (Size == Attributes.Sizes.Word)
          sb.Append(settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Attributes.Size.Word.ToLowerInvariant() : Attributes.Size.Word.ToUpperInvariant());
        else if (Size == Attributes.Sizes.Long)
          sb.Append(settings.CharacterCasing == RenderingSettings.InstructionCase.Lower ? Attributes.Size.Long.ToLowerInvariant() : Attributes.Size.Long.ToUpperInvariant());
        else
          throw new Exception("Unknown size.");
      }

      return sb.ToString();
    }

    public override string RenderOperands(RenderingSettings settings)
    {
      var sb = new StringBuilder(32);

      if (Operand1 != null && Operand2 != null)
        sb.Append($"{Operand1.Render(settings)},{Operand2.Render(settings)}");
      else if (Operand1 != null && Operand2 == null)
        sb.Append($"{Operand1.Render(settings)}");
      else if (Operand1 == null && Operand2 == null)
        sb.Append(string.Empty);
      else
        throw new Exception("Unknown or unsupported source/destination combination.");

      return sb.ToString();
    }

    internal Instruction? Check(bool src = true, bool dst = true)
    {
      bool ok = true;

      if (src == true)
        ok = ok && Operand1 != null;

      if (dst == true)
        ok = ok && Operand2 != null;

      return ok ? this : null;
    }

    internal void SetOpcode(IReadOnlyList<byte> opcode)
    {
      Data = new byte[opcode.Count];
      for (int i = 0; i < Data.Length; i++)
        Data[i] = opcode[i];
    }

    protected override void Serialize(BinaryWriter writer)
    {
      writer.Write(Operand1 == null ? (byte)0x00 : (byte)0xFF);
      Operand1?.Serialize(writer);
      writer.Write(Operand2 == null ? (byte)0x00 : (byte)0xFF);
      Operand2?.Serialize(writer);
      writer.Write((int)Size);
      writer.Write(DoNotRenderSize);
      writer.Write((int)CpuCompatibility);
    }

    private Operand DeserializeOperand(BinaryReader reader, IReadOnlyDictionary<long, AddressLabel> labels)
    {
      var id = reader.ReadUInt16();

      if (OperandInfo.IsOperand(id) == false)
        throw new Exception("Cannot read map file. Operand id expected.");

      if (id == OperandInfo.AbsLong.Id)
        return new AbsLong(reader, labels);
      else if (id == OperandInfo.AbsShort.Id)
        return new AbsShort(reader, labels);
      else if (id == OperandInfo.AnDirect.Id)
        return new AnDirect(reader);
      else if (id == OperandInfo.AnIndirect.Id)
        return new AnIndirect(reader);
      else if (id == OperandInfo.AnIndirectDisplacement.Id)
        return new AnIndirectDisplacement(reader);
      else if (id == OperandInfo.AnIndirectIndexDisplacement.Id)
        return new AnIndirectIndexDisplacement(reader);
      else if (id == OperandInfo.AnIndirectPostincrement.Id)
        return new AnIndirectPostincrement(reader);
      else if (id == OperandInfo.AnIndirectPredecrement.Id)
        return new AnIndirectPredecrement(reader);
      else if (id == OperandInfo.CcrDirect.Id)
        return new CcrDirect(reader);
      else if (id == OperandInfo.DnDirect.Id)
        return new DnDirect(reader);
      else if (id == OperandInfo.ImmediateData.Id)
        return new ImmediateData(reader);
      else if (id == OperandInfo.Label.Id)
        return new Label(reader, labels);
      else if (id == OperandInfo.PcIndirectDisplacement.Id)
        return new PcIndirectDisplacement(reader, labels);
      else if (id == OperandInfo.PcIndirectIndexDisplacement.Id)
        return new PcIndirectIndexDisplacement(reader, labels);
      else if (id == OperandInfo.RegisterListAnDn.Id)
        return new RegisterListAnDn(reader);
      else if (id == OperandInfo.RegisterListDnAn.Id)
        return new RegisterListDnAn(reader);
      else if (id == OperandInfo.SrDirect.Id)
        return new SrDirect(reader);
      else if (id == OperandInfo.UspDirect.Id)
        return new UspDirect(reader);
      else
        throw new Exception($"Unknown or unsupported operator identifier {id}.");
    }
  }
}
