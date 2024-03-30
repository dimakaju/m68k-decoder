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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Dimakaju.M68k.Constants;
using Dimakaju.M68k.Instructions;

namespace Dimakaju.M68k
{
  public class DisassemblyMap
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DisassemblyMap"/> class.
    /// </summary>
    /// <param name="stream">Stream to be decoded</param>
    public DisassemblyMap(Stream stream)
    {
      BinaryReader reader = new BinaryReader(stream);

      if (reader.ReadUInt32() != 0xC0DEFACEU)
        throw new Exception("Magic not found. Is this a serialized map file?");

      if (reader.ReadUInt32() != 0x1U)
        throw new Exception("Unknown or unsupported map file version.");

      var data = (Dictionary<long, DecodedData>)Data;
      var label = (Dictionary<long, AddressLabel>)Labels;

      try
      {
        StartAddress = reader.ReadInt64();
        EndAddress = reader.ReadInt64();

        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
          var item = new AddressLabel(reader);
          label.Add(item.Address, item);
        }

        count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
          var id = reader.ReadUInt16();

          if (DataInfo.IsData(id) == false && MnemonicInfo.IsMnemonic(id) == false)
            throw new Exception("Cannot read map file. Data or instruction id expected.");

          if (id == DataInfo.Dcb.Id)
          {
            var item = new Dcb(reader);
            data.Add(item.Address, item);
          }
          else if (id == DataInfo.Dcbb.Id)
          {
            var item = new Dcbb(reader);
            data.Add(item.Address, item);
          }
          else if (id == DataInfo.Dcbl.Id)
          {
            var item = new Dcbl(reader);
            data.Add(item.Address, item);
          }
          else if (id == DataInfo.Dcbs.Id)
          {
            var item = new Dcbs(reader);
            data.Add(item.Address, item);
          }
          else if (id == DataInfo.Dcbw.Id)
          {
            var item = new Dcbw(reader);
            data.Add(item.Address, item);
          }
          else if (id == DataInfo.Dcl.Id)
          {
            var item = new Dcl(reader);
            data.Add(item.Address, item);
          }
          else if (id == DataInfo.Dcw.Id)
          {
            var item = new Dcw(reader);
            data.Add(item.Address, item);
          }
          else if (id == DataInfo.Endmarker.Id)
          {
            var item = new Endmarker(reader);
            data.Add(item.Address, item);
          }
          else
          {
            var item = new Instruction(reader, id, Labels);
            data.Add(item.Address, item);
          }
        }

