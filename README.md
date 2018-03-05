# dnscrypt-measurement [![License](http://img.shields.io/badge/license-MIT-green.svg?style=flat-square)](https://github.com/bitbeans/dnscrypt-measurement/blob/master/LICENSE.md)

**Deprecated use https://github.com/bitbeans/DnsCrypt.Toolbox instead!**

Simple tool to find the fastest DNSCrypt resolver.
Queries are NOT authenticated (for now)!
## Usage

    dnscrypt-measurement.exe -l=dnscrypt-resolvers.csv
	=====================================
    56 Resolvers (fastest first)
    Only DNSSEC: False
    Only NoLogs: False
    Only IPv4: True
    =====================================
    16 ms, Cisco OpenDNS with FamilyShield, NoLogs: False, DNSSEC: False
    19 ms, CS Germany DNSCrypt server, NoLogs: True, DNSSEC: False
    19 ms, D0wn Resolver Germany 01, NoLogs: True, DNSSEC: False
    25 ms, D0wn Resolver Netherlands 03, NoLogs: True, DNSSEC: False
    25 ms, CS Italy DNSCrypt server, NoLogs: True, DNSSEC: False
    25 ms, CS cryptofree France DNSCrypt server, NoLogs: True, DNSSEC: False
    26 ms, D0wn Resolver France 02, NoLogs: True, DNSSEC: False
    26 ms, DNSCrypt.eu Holland, NoLogs: True, DNSSEC: True
    26 ms, Cisco OpenDNS, NoLogs: False, DNSSEC: False
    26 ms, CS Switzerland DNSCrypt server, NoLogs: True, DNSSEC: False
    26 ms, DNSCrypt.org France, NoLogs: True, DNSSEC: True
    27 ms, CS secondary France DNSCrypt server, NoLogs: True, DNSSEC: False
    28 ms, D0wn Resolver Netherlands 02, NoLogs: True, DNSSEC: False
    28 ms, CS secondary cryptofree France DNSCrypt server, NoLogs: True, DNSSEC: False
    29 ms, D0wn Resolver United Kingdom 01, NoLogs: True, DNSSEC: False
    33 ms, D0wn Resolver Luxembourg 01, NoLogs: True, DNSSEC: False
    36 ms, DNSCrypt.eu Denmark, NoLogs: True, DNSSEC: True
    46 ms, D0wn Resolver Latvia 01, NoLogs: True, DNSSEC: False
    49 ms, Ipredator.se Server, NoLogs: True, DNSSEC: True
    50 ms, D0wn Resolver Latvia 02, NoLogs: True, DNSSEC: False
    50 ms, D0wn Resolver Spain 01- d0wn, NoLogs: True, DNSSEC: False
    50 ms, D0wn Resolver Sweden 01, NoLogs: True, DNSSEC: False
    59 ms, D0wn Resolver Italy 01, NoLogs: True, DNSSEC: False
    59 ms, Secondary OpenNIC Anycast DNS Resolver, NoLogs: True, DNSSEC: False
    59 ms, Primary OpenNIC Anycast DNS Resolver, NoLogs: True, DNSSEC: False
    60 ms, D0wn Resolver Moldova 01, NoLogs: True, DNSSEC: False
    61 ms, D0wn Resolver Romania 01, NoLogs: True, DNSSEC: False
    63 ms, ns0.dnscrypt.is in Reykjavík, Iceland, NoLogs: True, DNSSEC: True
    64 ms, D0wn Resolver Russia 01, NoLogs: True, DNSSEC: False
    65 ms, D0wn Resolver Iceland 01, NoLogs: True, DNSSEC: False
    93 ms, D0wn Resolver Cyprus 01, NoLogs: True, DNSSEC: False
    120 ms, D0wn Resolver United States of America 01, NoLogs: True, DNSSEC: False
    120 ms, CS New York City NY US DNSCrypt server, NoLogs: True, DNSSEC: False
    123 ms, Anatomical DNS, NoLogs: True, DNSSEC: True
    129 ms, CS Atlanta GA US DNSCrypt server, NoLogs: True, DNSSEC: False
    131 ms, CS Chicago IL US DNSCrypt server, NoLogs: True, DNSSEC: False
    155 ms, CS Dallas TX US DNSCrypt server, NoLogs: True, DNSSEC: False
    170 ms, D0wn Resolver Tanzania 01, NoLogs: True, DNSSEC: False
    176 ms, okTurtles, NoLogs: True, DNSSEC: False
    184 ms, CS Seattle WA US DNSCrypt server, NoLogs: True, DNSSEC: False
    190 ms, CS Las Vegas NV US DNSCrypt server, NoLogs: True, DNSSEC: False
    289 ms, D0wn Resolver Singapore 01, NoLogs: True, DNSSEC: False
    344 ms, D0wn Resolver Hong Kong 01, NoLogs: True, DNSSEC: False
    370 ms, D0wn Resolver Singapore 02, NoLogs: True, DNSSEC: False
    410 ms, D0wn Resolver Indonesia 01, NoLogs: True, DNSSEC: False
    425 ms, D0wn Resolver Australia 01, NoLogs: True, DNSSEC: False

### Options

    -l|list=<path> (the path to the dnscrypt-proxy.csv file)
    -n|nologs (only test resolvers with NoLogs support enabled.)
    -d|dnssec (only test resolvers with DNSSEC support enabled.)
    -h|help (show this message and exit)
		

## License
[MIT](https://en.wikipedia.org/wiki/MIT_License)
