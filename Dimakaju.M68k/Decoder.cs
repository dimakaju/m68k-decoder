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
using System.Linq;
using System.Reflection;
using Dimakaju.M68k.Addressing;
using Dimakaju.M68k.Instructions;
using Dimakaju.M68k.Tools;

namespace Dimakaju.M68k
{
  public class Decoder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Decoder"/> class.
    /// </summary>
    /// <param name="settings">Decoder settings</param>
    public Decoder(DecoderSettings settings)
    {
      Settings = settings ?? throw new ArgumentNullException(nameof(settings));
      Decoders = InitializeDecoders();
    }

    /// <summary>
    /// Gets the currently applied decoder-settings.
    /// </summary>
    public DecoderSettings Settings { get; }

    private IReadOnlyList<MnemonicDecoder> Decoders { get; }

    /// <summary>
    /// Decodes a stream creating a disassembly map object. The stream needs to be seekable, must be readable and must be deterministic, e.g. a file-stream.
    /// The stream will be decoded all at once.
    /// </summary>
    /// <param name="stream">The stream to be decoded</param>
    /// <returns>A disassembly map</returns>
    public DisassemblyMap Decode(Stream stream)
    {
      if (stream.CanRead == false)
        throw new InvalidOperationException($"Stream has to be readable.");

      if (stream.CanSeek == false)
        throw new InvalidOperationException($"Stream has to be seekable.");

      var reader = new BitStreamReader(stream ?? throw new ArgumentNullException(nameof(stream)));
      DisassemblyMap map = new DisassemblyMap(Settings.BaseAddress, Settings.BaseAddress + reader.Length);
      Decode(map, reader);
      map.Put(new Endmarker() { Address = Settings.BaseAddress + reader.AddressPosition });
      map.GenerateLabelDisplacements();
      return map;
    }

    /// <summary>
    /// Decodes an address range of a disassembly map.
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    public void Decode(DisassemblyMap map, long start, long end)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      if (start % 2 != 0)
        throw new Exception($"Cannot decode an odd address.");

      byte[] data = map.ExtractData(start, end);
      map.RemoveRange(start, end);

      var reader = new BitStreamReader(new MemoryStream(data), start - Settings.BaseAddress);
      Decode(map, reader);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as byte range data (dcb.b).
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    public void SetDcbbRange(DisassemblyMap map, long start, long end)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      byte[] data = map.ExtractData(start, end);
      List<Dcbb.Entry> items = new List<Dcbb.Entry>();

      long address = -1;
      int count = 0;
      byte item = 0;
      for (int i = 0; i < data.Length; i++)
      {
        if (address == -1)
        {
          count = 1;
          item = data[i];
          address = start + i;
        }

        if (i + 1 >= data.Length)
        {
          items.Add(new Dcbb.Entry() { Address = address, Count = count, Item = item });
          address = -1;
          break;
        }

        if (item == data[i + 1])
        {
          count++;
        }
        else
        {
          items.Add(new Dcbb.Entry() { Address = address, Count = count, Item = item });
          address = -1;
        }
      }

      map.RemoveRange(start, end);
      for (int i = 0; i < items.Count; i++)
      {
        byte[] dcdata = new byte[items[i].Count];
        for (int k = 0; k < dcdata.Length; k++)
          dcdata[k] = items[i].Item;
        map.Put(new Dcbb(dcdata, items[i].Item) { Address = items[i].Address });
      }

      map.CleanupLabels(start, end);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as byte range data (dcb.w).
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    public void SetDcbwRange(DisassemblyMap map, long start, long end)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      byte[] data = map.ExtractData(start, end);

      if (data.Length % 2 != 0)
        throw new Exception($"Cannot set dcb.w range. Given range is not divisible by 2.");

      if (data.Length < 2)
        return;

      List<Dcbw.Entry> items = new List<Dcbw.Entry>();

      long address = -1;
      int count = 0;
      byte[] item = new byte[] { 0, 0 };
      for (int i = 0; i < data.Length / 2; i++)
      {
        if (address == -1)
        {
          count = 1;
          item[0] = data[(i * 2) + 0];
          item[1] = data[(i * 2) + 1];
          address = start + (i * 2);
        }

        if (i + 1 >= data.Length / 2)
        {
          var dcbwitem = new Dcbw.Entry() { Address = address, Count = count };
          dcbwitem.Item[0] = item[0];
          dcbwitem.Item[1] = item[1];
          items.Add(dcbwitem);
          address = -1;
          break;
        }

        if (item[0] == data[(i * 2) + 2] && item[1] == data[(i * 2) + 3])
        {
          count++;
        }
        else
        {
          var dcbwitem = new Dcbw.Entry() { Address = address, Count = count };
          dcbwitem.Item[0] = item[0];
          dcbwitem.Item[1] = item[1];
          items.Add(dcbwitem);
          address = -1;
        }
      }

