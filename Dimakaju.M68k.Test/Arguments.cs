using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimakaju.M68k.Test
{
  internal static class Arguments
  {
    public static void PrintHelp(bool error)
    {
      if (error)
      {
        Console.WriteLine("Invalid parameters.");
        Console.WriteLine("");
      }

      Console.WriteLine("Simple M68k disassembler (w) Alexander Dimitriadis");
      Console.WriteLine("Usage    : decoder.exe [action] [file] {options}");
      Console.WriteLine("");
      Console.WriteLine("Actions  : new  = Begins a new disassembly project");
      Console.WriteLine("           load = Loads a previously saved disassembly project (.map file)");
      Console.WriteLine("");
      Console.WriteLine("Options  : --base=[address] = Base hex(!) address of binary file when using action \"new\". Defaults to 10000.");
      Console.WriteLine("");
      Console.WriteLine("Examples : decoder.exe new binary-file.bin --base=7F000");
      Console.WriteLine("           decoder.exe load project-file.map");
    }

    public static bool TryParseBaseAddress(string baseoption, out long address)
    {
      var split = baseoption.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
      if (split.Length != 2)
      {
        address = -1;
        return false;
      }

      if (long.TryParse(split[1], System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out address) == true)
        return true;

      address = -1;
      return false;
    }
  }
}
