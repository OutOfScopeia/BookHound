# start from prebuilt emulator image
FROM budtmo/docker-android:latest

# install dotnet SDK
USER root
RUN apt-get update
RUN apt-get upgrade -y
RUN apt-get install -y dotnet-sdk-10.0

ENV USER=androidusr
ENV USERNAME=androidusr

# RUN mkdir -p /opt/android

RUN chown -R androidusr:androidusr /opt/android
# RUN sudo groupadd -r kvm
# RUN sudo gpasswd -a $USER kvm
# RUN sudo usermod -aG kvm androidusr


# RUN chmod -R u+rwx /opt/android
# RUN chown -R androidusr:androidusr /workspaces/BookHoundApp

# Switch back to non-root if your emulator image uses one
USER androidusr
WORKDIR /home/androidusr


