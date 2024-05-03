#!/bin/bash
set -e

MOD_DLL="techtonica_vr"
GAME_PATH="/c/Program Files (x86)/Steam/steamapps/common/Techtonica"

# Copy the vr dlls to the libs folder
echo "Copying the vr dlls to the libs folder..."
rm -rf libs/Managed
rm -rf libs/StreamingAssets
rm -rf libs/UnitySubsystems
mkdir -p libs/Managed
cp -r unity/build/unity_Data/StreamingAssets libs
cp -r unity/build/unity_Data/UnitySubsystems libs
cp -r unity/build/unity_Data/Managed/{SteamVR*.dll,Unity.XR.*.dll,UnityEngine.XR*.dll,UnityEngine.VR*.dll,Valve*.dll,UnityEngine.SubsystemsModule.dll,UnityEngine.AssetBundleModule.dll} libs/Managed

# Build the project
echo "Building the project..."
cd plugin
dotnet build
cd ..

# Cleanup
echo "Cleaning up..."
rm -rf "$GAME_PATH/BepInEx/plugins/$MOD_DLL"
mkdir -p "$GAME_PATH/BepInEx/plugins/$MOD_DLL/assets"
mkdir -p "$GAME_PATH/BepInEx/plugins/$MOD_DLL/vr_config"

# Copy the mod dll to the mods folder
echo "Copying the mod dll to the mods folder..."
cp ./plugin/bin/Debug/netstandard2.1/techtonica_vr.* "$GAME_PATH/BepInEx/plugins/$MOD_DLL"

# Copy the assets to the game folder
echo "Copying the assets to the game folder..."
cp -r ./unity/AssetBundles/StandaloneWindows/* "$GAME_PATH/BepInEx/plugins/$MOD_DLL/assets"
cp -r ./libs/* "$GAME_PATH/BepInEx/plugins/$MOD_DLL/vr_config"
