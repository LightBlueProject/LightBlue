LightBlue
=========
LightBlue is an alternate approach for Azure development to the Azure emulator. 

Features
--------
* Can start a worker role in a console host
* IAzureEnvironmentSource to indicate where the role is running
* Settings API
* Web Role support

Planned Features
----------------
* Blob storage abstraction (initially restricted to key portions of the block blob API)
* Queue storage abstraction

LightBlue doesn't aim to abstract every API that Azure provides. PRs adding missing features welcome.

Why Not Use The Azure Emulator?
------------------------------------
In short, it's kind of terrible.

* It needs to package your code which for larger systems can take quite some time. 
* You can't control individual roles within a deployment
* The storage emulator has massive performance problems which can cross over to being completely unusable under not particularly heavy load.

Apart from the deploy speed these are not problems that Azure itself suffers from. As developing against real Azure is impractical in most scenarios an alternative is needed.