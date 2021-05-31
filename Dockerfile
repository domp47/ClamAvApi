FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

COPY ClamAvProxy/ /

RUN dotnet publish ClamAvProxy.sln --configuration Release --output ./BuildOut/ --runtime linux-x64


FROM ubuntu:20.04

# Install Clam
RUN apt-get update && apt-get -y install clamav-daemon clamav-freshclam ca-certificates

# permission juggling
RUN mkdir /var/run/clamav && \
    chown clamav:clamav /var/run/clamav && \
    chmod 750 /var/run/clamav

# av configuration update
RUN sed -i 's/^Foreground .*$/Foreground true/g' /etc/clamav/clamd.conf && \
    echo "TCPSocket 3310" >> /etc/clamav/clamd.conf && \
    if [ -n "$HTTPProxyServer" ]; then echo "HTTPProxyServer $HTTPProxyServer" >> /etc/clamav/freshclam.conf; fi && \
    if [ -n "$HTTPProxyPort"   ]; then echo "HTTPProxyPort $HTTPProxyPort" >> /etc/clamav/freshclam.conf; fi && \
    if [ -n "$DatabaseMirror"  ]; then echo "DatabaseMirror $DatabaseMirror" >> /etc/clamav/freshclam.conf; fi && \
    if [ -n "$DatabaseMirror"  ]; then echo "ScriptedUpdates off" >> /etc/clamav/freshclam.conf; fi && \
    sed -i 's/^Foreground .*$/Foreground true/g' /etc/clamav/freshclam.conf


COPY check.sh /
COPY bootstrap.sh /

# Copy from build image to runtime image
COPY --from=build /BuildOut/* /RestProxy/

# port provision
EXPOSE 5000

RUN chown clamav:clamav bootstrap.sh check.sh /etc/clamav /etc/clamav/clamd.conf /etc/clamav/freshclam.conf && \
    chmod u+x bootstrap.sh check.sh

USER clamav

CMD ["/bootstrap.sh"]