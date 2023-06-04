#!/usr/bin/env sh

# breaks exectuion of scenario if error is encountered
set -e

# prints commands as they are executed
# set -x 

# TODO: Change path to unity here: 
export UNITY_EDITOR_CUSTOM_PATH="/c/Program Files/Unity/Hub/Editor/2021.3.14f1/Editor"
echo $UNITY_EDITOR_CUSTOM_PATH

unityexedir=/Unity.exe

export UNITY_EXECUTABLE=$UNITY_EDITOR_CUSTOM_PATH$unityexedir
echo "$UNITY_EXECUTABLE"

#C:\Users\user\Documents\UnityInstalls\2020.3.16f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools
unityadbexedir=/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe

# if using path with spaces - use "$Variable" to escape it properly
export UNITY_ADB="$UNITY_EDITOR_CUSTOM_PATH"$unityadbexedir
echo $UNITY_ADB


"$UNITY_ADB" logcat -s Unity DEBUG