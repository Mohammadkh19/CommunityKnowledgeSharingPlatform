#!/bin/bash

# Install .NET 8
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0

# Set environment variables for .NET
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH

# Verify installation
dotnet --version

# Publish the .NET app
dotnet publish -c Release -o out
