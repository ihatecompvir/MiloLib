# MiloLib

MiloLib is a .NET 10 library for manipulating Milo scenes used by Harmonix games. It supports scenes from Rock Band, Guitar Hero, Dance Central, and other Milo engine games. Support depends on the asset type, revision, and platform, as not all games and platforms are supported as of yet. Your mileage may vary.

This is not a standalone program, it is a library intended to be used to build programs on top of.

## Use in another project

Add a project reference to `MiloLib/MiloLib.csproj`. Then you should be able to use the library.

Open and save an archive:

```csharp
using MiloLib;

MiloFile milo = new MiloFile("input.milo_xbox");
// Read or edit assets through milo.dirMeta.
milo.Save("edited.milo_xbox", milo.compressionType);
```