      map.RemoveRange(start, end);
      for (int i = 0; i < items.Count; i++)
      {
        byte[] dcdata = new byte[items[i].Count * 2];
        for (int k = 0; k < dcdata.Length / 2; k++)
        {
          dcdata[(k * 2) + 0] = items[i].Item[0];
          dcdata[(k * 2) + 1] = items[i].Item[1];
        }

        map.Put(new Dcbw(dcdata, items[i].Item) { Address = items[i].Address });
      }

      map.CleanupLabels(start, end);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as byte range data (dcb.l).
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    public void SetDcblRange(DisassemblyMap map, long start, long end)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      byte[] data = map.ExtractData(start, end);

      if (data.Length % 4 != 0)
        throw new Exception($"Cannot set dcb.l range. Given range is not divisible by 4.");

      if (data.Length < 4)
        return;

      List<Dcbl.Entry> items = new List<Dcbl.Entry>();

      long address = -1;
      int count = 0;
      byte[] item = new byte[] { 0, 0, 0, 0 };
      for (int i = 0; i < data.Length / 4; i++)
      {
        if (address == -1)
        {
          count = 1;
          item[0] = data[(i * 4) + 0];
          item[1] = data[(i * 4) + 1];
          item[2] = data[(i * 4) + 2];
          item[3] = data[(i * 4) + 3];
          address = start + (i * 4);
        }

        if (i + 1 >= data.Length / 4)
        {
          var dcblitem = new Dcbl.Entry() { Address = address, Count = count };
          dcblitem.Item[0] = item[0];
          dcblitem.Item[1] = item[1];
          dcblitem.Item[2] = item[2];
          dcblitem.Item[3] = item[3];
          items.Add(dcblitem);
          address = -1;
          break;
        }

        if (item[0] == data[(i * 4) + 4] && item[1] == data[(i * 4) + 5] && item[2] == data[(i * 4) + 6] && item[3] == data[(i * 4) + 7])
        {
          count++;
        }
        else
        {
          var dcblitem = new Dcbl.Entry() { Address = address, Count = count };
          dcblitem.Item[0] = item[0];
          dcblitem.Item[1] = item[1];
          dcblitem.Item[2] = item[2];
          dcblitem.Item[3] = item[3];
          items.Add(dcblitem);
          address = -1;
        }
      }

      map.RemoveRange(start, end);
      for (int i = 0; i < items.Count; i++)
      {
        byte[] dcdata = new byte[items[i].Count * 4];
        for (int k = 0; k < dcdata.Length / 4; k++)
        {
          dcdata[(k * 4) + 0] = items[i].Item[0];
          dcdata[(k * 4) + 1] = items[i].Item[1];
          dcdata[(k * 4) + 2] = items[i].Item[2];
          dcdata[(k * 4) + 3] = items[i].Item[3];
        }

        map.Put(new Dcbl(dcdata, items[i].Item) { Address = items[i].Address });
      }

      map.CleanupLabels(start, end);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as byte range data (db.b).
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    /// <param name="chunkSize">Optional parameter to set the amount of items in one dc.b row</param>
    public void SetDcbRange(DisassemblyMap map, long start, long end, int chunkSize = 1)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      if (chunkSize <= 0)
        throw new InvalidOperationException($"Chunk size must not be 0 or negative.");

      byte[] data = map.ExtractData(start, end);
      map.RemoveRange(start, end);
      int index = 0;

      for (int i = 0; i < data.Length / chunkSize; i++)
      {
        byte[] dcbdata = new byte[chunkSize];
        Array.Copy(data, i * chunkSize, dcbdata, 0, chunkSize);
        map.Put(new Dcb(dcbdata) { Address = start + index });
        index += chunkSize;
      }

      int rest = data.Length - (data.Length / chunkSize * chunkSize);
      if (rest > 0)
      {
        byte[] dcbdatarest = new byte[rest];
        Array.Copy(data, index, dcbdatarest, 0, rest);
        map.Put(new Dcb(dcbdatarest) { Address = start + index });
      }

      map.CleanupLabels(start, end);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as a string literal.
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    public void SetDcbsRange(DisassemblyMap map, long start, long end)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      byte[] data = map.ExtractData(start, end);
      map.RemoveRange(start, end);
      map.Put(new Dcbs(data) { Address = start });

      map.CleanupLabels(start, end);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as byte range data (dc.w).
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    /// <param name="chunkSize">Optional parameter to set the amount of items in one dc.w row</param>
    public void SetDcwRange(DisassemblyMap map, long start, long end, int chunkSize)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      byte[] data = map.ExtractData(start, end);

      if (data.Length % 2 != 0)
        throw new Exception($"Cannot set dc.w range. Given range is not divisible by 2.");

      if (chunkSize <= 0)
        throw new InvalidOperationException($"Chunk size must not be 0 or negative.");

      map.RemoveRange(start, end);

