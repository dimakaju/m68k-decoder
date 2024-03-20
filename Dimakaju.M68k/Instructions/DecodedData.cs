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
using System.Text;

namespace Dimakaju.M68k.Instructions
{
  public abstract class DecodedData
  {
    internal DecodedData(BinaryReader reader)
    {
      Address = reader.ReadInt64();
      var length = reader.ReadInt32();
      Data = reader.ReadBytes(length);
    }

    internal DecodedData()
    {
    }

    /// <summary>
    /// The address of this data-item
    /// </summary>
    public long Address { get; internal set; }

    /// <summary>
    /// The length of this data-item
    /// </summary>
    public abstract int Length { get; }

    /// <summary>
    /// The raw-data
    /// </summary>
    public abstract byte[] Data { get; internal set; }

    internal abstract ushort Identifier { get; }

    /// <summary>
    /// Renders the mnemonic of the instruction.
    /// </summary>
    /// <param name="settings">Rendering settings</param>
    /// <returns>Formatted string</returns>
    public abstract string RenderMnemonic(RenderingSettings settings);

    /// <summary>
    /// Renders the operands of the instruction.
    /// </summary>
    /// <param name="settings">Rendering settings</param>
    /// <returns>Formatted string</returns>
    public abstract string RenderOperands(RenderingSettings settings);

    /// <summary>
    /// Renders the hex-string of the instruction (e.g. 2C 78 00 04).
    /// </summary>
    /// <param name="settings">Rendering settings</param>
    /// <returns>Formatted string</returns>
    public virtual string RenderHexString(RenderingSettings settings)
    {
      var sb = new StringBuilder(Data.Length * 3);

      if (Data != null && Data.Length > 0)
      {
        if (settings.CharacterCasing == RenderingSettings.InstructionCase.Lower)
        {
          for (int i = 0; i < Data.Length; i++)
            sb.Append($"{Data[i]:x2} ");
        }
        else
        {
          for (int i = 0; i < Data.Length; i++)
            sb.Append($"{Data[i]:X2} ");
        }
      }

      return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Renders the ascii-string of the instruction.
    /// </summary>
    /// <param name="settings">Rendering settings</param>
    /// <returns>Formatted string</returns>
    public virtual string RenderAscii(RenderingSettings settings)
    {
      var sb = new StringBuilder(Data.Length * 3);

      if (Data != null && Data.Length > 0)
      {
        for (int i = 0; i < Data.Length; i++)
          sb.Append($"{(Data[i] < 0x20 || Data[i] > 0x7E ? "." : Encoding.ASCII.GetString(Data, i, 1))}");
      }

      return sb.ToString();
    }

    internal byte[] Serialize()
    {
      using (var ms = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
          writer.Write(Identifier);
          writer.Write(Address);
          writer.Write(Length);
          writer.Write(Data);
          Serialize(writer);
          return ms.ToArray();
        }
      }
    }

    protected virtual void Serialize(BinaryWriter writer)
    {
    }
  }
}
