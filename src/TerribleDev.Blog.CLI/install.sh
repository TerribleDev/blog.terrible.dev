#!/bin/bash
dotnet restore
dotnet pack /p:Version=1.2.0 /p:Configuration=Release
dotnet tool uninstall --global tempo
dotnet tool install --global --add-source ./nupkg Tempo
