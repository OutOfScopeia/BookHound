FROM mcr.microsoft.com/dotnet/sdk:10.0.101-noble-amd64
# Non-interactive apt
ENV DEBIAN_FRONTEND=noninteractive
# Android SDK root
ENV ANDROID_SDK_ROOT=/opt/android-sdk
ENV PATH=$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin:$ANDROID_SDK_ROOT/platform-tools:$ANDROID_SDK_ROOT/emulator
# Install dependencies for Android SDK
RUN apt-get update && apt-get install -y openjdk-17-jdk unzip curl
# Download cmdline-tools
RUN curl -L -o /tmp/cmdline-tools.zip https://dl.google.com/android/repository/commandlinetools-linux-13114758_latest.zip
RUN mkdir -p $ANDROID_SDK_ROOT/cmdline-tools
RUN unzip /tmp/cmdline-tools.zip -d /tmp
RUN mkdir -p /opt/android-sdk/cmdline-tools/latest \
&& cp -a /tmp/cmdline-tools/. /opt/android-sdk/cmdline-tools/latest/ \
&& rm -rf /tmp/cmdline-tools
RUN yes | sdkmanager --sdk_root=$ANDROID_SDK_ROOT "platform-tools"
RUN yes | sdkmanager --sdk_root=$ANDROID_SDK_ROOT "platforms;android-36"
RUN yes | sdkmanager --sdk_root=$ANDROID_SDK_ROOT "build-tools;36.0.0"
RUN yes | sdkmanager --sdk_root=$ANDROID_SDK_ROOT "system-images;android-36;google_apis;x86_64"
# VNC
ENV USER=root
ENV HOME=/root
ENV LANG=en_US.UTF-8
ENV LC_ALL=en_US.UTF-8
RUN apt-get install -y locales xfce4 xfce4-goodies libx11-xcb1 dbus-x11 x11-apps xterm tigervnc-standalone-server novnc websockify 
# Distro-related fix for one android emulator warning after installing libx11-xcb1
RUN ln -s /usr/lib/x86_64-linux-gnu/libX11-xcb.so.1 /usr/lib/x86_64-linux-gnu/libX11-xcb.so || true
# "Debug" only
# RUN apt-get install nano -y
RUN apt-get clean
RUN locale-gen en_US.UTF-8 && update-locale LANG=en_US.UTF-8
# set xfce as default session
RUN echo "xfce4-session" > ~/.xsession
# Create VNC password non-interactively
RUN mkdir -p /root/.vnc && \
echo "dummy" | vncpasswd -f > /root/.vnc/passwd && \
chmod 600 /root/.vnc/passwd
# VNC xstartup cfg
COPY xstartup /root/.vnc/xstartup
RUN chmod +x /root/.vnc/xstartup
# Autostart script for the emulator when XFCE starts
COPY start-emulator.sh /usr/local/bin/start-emulator.sh
RUN chmod +x /usr/local/bin/start-emulator.sh
COPY android-emulator.desktop ~/.config/autostart/android-emulator.desktop
# Container startup script
COPY runVNC.sh /usr/local/bin/runVNC.sh
RUN chmod +x /usr/local/bin/runVNC.sh
CMD ["sh", "-c", "/usr/local/bin/runVNC.sh"]