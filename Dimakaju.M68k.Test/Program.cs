using Dimakaju.M68k;
using Dimakaju.M68k.Test;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DecoderTest
{
  // Quick and dirty tool to demonstrate all available featues of the M68k decoder dll.
  internal class Program
  {
    static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Arguments.PrintHelp(false);
        return;
      }

      if (args[0] == "new" && (args.Length == 2 || args.Length == 3))
      {
        var baseAddress = 0x10000L;
        if (args.Length == 3)
        {
          if (args[2].StartsWith("--base=") && Arguments.TryParseBaseAddress(args[2], out baseAddress) == false)
          {
            Arguments.PrintHelp(true);
            return;
          }
        }
        StartNew(args[1], baseAddress);
        return;
      }
      else if (args[0] == "load" && args.Length == 2)
      {
        LoadMap(args[1]);
        return;
      }

      Arguments.PrintHelp(true);
    }

    // Start a new disassembly
    static void StartNew(string file, long baseAddress)
    {
      try
      {
        Decoder decoder = new Decoder(new DecoderSettings(baseAddress));
        DisassemblyMap map;
        
        Stopwatch sw = new Stopwatch();        
        sw.Start();

        using (FileStream fs = new FileStream(file, FileMode.Open))
          map = decoder.Decode(fs);

        sw.Stop();
        Console.WriteLine($"Initial decoding performed in {sw.ElapsedMilliseconds}ms.");

        DoDisassembler(decoder, map);

      }
      catch (Exception x)
      {
        Console.WriteLine(x.Message);
      }
    }

    // Load an existing map file
    static void LoadMap(string mapfile)
    {
      try
      {
        DisassemblyMap map;
        using (FileStream fs = new FileStream(mapfile, FileMode.Open))
          map = new DisassemblyMap(fs);

        Decoder decoder = new Decoder(new DecoderSettings(map.StartAddress));
        DoDisassembler(decoder, map);
      }
      catch (Exception x)
      {
        Console.WriteLine(x.Message);
      }
    }

    // Disassembler main-loop
    static void DoDisassembler(Decoder decoder, DisassemblyMap map)
    {
      while (true)
      {
        Help.Prompt();
        string? cmd = Console.ReadLine();
        if (cmd == null)
          continue;

        bool displayTime = true;
        Stopwatch sw = new Stopwatch();
        sw.Restart();

        if (cmd == "?")
        {
          Help.ShowHelp();
          displayTime = false;
        }
        else if (cmd == "c")
        {
          Console.Clear();
          displayTime = false;
        }
        else if (cmd == "x")
        {
          displayTime = false;
          break;
        }
        else if (cmd == "p")
        {
          Console.Clear();
          PrintMap(map);
          displayTime = false;
        }
        else if (cmd.StartsWith("sm"))
        {
          try
          {
            string[] arg = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string name = arg[1];
            using (var fs = new FileStream(@name, FileMode.Create))
              map.SaveTo(fs);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("d "))
        {
          try
          {
            var region = ParseRegion(cmd);
            decoder.Decode(map, region.Start, region.End);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("db "))
        {
          try
          {
            var region = ParseRegionSize(cmd);
            decoder.SetDcbRange(map, region.Start, region.End, region.Size);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("dw "))
        {
          try
          {
            var region = ParseRegionSize(cmd);
            decoder.SetDcwRange(map, region.Start, region.End, region.Size);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("dl "))
        {
          try
          {
            var region = ParseRegionSize(cmd);
            decoder.SetDclRange(map, region.Start, region.End, region.Size);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("ds"))
        {
          try
          {
            var region = ParseRegion(cmd);
            decoder.SetDcbsRange(map, region.Start, region.End);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("dbb"))
        {
          try
          {
            var region = ParseRegion(cmd);
            decoder.SetDcbbRange(map, region.Start, region.End);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("dbw"))
        {
          try
          {
            var region = ParseRegion(cmd);
            decoder.SetDcbwRange(map, region.Start, region.End);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("dbl"))
        {
          try
          {
            var region = ParseRegion(cmd);
            decoder.SetDcblRange(map, region.Start, region.End);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else if (cmd.StartsWith("rn "))
        {
          try
          {
            string[] arg = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            long address = long.Parse(arg[1], NumberStyles.HexNumber);
            string name = arg[2];
            decoder.RenameLabel(map, address, name);
          }
          catch (Exception x)
          {
            Console.WriteLine($"Error: {x.Message}");
          }
        }
        else
        {
          Console.WriteLine("?");
          displayTime = false;
        }

        sw.Stop();
        if (displayTime == true)
          Console.WriteLine($"Done. Elapsed time = {sw.ElapsedMilliseconds}ms.");
      }
    }

    // Parse region data
    private static (long Start, long End) ParseRegion(string cmd)
    {
      try
      {
        string[] arg = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        long start = long.Parse(arg[1], NumberStyles.HexNumber);
        long end = long.Parse(arg[2], NumberStyles.HexNumber);
        return (start, end);
      }
      catch
      {
        throw new Exception("Cannot determine start and/or end address.");
      }
    }

    // Parse region data and size
    private static (long Start, long End, int Size) ParseRegionSize(string cmd)
    {
      try
      {
        var region = ParseRegion(cmd);
        string[] arg = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if(arg.Length<=3)
          return (region.Start, region.End, 1);
        else
          return (region.Start, region.End, int.Parse(arg[3]));
      }
      catch
      {
        throw new Exception("Cannot determine group size.");
      }
    }

    // Renders the disassembly map into human readable format.
    private static void PrintMap(DisassemblyMap map)
    {
      RenderingSettings rs = new RenderingSettings(useHex: true, characterCasing: RenderingSettings.InstructionCase.Lower);
      foreach (var item in map.Data.Select(x => x.Value).OrderBy(x => x.Address))
      {
        if (map.TryGetLabel(item.Address, out AddressLabel? label) == true)
        {
          if (DoRenderLabel(map, label) == true)
            Console.WriteLine($"{string.Empty,-10}{label.Name}:");
        }

        Console.WriteLine($"${item.Address:X8}: {item.RenderMnemonic(rs),-20}{item.RenderOperands(rs),-40}; {item.RenderHexString(rs),-20} | {item.RenderAscii(rs)}");
      }
    }

    // Render a label only if necessary. This means: if the address is referenced by anything and if it is a displacement-label.
    private static bool DoRenderLabel(DisassemblyMap map, AddressLabel label)
    {
      if (label.InstructionAddresses.Count > 0)
        return true;

      if (map.Labels.Any(x => x.Value.DisplacementLabel == label) == true)
        return true;

      return false;
    }
  }
}
