### M68k Decoder
M68k Decoder is a disassembler / decoder for the [Motorola (now NXP) 68000](https://www.nxp.com/products/processors-and-microcontrollers/legacy-mpu-mcus/32-bit-coldfire-mcus-mpus/68k-processors-legacy/m680x0/low-cost-32-bit-microprocessor-including-hc000-hc001-ec000-and-sec000:MC68000) family of processors.
It currently only handles the M68000 instruction set. But it is intended to include M68020, M68030, M68040, M68060 specific instructions and addressing modes as well as other related hardware like MMUs in the future.

The decoder is written 100% in C# targeting the .NET Standard 2.1 framework.

### Source
This repository contains a Visual Studio 2022 solution file with two projects: `Dimjaku.M68k.Decoder` and `Dimjaku.M68k.Decoder.Test`.

##### Dimjaku.M68k.Decoder
This is the main source that gives you the assembly `Dimakaju.M68k.dll`. This is what you want to reference if you want to use this piece of software in your own projects.

##### Dimjaku.M68k.Decoder.Test
This is a small disassembler I wrote to demonstrate the use of the assembly. Everything the dll is capable of can be found here. This utility currently targets the `.NET 6.0` framework.

### Usage
Using this project is pretty straightforward. Below you will find short examples for getting started. For a deeper dive into what m68k-decoder can do, take a look at the sample project that is part of this repository (`Dimjaku.M68k.Decoder.Test`).

##### Disassembling a binary file
```csharp
public static void Disassemble()
{
  // Create a decoder-setting object setting the start address of the binary to $50000.
  var settings = new DecoderSettings(0x50000);

  // Create a decoder instance
  var decoder = new Decoder(settings);

  // Pass a stream to the decoder for decoding. Note that the entire stream is decoded at once.
  // Streams passed to it must be deterministic and seekable (for example, a file stream).
  DisassemblyMap map;
  using (FileStream fs = new FileStream("m68k-binary.bin", FileMode.Open))
    map = decoder.Decode(fs);

  // The created map file can now be used to print disassembly information, let's do that:
  // First, let's create rendering-settings ...
  var rs = new RenderingSettings(useHex: true, characterCasing: RenderingSettings.InstructionCase.Lower);

  // ... then use them to render the output
  foreach (var data in map.Data.Values.OrderBy(x => x.Address))
    Console.WriteLine($"{data.RenderMnemonic(rs),-20} {data.RenderOperands(rs)}");

  // This produces output like (of course depending on what you've put in):
  // move.l    d0,(a0)+
  // lea       20(a0),d1
  // moveq     #0,d0
  // etc.  
}
```

##### Loading and saving a disassembly map
The disassembly map (class `DisassemblyMap`) holds the decoded data and can be used to further inspect or change regions within the binary file. It also can be saved to a stream and loaded from a stream.

Let's first save a map object to a file ...
```csharp
public static void SaveMap(DisassemblyMap map, string filename)
{
  using (var fs = new FileStream(filename, FileMode.Create))
    map.SaveTo(fs);

  // ...
}
```

... this file can then be loaded again
```csharp
public static void LoadMap(string file)
{
  DisassemblyMap map;
  using (FileStream fs = new FileStream(file, FileMode.Open))
    map = new DisassemblyMap(fs);

  Decoder decoder = new Decoder(new DecoderSettings(map.StartAddress));

  // ...
}
```
