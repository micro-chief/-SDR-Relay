# SKYNET SDR Relay

Minimal Steam Datagram Relay test service for Deadlock / SKYNET.

This project provides a lightweight UDP SDR relay endpoint used by the SKYNET Steam Emulator while experimenting with Deadlock networking.

> **Status:** experimental.
>
> The relay currently implements the minimum SDR RouterPing request/reply flow required for relay discovery and latency testing. It is not a full implementation of Valve's Steam Datagram Relay network.

---

## Requirements

- Windows 10 / 11 or Windows Server
- .NET 8 SDK or .NET 8 Runtime
- An open UDP port
- SKYNET Steam Emulator configured to advertise this relay

Default relay port:

```text
28009/UDP
```

---

## Repository structure

```text
-SDR-Relay/
├── Program.cs
├── SdrRelayService.cs
├── SDR-Relay.csproj
└── Generated/
```

`Program.cs` starts the .NET Worker Service.

`SdrRelayService.cs` contains the UDP SDR relay implementation.

`Generated/` contains generated / protobuf-related code used by the relay.

---

## Build

Clone the repository:

```powershell
git clone https://github.com/micro-chief/-SDR-Relay.git
cd -SDR-Relay
```

Restore and build:

```powershell
dotnet restore
dotnet build -c Release
```

The compiled application will be created under:

```text
bin\Release\net8.0\
```

---

## Run

### Development

From the repository directory:

```powershell
dotnet run
```

The relay listens by default on:

```text
0.0.0.0:28009/UDP
```

A successful startup should print something similar to:

```text
SDR relay listening on UDP 0.0.0.0:28009
```

Stop the relay with:

```text
Ctrl+C
```

### Run the compiled build

Build first:

```powershell
dotnet build -c Release
```

Then run:

```powershell
.\bin\Release\net8.0\SDR-Relay.exe
```

---

## Configure the UDP port

The configuration key is:

```text
Sdr:RelayPort
```

Default:

```text
28009
```

Override it from the command line:

```powershell
dotnet run -- --Sdr:RelayPort=28010
```

Or for the compiled executable:

```powershell
.\SDR-Relay.exe --Sdr:RelayPort=28010
```

You can also use a .NET environment variable:

```powershell
$env:Sdr__RelayPort = "28010"
dotnet run
```

---

## Firewall

The relay uses UDP.

For the default configuration, allow inbound traffic on:

```text
UDP 28009
```

Example Windows Firewall rule:

```powershell
New-NetFirewallRule `
    -DisplayName "SKYNET SDR Relay" `
    -Direction Inbound `
    -Protocol UDP `
    -LocalPort 28009 `
    -Action Allow
```

If the relay is hosted behind a router / NAT and must be reachable from another network, forward the same UDP port to the machine running the relay.

---

## SKYNET configuration

The SKYNET Steam Emulator can advertise custom SDR POPs / relays through its local SDR configuration.

Example:

```ini
[sky]
address=192.168.0.101
port=28009
```

The exact `sdr-relays.ini` format may change while the SKYNET SDR configuration layer is under development.

The important values are:

```text
Relay IP   = address of the machine running SDR-Relay
Relay Port = UDP port used by SDR-Relay
```

For a relay running on another PC in the LAN, do not use:

```text
127.0.0.1
```

Use that computer's LAN address instead, for example:

```text
192.168.0.101
```

---

## Testing from Deadlock

Start the relay first:

```powershell
dotnet run
```

Then start SKYNET and Deadlock.

Inside the Deadlock console, use:

```text
net_print_sdr_ping_times
```

A working local relay should appear in the SDR POP list with a measured RTT.

Example:

```text
SDR relay network status: OK
Measured RTT to SDR points of presence.

sky: 1ms
```

The exact latency depends on where the relay is running.

---

## How it works

The relay opens an IPv4 UDP socket and listens for SDR datagrams.

Currently the implemented flow is:

```text
Deadlock / SteamNetworkingSockets
            |
            | UDP RouterPing
            v
      SKYNET SDR Relay
            |
            | RouterPingReply
            v
Deadlock / SteamNetworkingSockets
```

The service currently handles the SDR router ping message used for relay discovery / latency measurement.

The reply contains data such as:

```text
client timestamp
client cookie
observed public IP
observed public UDP port
server time
challenge
```

Unknown datagram message types are currently ignored / logged.

---

## Multiple relays

Multiple relay instances can be started by assigning different UDP ports.

Terminal 1:

```powershell
dotnet run -- --Sdr:RelayPort=28009
```

Terminal 2:

```powershell
dotnet run -- --Sdr:RelayPort=28010
```

Terminal 3:

```powershell
dotnet run -- --Sdr:RelayPort=28011
```

Each endpoint can then be advertised as a separate relay in the SKYNET SDR configuration.

For relays on separate machines, the same UDP port can normally be used on every machine.

---

## Troubleshooting

### `Address already in use`

Another application is already using the configured UDP port.

Use another port:

```powershell
dotnet run -- --Sdr:RelayPort=28010
```

### Relay does not appear in `net_print_sdr_ping_times`

Check:

1. SDR-Relay is running.
2. The configured relay IP is correct.
3. The configured UDP port matches the relay.
4. Windows Firewall allows the UDP port.
5. The client can reach the relay host.
6. SKYNET is serving the expected SDR configuration.

### Works on localhost but not from another PC

Do not advertise:

```text
127.0.0.1
```

Advertise the LAN / server IP instead:

```text
192.168.x.x
```

Also verify firewall and NAT configuration.

---

## Current implementation status

Implemented:

- .NET 8 Worker Service
- IPv4 UDP listener
- Configurable relay UDP port
- SDR RouterPing request handling
- SDR RouterPing reply generation
- Client IP / port reflection
- Basic logging

Planned:

- Additional SDR packet types
- More complete Steam Datagram Relay protocol handling
- Better diagnostics
- Multi-POP configuration
- Integration with SKYNET Game Coordinator
- Dedicated Deadlock game-server routing
- Automated relay configuration generation

---

## Related projects

### SKYNET Steam Emulator

```text
https://github.com/micro-chief/-SKYNET-Steam-Emulator
```

### SDR Relay

```text
https://github.com/micro-chief/-SDR-Relay
```


## Disclaimer

This repository is an experimental compatibility / research project.

It is not affiliated with, endorsed by, or sponsored by Valve Corporation.

Steam, Steam Datagram Relay, SteamNetworkingSockets and Deadlock are trademarks or properties of their respective owners.
