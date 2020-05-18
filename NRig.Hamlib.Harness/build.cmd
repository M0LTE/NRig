@echo off
rem pushd C:\Users\tomandels\Source\Repos\NRig\NRig.Cli
dotnet publish -r linux-arm -nologo -consoleLoggerParameters:NoSummary -verbosity:quiet -o ./publish/linux-arm
dotnet publish -r win-x64 -nologo -consoleLoggerParameters:NoSummary -verbosity:quiet -o ./publish/win-x64
rem scp bin\debug\netcoreapp3.1\linux-arm\publish\nrig* pi@nettest-pi:/home/pi/nrig-cli/
rem popd
