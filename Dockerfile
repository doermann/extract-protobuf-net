FROM alpine:edge

RUN echo "@testing http://dl-cdn.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories && \
	apk --no-cache add ca-certificates mono@testing wget && \
    update-ca-certificates && \
    cert-sync /etc/ssl/certs/ca-certificates.crt

COPY ./ExtractProto /ExtractProto

RUN wget -O /nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe && \
    mono /nuget.exe restore /ExtractProto/ExtractProto.sln && \
    xbuild /ExtractProto/ExtractProto.sln && \
    mv /ExtractProto/bin/Debug/* / && \
    rm -rf ExtractProto && \
    rm -rf /nuget.exe

COPY ./entrypoint.sh /

ENTRYPOINT ["/entrypoint.sh"]
