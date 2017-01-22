FROM ubuntu:latest

ENV DURATION=${DURATION:-300} \
    SERVER=${SERVER:-localhost} \
    PORT=${PORT:-8080}

RUN add-apt-repository ppa:webupd8team/java 
RUN apt-get update && apt-get install -y \
    oracle-java8-installer \
    ant \
    ffmpeg \
    streamripper \
    unzip \
    wget

COPY build/setup build/setup
RUN build/setup

COPY identify/identify identify/identify 
ENTRYPOINT ["identify/identify"]