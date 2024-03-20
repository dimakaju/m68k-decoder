using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dimakaju.M68k.Test
{
  internal static class Help
  {
    public static void Prompt()
    {
      Console.WriteLine("Commands = [?, x, p, c, rn, sm, d, db, dl, dw, ds, dbb, dbw, dbl]");
      Console.Write(">");
    }

    public static void ShowHelp()
    {
      Console.WriteLine("? = Show this Help");
      Console.WriteLine("x = Exit disassembler");
      Console.WriteLine("c = Clear screen");
      Console.WriteLine("p = Print disassembly");
      Console.WriteLine("sm [filename] = Saves the project which can be loaded again. Example: sm disassembly.map");
      Console.WriteLine("rn [address] [name] = Renames a label at a specific address. Example: rn 70020 New_Label");
      Console.WriteLine("d [start address] [end address] = Disassemble region. Example: d 70000 70020");
      Console.WriteLine("db [start address] [end address] <size> = Convert region to dc.b with optional group <size> (default=1). Example db 70000 70020 2");
      Console.WriteLine("dw [start address] [end address] <size> = Convert region to dc.w with optional group <size> (default=1). Example dw 70000 70020 2");
      Console.WriteLine("dl [start address] [end address] <size> = Convert region to dc.l with optional group <size> (default=1). Example dl 70000 70020 2");
      Console.WriteLine("ds [start address] [end address] = Convert region to dc.b string literal. Example ds 70000 70020");
      Console.WriteLine("dbb [start address] [end address] = Convert region to dcb.b. Example dbb 70000 70020");
      Console.WriteLine("dbw [start address] [end address] = Convert region to dcb.b. Example dbw 70000 70020");
      Console.WriteLine("dbl [start address] [end address] = Convert region to dcb.b. Example dbl 70000 70020");
    }
  }
}
