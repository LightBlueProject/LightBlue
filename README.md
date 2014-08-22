LightBlue
=========
LightBlue is an alternate approach for Azure development to the Azure emulator. 

Features
--------
* Supports running worker and web roles individually in console hosts
* Core APIs including settings, environment and resources
* Storage API providing key portions of the block blob API
* HTTPS support for web roles

Planned Features
----------------
* Queue storage abstraction

LightBlue doesn't aim to abstract every API that Azure provides. PRs adding missing features welcome.

Key Advantages
--------------
* Faster role startup
* Individual control of roles
* Improved blob storage performance over the storage emulator
* No packaging overhead

Why Not Use The Azure Emulator?
-------------------------------
Because it seeks to emulate the Azure environment the emulator has non-trivial overhead. When you start having more than a few roles the time taken to package, deploy and start becomes prohibitive. Reducing startup time is a significant boost in developer productivity.

Additionally we have found the storage emulator to have some significant performance issues. Our system pushes a few hundred items into blob storage when initialised. This can take minutes and on some developer systems simply does not work at all. Blob storage in actual Azure environments does not display this performance issue, it seems to be unique to the emulator. Replacing the storage emulator for this usage is necessary for us to be able to work on our codebase. It's not practical to develop against Azure itself, LightBlue is our solution to this requirement. We've also found the storage emulator to use large amounts of disk space, LightBlue is relatively frugal in comparison.

Additionally LightBlue removes some of the limitations we've experienced in using the emulator. The compute emulator only allows control at the deployment level, which may contain many roles. With LightBlue roles can be individually stopped and started. This kind of fine grained control is useful when dealing with a system with multiple components.

How Closely Does LightBlue Match Azure?
---------------------------------------
The LightBlue storage APIs are in general fairly similar to the Azure SDK APIs. The abstractions are not identical but should be familiar to developers who've used the Azure SDKs.

LightBlue makes a deliberate decision to not be identical to Azure in favour of being simple and loghtweight. The blob storage implementation provided for development use is based on the file system. As such this places limitations on length and valid characters in blob names that do not match what Azure supports. LightBlue also does not support any form of security for blob storage access.

Most code developed against LightBlue should work against Azure without issue but verification against the emulator or actual Azure is recommended. LightBlue supports this by retaining fully the ability to run against the emulator.
