#!/bin/dash
set -e

apt-get update
apt-get install -y openjdk-17-jdk unzip curl

# Download cmdline-tools
curl -L -o /tmp/cmdline-tools.zip \
  https://dl.google.com/android/repository/commandlinetools-linux-13114758_latest.zip

mkdir -p /opt/android-sdk
export ANDROID_SDK_ROOT=/opt/android-sdk
export PATH=$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin:$ANDROID_SDK_ROOT/platform-tools

mkdir -p $ANDROID_SDK_ROOT/cmdline-tools
unzip /tmp/cmdline-tools.zip -d /tmp
mv /tmp/cmdline-tools $ANDROID_SDK_ROOT/cmdline-tools/latest

# export PATH=$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin

yes | sdkmanager --sdk_root=$ANDROID_SDK_ROOT "platform-tools"
yes | sdkmanager --sdk_root=$ANDROID_SDK_ROOT "platforms;android-36"
yes | sdkmanager --sdk_root=$ANDROID_SDK_ROOT "build-tools;36.0.0"

echo 'Android + Java SDK installed.'
dotnet workload restore BookHoundApp/BookHoundApp.sln
echo 'Dotnet workloads restored'
sleep infinity
