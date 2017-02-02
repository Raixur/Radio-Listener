FROM microsoft/dotnet:runtime

RUN echo deb http://ftp.uk.debian.org/debian jessie-backports main \
    >> /etc/apt/sources.list

RUN apt-get update && apt-get install -y \
    ffmpeg \
    streamripper

WORKDIR /app
ENTRYPOINT ["dotnet", "radio-listener.dll"]
COPY . /app