        GenerateLabelDisplacements();
      }
      catch (Exception x)
      {
        throw new Exception($"Cannot load map file: {x.Message}. It's likely corrupted.", x);
      }
    }

    internal DisassemblyMap(long startAddress, long endAddress)
    {
      StartAddress = startAddress;
      EndAddress = endAddress;

      var label = new AddressLabel(startAddress, $"lbl{Formatting.Hex((uint)startAddress)}");
      var dictionary = (Dictionary<long, AddressLabel>)Labels;
      dictionary.Add(label.Address, label);
      label.IsOutOfRange = false;
    }

    /// <summary>
    /// The start address of the disassembly map
    /// </summary>
    public long StartAddress { get; }

    /// <summary>
    /// The end address of the disassembly map
    /// </summary>
    public long EndAddress { get; }

    /// <summary>
    /// A readonly dictionary of addresses and decoded data. Decoded data can be e.g. a instruction.
    /// </summary>
    public IReadOnlyDictionary<long, DecodedData> Data { get; } = new Dictionary<long, DecodedData>();

    /// <summary>
    /// A readonly dictionary of addresses and labels.
    /// </summary>
    public IReadOnlyDictionary<long, AddressLabel> Labels { get; } = new Dictionary<long, AddressLabel>();

    /// <summary>
    /// Tries to get an address label at a specified address.
    /// </summary>
    /// <param name="address">The address</param>
    /// <param name="label">The label</param>
    /// <returns>True if address is found and has a label</returns>
    public bool TryGetLabel(long address, [NotNullWhen(true)] out AddressLabel? label)
    {
      if (Labels.TryGetValue(address, out label) == true)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Tries to get decoded data at a specified address. Decoded data can be e.g. a instruction.
    /// </summary>
    /// <param name="address">The address</param>
    /// <param name="data">The data</param>
    /// <returns>True if address is found</returns>
    public bool TryGetData(long address, [NotNullWhen(true)] out DecodedData? data)
    {
      if (Data.TryGetValue(address, out data) == true)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Saves the current instance of this map into a stream.
    /// </summary>
    /// <param name="stream">The stream to save to</param>
    public void SaveTo(Stream stream)
    {
      BinaryWriter writer = new BinaryWriter(stream);
      writer.Write(0xC0DEFACEU);   // Magic
      writer.Write(0x1U);          // Map-Version
      writer.Write(StartAddress);
      writer.Write(EndAddress);

      writer.Write(Labels.Count);
      foreach (var label in Labels.Values)
        writer.Write(label.Serialize());

      writer.Write(Data.Count);
      foreach (var data in Data.Values)
        writer.Write(data.Serialize());

      writer.Flush();
    }

    internal void RenameLabel(long address, string name)
    {
      if (TryRenameLabel(address, name) == false)
        throw new Exception($"There is no label associated woth address ${address:X8}");
    }

    internal bool TryRenameLabel(long address, string name)
    {
      _ = name ?? throw new ArgumentNullException(nameof(name));

      var dictionary = (Dictionary<long, AddressLabel>)Labels;

      if (dictionary.TryGetValue(address, out AddressLabel? value) == true)
      {
        value.Name = name;
        return true;
      }
      else
      {
        return false;
      }
    }

    internal void Put(DecodedData data)
    {
      var dictionary = (Dictionary<long, DecodedData>)Data;
      dictionary.Add(data.Address, data);
    }

    internal AddressLabel Put(AddressLabel label, long instructionAddress)
    {
      var dictionary = (Dictionary<long, AddressLabel>)Labels;

      if (dictionary.TryGetValue(label.Address, out AddressLabel? value) == true)
      {
        value.AddInstructionAddress(instructionAddress);
        return value;
      }

      label.AddInstructionAddress(instructionAddress);
      dictionary.Add(label.Address, label);
      label.IsOutOfRange = label.Address < StartAddress || label.Address >= EndAddress;
      return label;
    }

    internal void RemoveRange(long start, long end)
    {
      if (start == end)
        return;

      if (start > end)
        throw new Exception($"Cannot extract data. Start > End (${start:X8}>${end:X8}).");

      if (Data.ContainsKey(start) == false)
        throw new Exception($"Cannot extract data. Start ${start:X8} is not available in map.");

      if (Data.ContainsKey(end) == false)
        throw new Exception($"Cannot extract data. End ${end:X8} is not available in map.");

      var dictionary = (Dictionary<long, DecodedData>)Data;
      for (long i = start; i < end; i++)
        dictionary.Remove(i);
    }

    internal byte[] ExtractData(long start, long end)
    {
      if (start == end)
        return Array.Empty<byte>();

      if (start > end)
        throw new Exception($"Cannot extract data. Start > End (${start:X8}>${end:X8}).");

      if (Data.ContainsKey(start) == false)
        throw new Exception($"Cannot extract data. Start ${start:X8} is not available in map.");

      if (Data.ContainsKey(end) == false)
        throw new Exception($"Cannot extract data. End ${end:X8} is not available in map.");

      int capacity = (int)(end - start);
      List<byte> data = new List<byte>();

      long address = start;
      while (true)
      {
        if (Data.TryGetValue(address, out DecodedData value) == true)
          data.AddRange(value.Data);

        if (++address == end)
          break;
      }

      return data.ToArray();
    }

    internal void GenerateLabelDisplacements()
    {
      foreach (var label in Labels.Values.Where(x => x.IsOutOfRange == false).OrderBy(x => x.Address))
      {
        if (Data.TryGetValue(label.Address, out _) == false)
        {
          var dlabel1 = Labels.Values.Where(x => x.Address < label.Address && x.DisplacementLabel == null).FirstOrDefault();
          if (dlabel1 != null)
            label.DisplacementLabel = dlabel1;
          else
            label.DisplacementLabel = null;
        }
        else
        {
          label.DisplacementLabel = null;
        }
      }
    }

    internal void CleanupLabels(long start, long end)
    {
      var labels = Labels.Values.Where(x => x.InstructionAddresses.Any(y => y >= start && y < end));
      foreach (var label in labels)
        label.RemoveInstructionAddressRange(start, end);
    }
  }
}
