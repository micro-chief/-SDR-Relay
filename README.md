# SKYNET SDR Relay

Minimal, cross-platform Steam Datagram Relay compatibility service for SKYNET
and AppID `1422450`.

> [!IMPORTANT]
> This project is experimental. It currently implements only the SDR
> `RouterPing` request/reply flow used for relay discovery and latency
> measurement. It does not yet forward application traffic and is not a full
> implementation of Valve's Steam Datagram Relay network.

## Supported platforms

| Runtime identifier | Target |
| --- | --- |
| `win-x64` | Windows x86-64 |
| `linux-x64` | Linux x86-64 |
| `linux-arm64` | 64-bit ARM Linux, including Raspberry Pi with a 64-bit OS |
| `linux-arm` | 32-bit ARM Linux where .NET 8 is supported |

For Raspberry Pi and other new ARM deployments, use a 64-bit operating system
and the `linux-arm64` build. The ARM targets are produced automatically, but
validation on physical ARM hardware is still in progress.

The service is managed .NET code and does not use architecture-specific native
libraries. Framework-dependent builds use `AnyCPU`; self-contained builds
include the runtime and native launcher for the selected target.

## Current capabilities

- .NET 8 Worker Service
- IPv4 UDP listener
- configurable UDP port and POP values
- SDR `RouterPing` request handling
- SDR `RouterPingReply` generation
- remote IPv4 address and UDP port reflection
- architecture-independent IPv4 byte-order handling
- self-contained builds for Windows x64, Linux x64, Linux ARM64 and Linux ARM
- automatic GitHub Actions artifacts for all supported runtime identifiers

## Requirements

- .NET 8 SDK when building from source, or no preinstalled runtime when using a
  self-contained artifact
- an open UDP port; the default is `28009/UDP`
- SKYNET configured to advertise the relay endpoint

## Repository structure

```text
-SDR-Relay/
├── .github/workflows/publish.yml
├── Generated/
├── Program.cs
├── SdrRelayService.cs
├── SDR-Relay.csproj
├── appsettings.json
└── sdr-relays.ini
```

- `Program.cs` starts the .NET Worker Service.
- `SdrRelayService.cs` contains the UDP SDR implementation.
- `Generated/` contains protobuf-generated C# types.
- `appsettings.json` configures the relay process.
- `sdr-relays.ini` is an example of the configuration consumed by SKYNET.

## Download a ready-to-run build

1. Open the repository on GitHub.
2. Select **Actions**.
3. Open the latest successful **Publish** workflow run.
4. Find **Artifacts** at the bottom of the run page.
5. Download the artifact matching the target platform.

Available artifact names:

```text
SDR-Relay-win-x64
SDR-Relay-linux-x64
SDR-Relay-linux-arm64
SDR-Relay-linux-arm
```

GitHub downloads the artifact as a ZIP archive. The ZIP contains a
`SDR-Relay-<runtime-id>.tar.gz` archive. Extract both layers before running the
service.

## Build from source

```bash
git clone https://github.com/micro-chief/-SDR-Relay.git
cd -SDR-Relay
dotnet restore
dotnet build --configuration Release
```

The framework-dependent build is written to:

```text
bin/Release/net8.0/
```

## Publish for a target platform

### Raspberry Pi or another 64-bit ARM Linux host

```bash
dotnet publish --configuration Release --runtime linux-arm64 --self-contained true
```

### 32-bit ARM Linux

```bash
dotnet publish --configuration Release --runtime linux-arm --self-contained true
```

### Linux x86-64

```bash
dotnet publish --configuration Release --runtime linux-x64 --self-contained true
```

### Windows x86-64

```powershell
dotnet publish --configuration Release --runtime win-x64 --self-contained true
```

Published files are written to:

```text
bin/Release/net8.0/<runtime-id>/publish/
```

## Run

### Development

```bash
dotnet run
```

### Linux ARM64

From the extracted `linux-arm64` artifact or publish directory:

```bash
chmod +x SDR-Relay
./SDR-Relay
```

### Linux ARM32 or Linux x86-64

Use the same commands with the artifact matching the target architecture:

```bash
chmod +x SDR-Relay
./SDR-Relay
```

### Windows x64

```powershell
.\SDR-Relay.exe
```

The default listen endpoint is:

```text
0.0.0.0:28009/UDP
```

Stop the process with `Ctrl+C`.

