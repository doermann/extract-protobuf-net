# extract-protobuf-net

A tool for reverse engineering protobuf binaries created with [protobuf-net](https://github.com/mgravell/protobuf-net).

While protobuf-net includes a basic `GetProto` method on `Serializer`, it only exports the single class and the
dependencies of that class. To bulk export many classes from a protobuf binary requires deduplification of the objects 
as well as fixing some bugs in the export relating to Enum references.

The code should be considered alpha quality. No tests are included. The project has only been tested on a handful of 
Unity based Android apps that use protobuf-net for game communication.

## Usage

Options:
> **-a** the assembly to open. typically a .dll  
> **-p** the proto package name to use when exporting  
> **-t** the list of types to export

If you want to install .net/mono and build from scratch, just use the .sln file or:
```
xbuild /ExtractProto/ExtractProto.sln
mono /ExtractProto/bin/Debug/ExtractProto.exe [options]
```

If you would rather just use docker and not worry about installing dependencies:
```
./extract-proto.sh [options]
```

Typically you will want to pipe the output to a file:
```
./extract-proto.sh [options] > my.proto
```

## Example

```
./extract-proto.sh \
    -a /ProtoOutput.dll \
    -p mypackage \
    -t RequestEnvelope ResponseEnvelope AuthRequest AuthResponse > mypackage.proto
```
