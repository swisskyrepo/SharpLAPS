# SharpLAPS

> The attribute **ms-mcs-AdmPwd** stores the clear-text LAPS password. 

This executable is made to be executed within Cobalt Strike session using `execute-assembly`.
It will retrieve the **LAPS** password from the Active Directory.

Require (either):
* Account with `ExtendedRight` or `Generic All Rights`
* Domain Admin privilege

## Usage

```
  _____ __                     __    ___    ____  _____
  / ___// /_  ____ __________  / /   /   |  / __ \/ ___/
  \__ \/ __ \/ __ `/ ___/ __ \/ /   / /| | / /_/ /\__ \
 ___/ / / / / /_/ / /  / /_/ / /___/ ___ |/ ____/___/ /
/____/_/ /_/\__,_/_/  / .___/_____/_/  |_/_/    /____/
                     /_/
Required
/host:<1.1.1.1>  LDAP host to target, most likely the DC

Optional
/user:<username> Username of the account
/pass:<password> Password of the account
/out:<file>      Outputting credentials to file
/ssl             Enable SSL (LDAPS://)

Usage: SharpLAPS.exe /user:DOMAIN\User /pass:MyP@ssw0rd123! /host:192.168.1.1
```

![]()