      int index = 0;
      for (int i = 0; i < data.Length / (chunkSize * 2); i++)
      {
        byte[] dcbdata = new byte[chunkSize * 2];
        Array.Copy(data, i * chunkSize * 2, dcbdata, 0, chunkSize * 2);
        var dcl = new Dcw(dcbdata) { Address = start + (i * chunkSize * 2) };
        map.Put(dcl);
        index += chunkSize * 2;
      }

      int rest = data.Length - (data.Length / (chunkSize * 2) * chunkSize * 2);
      if (rest > 0)
      {
        byte[] dcbdatarest = new byte[rest];
        Array.Copy(data, index, dcbdatarest, 0, rest);
        map.Put(new Dcw(dcbdatarest) { Address = start + index });
      }

      map.CleanupLabels(start, end);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as byte range data (dc.l).
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="start">Start address</param>
    /// <param name="end">End address</param>
    /// <param name="chunkSize">Optional parameter to set the amount of items in one dc.l row</param>
    public void SetDclRange(DisassemblyMap map, long start, long end, int chunkSize)
    {
      _ = map ?? throw new ArgumentNullException(nameof(map));

      if (map.StartAddress != Settings.BaseAddress)
        throw new InvalidOperationException($"Map file does not have the same base as the decoder. Map = {map.StartAddress:X8}, Decoder = {Settings.BaseAddress:X8}.");

      if (map.StartAddress > start)
        throw new InvalidOperationException($"Map file does not contain given start address {start:X8}.");

      if (map.EndAddress < end)
        throw new InvalidOperationException($"Map file does not contain given end address {end:X8}.");

      byte[] data = map.ExtractData(start, end);

      if (data.Length % 4 != 0)
        throw new Exception($"Cannot set dc.l range. Given range is not divisible by 4.");

      if (chunkSize <= 0)
        throw new InvalidOperationException($"Chunk size must not be 0 or negative.");

      map.RemoveRange(start, end);

      int index = 0;
      for (int i = 0; i < data.Length / (chunkSize * 4); i++)
      {
        byte[] dcbdata = new byte[chunkSize * 4];
        Array.Copy(data, i * chunkSize * 4, dcbdata, 0, chunkSize * 4);
        var dcl = new Dcl(dcbdata) { Address = start + (i * chunkSize * 4) };
        map.Put(dcl);
        index += chunkSize * 4;
      }

      int rest = data.Length - ((data.Length / (chunkSize * 4)) * chunkSize * 4);
      if (rest > 0)
      {
        byte[] dcbdatarest = new byte[rest];
        Array.Copy(data, index, dcbdatarest, 0, rest);
        map.Put(new Dcl(dcbdatarest) { Address = start + index });
      }

      map.CleanupLabels(start, end);
      map.GenerateLabelDisplacements();
    }

    /// <summary>
    /// Sets an address range inside a disassembly map as byte range data (dc.l).
    /// </summary>
    /// <param name="map">The disassembly map</param>
    /// <param name="address">The address of the label</param>
    /// <param name="name">The new name of the label</param>
    public void RenameLabel(DisassemblyMap map, long address, string name)
      => map.RenameLabel(address, name);

    private void Decode(DisassemblyMap map, BitStreamReader reader)
    {
      while (reader.IsFinished() == false)
      {
        Instruction? result = null;
        for (int i = 0; i < Decoders.Count; i++)
        {
          var decoder = Decoders[i];
          result = decoder.Decode(reader, Settings);
          if (result != null)
          {
            map.Put(result);

            if (result.Operand1 != null && result.Operand1 is ReferenceOperand ea1)
            {
              var label1 = ea1.GenerateLabel(result.Address);
              ea1.Label = map.Put(label1, result.Address);
            }

            if (result.Operand2 != null && result.Operand2 is ReferenceOperand ea2)
            {
              var label2 = ea2.GenerateLabel(result.Address);
              ea2.Label = map.Put(label2, result.Address);
            }

            break;
          }
        }

        if (result == null)
        {
          var streamPosition = reader.AddressPosition;
          map.Put(new Dcb(new byte[] { (byte)reader.Read(8) }) { Address = Settings.BaseAddress + streamPosition });

          if (reader.IsFinished() == false)
          {
            streamPosition = reader.AddressPosition;
            map.Put(new Dcb(new byte[] { (byte)reader.Read(8) }) { Address = Settings.BaseAddress + streamPosition });
          }
        }
      }
    }

    private IReadOnlyList<MnemonicDecoder> InitializeDecoders()
    {
      List<MnemonicDecoder> decoders = new List<MnemonicDecoder>();
      var types = Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof(MnemonicDecoder).IsAssignableFrom(x) && x.IsAbstract == false);

      foreach (var dtype in types)
      {
        var decoder = Activator.CreateInstance(dtype);
        if (decoder != null)
          decoders.Add((MnemonicDecoder)decoder);
      }

      return decoders;
    }
  }
}
