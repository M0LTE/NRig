# NRig
.NET Standard library for controlling common ham radio rigs directly via CAT. Currently early days but contributions welcome via PRs.

Published to https://www.nuget.org/packages/NRig automatically via GitHub actions.

# Support matrix

| Feature           | FT817/818/857/897/450d | TS2000 | any other |
| ----------------- | ---------------------- | ------ | --------- |
| Find radio        | yes                    | no     | no        |
| Get/set frequency | yes                    | yes    | no        |
| Anything else     | no                     | no     | no        |

# Usage:
TODO

# Roadmap
- use, and get feedback on, the interface presented to the application. Right kind of abstraction?
- get some early adoption
- implement more CAT commands for the existing rigs I own
- implement more rigs natively, not just the ones I own
- implement one of the existing rig abstraction libraries as a catch-all rig for stuff not supported natively yet
- one or more sample applications
- tests

# Contributing
Fork, hack away, send me a PR.
Increment the version number in the .csproj as necessary.

# Other libraries
- Hamlib - Win32, Win64, Linux. Looks good. https://github.com/Hamlib/Hamlib. There's mono bindings too: https://github.com/k5jae/HamLibSharp
- OmniRig - Delphi, 32 bit limitation - max frequency it can handle is 2.147483647 GHz. http://dxatlas.com/OmniRig/Files/OmniRig.zip http://dxatlas.com/OmniRig/Files/RigIni.zip

## Things that aren't libraries
- flrig is a rig control gui that talks directly to rigs. However it has an XML-RPC over HTTP API. A rig implementation for NRig could be built to talk to this.
- rigctld is (I think) a frontend for Hamlib that accepts commands via TCP. A rig implementation for NRig could be built to talk to this, though just consuming Hamlib directly might be simpler.