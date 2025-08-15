# GKAPI

A modding API for [Gatekeeper](https://store.steampowered.com/app/2106670/Gatekeeper/) (Unity Game by Gravity Lagoon) <br>
The modding framework used is BepInEx, which is required to use this mod and its addons. You can find information
about to install BepInEx [here](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_il2cpp.html)

## Current State

This Project...
- is currently not published on [Nuget](https://www.nuget.org/) so dependencies need the dll. 
  The dll has to be [built from source](#build-from-source) <br>
- uses the stripped [GameLibs](https://www.nuget.org/packages/Gatekeeper.GameLibs.Steam) from nuget

If you still want to try out what is currently possible, [clone the repository](#build-from-source) and
edit the Plugin.cs to your needs. In the Content folder is a simple ItemController which does nothing.

## Use the API
### Build from source

- Clone this repository with ``git clone https://github.com/Robocraft999/GKAPI.git`` 
  or download the [zip](https://github.com/Robocraft999/GKAPI/archive/refs/heads/master.zip)
- Navigate into the directory (if using the zip you have to unzip it first)
- Build with the command ``dotnet build`` (requires .NET SDK 6)
- The built dll is in the folder ``bin/Debug/net6.0/GKAPI.dll``

### Examples

An ExampleMod using the api can be found [here](https://github.com/GatekeeperModding/ExampleMod)

### Make your own Addon

WIP