## Relay process configuration

The service reads standard .NET configuration from `appsettings.json`, command
line arguments and environment variables.

| Key | Default | Description |
| --- | --- | --- |
| `Sdr:RelayPort` | `28009` | UDP listen port |
| `Sdr:PopId` | `sk2` | POP code for this instance |
| `Sdr:PeerPopId` | `sky` | peer POP returned in ping replies |
| `Sdr:PeerPingMs` | `1` | advertised latency to the peer POP |

Command-line example:

```bash
./SDR-Relay --Sdr:RelayPort=28010 --Sdr:PopId=arm
```

Linux environment variables:

```bash
export Sdr__RelayPort=28010
export Sdr__PopId=arm
./SDR-Relay
```

Windows PowerShell environment variables:

```powershell
$env:Sdr__RelayPort = "28010"
$env:Sdr__PopId = "arm"
.\SDR-Relay.exe
```

## SKYNET configuration

SKYNET can advertise custom SDR POPs and endpoints using `sdr-relays.ini`.

Example:

```ini
[SDR]
Revision=1786739253

[Relay.sky]
Address=192.168.0.101
Port=28009
Description=SKYNET Primary
Longitude=4.90
Latitude=52.37
Partners=1
Tier=0

[Relay.arm]
Address=192.168.0.23
Port=28009
Description=SKYNET ARM64 Relay
Longitude=4.91
Latitude=52.38
Partners=1
Tier=1

[TypicalPing.sky-arm]
From=sky
To=arm
Ping=1
```

Use an address reachable by the client. Do not advertise `127.0.0.1` for a
relay running on another host.

SKYNET already reads `sdr-relays.ini`, but automatic generation of this file is
not implemented yet. The example file must currently be created or updated
manually.

## Network topology requirement

The relay and the primary server endpoint must not share the same effective
network interface or packet path. SDR-aware clients observe the route, and
collapsing both roles onto the same path can produce incorrect behaviour.

Deploy the relay as an independent endpoint, for example:

```text
Client -> Raspberry Pi ARM64 relay -> primary server endpoint
```

Running multiple processes on one interface with different UDP ports can be
useful for local protocol development, but it does not reproduce independent
relay paths.

## Firewall

Allow inbound UDP traffic on the configured relay port.

### Debian, Ubuntu or Raspberry Pi OS with UFW

```bash
sudo ufw allow 28009/udp
```

### Windows PowerShell

```powershell
New-NetFirewallRule `
    -DisplayName "SKYNET SDR Relay" `
    -Direction Inbound `
    -Protocol UDP `
    -LocalPort 28009 `
    -Action Allow
```

If the endpoint is behind NAT and must be reachable from another network,
forward the same UDP port to the relay host.

## Test with AppID 1422450

1. Start SDR-Relay.
2. Start SKYNET.
3. Start AppID `1422450`.
4. Run the following command in the application console:

```text
net_print_sdr_ping_times
```

A reachable endpoint should appear in the SDR POP list with a measured RTT.

## Troubleshooting

### `Address already in use`

Another process is already using the configured UDP port. Select another port:

```bash
./SDR-Relay --Sdr:RelayPort=28010
```

### The relay does not appear in the SDR POP list

Check that:

1. SDR-Relay is running.
2. The configured address is reachable from the client.
3. The configured UDP port matches the process configuration.
4. The host firewall allows inbound UDP traffic.
5. NAT or port forwarding is configured when required.
6. SKYNET is serving the expected SDR configuration.

### The endpoint works locally but not from another machine

Do not advertise `127.0.0.1`. Use the relay host's LAN or public address and
verify the firewall and NAT configuration.

## Planned work

- additional SDR packet types
- application-traffic forwarding
- more complete Steam Datagram Relay protocol handling
- multi-POP configuration and diagnostics
- SKYNET GC integration
- automatic `sdr-relays.ini` generation
- validation on physical ARM64 and ARM32 hardware

## Related projects

- [SKYNET Steam Emulator](https://github.com/Hackerprod/-SKYNET-Steam-Emulator)
- [SDR Relay](https://github.com/micro-chief/-SDR-Relay)

## Disclaimer

This repository is an experimental compatibility and research project. It is
not affiliated with, endorsed by, or sponsored by Valve Corporation. Steam,
Steam Datagram Relay and SteamNetworkingSockets are trademarks or properties of
their respective owners.
