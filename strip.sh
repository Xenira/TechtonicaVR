#!/bin/bash

# Strip the game
echo "Stripping the game"
rm -rf ./strip-dll
mkdir ./strip-dll
./tools/NStrip.exe -cg -p --cg-exclude-events /c/Program\ Files\ \(x86\)/Steam/steamapps/common/Techtonica/Techtonica_Data/Managed ./strip-dll

echo "Done!"
read -p "Press enter to continue"