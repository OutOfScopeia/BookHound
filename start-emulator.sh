#!/usr/bin/env bash
export ANDROID_SDK_ROOT="/opt/android-sdk"
export PATH="$ANDROID_SDK_ROOT/emulator:$ANDROID_SDK_ROOT/platform-tools:$PATH"
export DISPLAY=:1

echo "Starting emulator script" >> /tmp/emulator.log
sleep 5
AVD_NAME="test"
echo "Creating AVD $AVD_NAME" >> /tmp/emulator.log
avdmanager create avd -n $AVD_NAME -k "system-images;android-36;google_apis;x86_64" --force --device "pixel_9_pro" >> /tmp/emulator.log 2>&1
echo "Starting emulator" >> /tmp/emulator.log
emulator -avd $AVD_NAME -gpu host -no-snapshot -no-audio -accel on >> /tmp/emulator.log 2>&1 &