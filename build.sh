#!/bin/bash

# Copy the vr dlls to the libs folder
echo "Copying the vr dlls to the libs folder..."
rm -rf libs
mkdir -p libs/Managed
cp -r unity/build/unity_Data/Plugins libs
cp -r unity/build/unity_Data/StreamingAssets libs
cp -r unity/build/unity_Data/UnitySubsystems libs
cp -r unity/build/unity_Data/Managed/{SteamVR*.dll,Unity.XR.*.dll,UnityEngine.XR*.dll,UnityEngine.VR*.dll,Valve*.dll,UnityEngine.SubsystemsModule.dll,UnityEngine.AssetBundleModule.dll} libs/Managed

# Build the project
echo "Building the project..."
cd plugin
dotnet build
cd ..

# Copy the mod dll to the mods folder
echo "Copying the mod dll to the mods folder..."
cp ./plugin/bin/Debug/netstandard2.1/techtonica_vr.dll /c/Program\ Files\ \(x86\)/Steam/steamapps/common/Techtonica/BepInEx/plugins/techtonica_vr.dll

# Copy the vr dlls to the game folder
echo "Copying the vr dlls to the game folder..."
cp -r ./libs/* /c/Program\ Files\ \(x86\)/Steam/steamapps/common/Techtonica/Techtonica_Data/

# Copy the assets to the game folder
echo "Copying the assets to the game folder..."
rm -rf /c/Program\ Files\ \(x86\)/Steam/steamapps/common/Techtonica/BepInEx/plugins/techtonica_vr/assets
mkdir -p /c/Program\ Files\ \(x86\)/Steam/steamapps/common/Techtonica/BepInEx/plugins/techtonica_vr/assets
cp -r ./unity/AssetBundles/StandaloneWindows/* /c/Program\ Files\ \(x86\)/Steam/steamapps/common/Techtonica/BepInEx/plugins/techtonica_vr/assets

echo "Done!"
read -p "Press enter to continue"