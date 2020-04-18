# NRig
.NET Standard library for controlling common ham radio rigs directly via CAT. Currently early days but contributions welcome via PRs.

Published to https://www.nuget.org/packages/NRig automatically via GitHub actions.

# Support matrix

| Feature           | FT817/818/857/897/450d | TS2000 | any other |
| ----------------- | ---------------------- | ------ | --------- |
| Find radio        | yes                    | no     | no        |
| Get/set frequency | yes                    | no     | no        |
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
