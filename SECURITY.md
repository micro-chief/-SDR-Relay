# Security Policy

## Supported Versions

SDR-Relay is an experimental compatibility and research project. Security fixes
are applied only to the latest revision of the `master` branch and, when
applicable, the latest published build artifacts.

| Version | Supported |
| --- | --- |
| Latest `master` revision | Yes |
| Latest successful build artifacts | Yes |
| Older commits, forks and artifacts | No |

Before reporting a vulnerability, please confirm that it is reproducible with
the latest revision.

## Reporting a Vulnerability

Please do not report suspected security vulnerabilities through public GitHub
Issues, Discussions, pull requests or comments.

Use GitHub's private vulnerability reporting feature:

1. Open the repository's **Security** tab.
2. Select **Advisories**.
3. Select **Report a vulnerability**.
4. Submit the report privately.

Include as much of the following information as possible:

- a clear description of the vulnerability and its potential impact;
- the affected commit, artifact, platform and runtime identifier;
- the network configuration and deployment assumptions;
- complete reproduction steps;
- a minimal proof of concept, when safe to provide;
- relevant logs with credentials, tokens, addresses and personal data removed;
- any known mitigation or suggested fix.

We aim to acknowledge a complete report within seven days. Investigation and
resolution times depend on the complexity and impact of the issue. Please allow
time for a fix before making vulnerability details public.

## Security-Relevant Reports

Examples of relevant reports include:

- remote code execution or unintended command execution;
- denial of service caused by malformed or unauthenticated datagrams;
- excessive memory, CPU, socket or storage consumption;
- unsafe packet parsing, integer overflows or buffer handling;
- unintended disclosure of network, configuration or user information;
- dependency, build pipeline or artifact integrity issues;
- exposed credentials, tokens or other secrets.

Normal experimental limitations, unsupported SDR message types and general
compatibility problems should be reported through a regular GitHub Issue unless
they create a security impact.

## Safe Testing

Security testing must be limited to systems and network endpoints that you own
or are explicitly authorized to test.

Do not:

- direct testing traffic at Valve Corporation or other third-party systems;
- access, modify or disrupt data belonging to other users;
- perform denial-of-service testing against public infrastructure;
- include credentials, tokens, private addresses or personal data in reports;
- use a vulnerability beyond what is necessary to demonstrate its impact.

This policy does not grant authorization to test any third-party application,
service, account, network or infrastructure.

## Third-Party Components

This repository uses third-party dependencies, protocol names and generated
definitions. Those materials remain subject to the rights and policies of their
respective owners.

If a vulnerability affects a third-party component independently of SDR-Relay,
report it to that component's maintainer. You may also notify this repository
privately when the issue directly affects an SDR-Relay deployment.

## Disclosure and Rewards

Please coordinate public disclosure through the private vulnerability report.
Confirmed issues may be documented after a fix or mitigation is available.

This project does not currently operate a bug bounty program and cannot offer
monetary rewards for vulnerability reports.
