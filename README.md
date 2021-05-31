# Clam AV API

Simple container that hosts a [Clam AV](https://www.clamav.net/) instance as well as C# API to act as a reverse proxy for the Anti Virus client.

## Reverse Proxy Endpoints

| Request Path | Request Type |                  Response                  | Description                                                                                                                                                           |
|:------------:|:------------:|:------------------------------------------:|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|     /Clam    |      GET     |               Empty Response               | Checks if the ClamAV instance is working by sending it a ping, if successful will respond with an empty response, otherwise will return the error message from ClamAV |
|     /Clam    |     POST     | { VirusDetected: bool, VirusName: string } | Scans the data provided in the body of the request, will respond with whether ClamAV detected any viruses in that file and what the name of that virus is.            |