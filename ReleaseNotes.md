LightBlue
====================

Version 0.1.11
--------------------
###Features
* Don't throw on repeated calls to LightBlueConfiguration.SetAsExternal if the environment does not change.
* Add LightBlueConfiguration.IsInitialised property

Version 0.1.10
--------------------
###Features
* Add functions that allow creating blobs and containers with Uri and StorageCredentials.

Version 0.1.9
--------------------
###BugFixes
* Correct errors when constructing blobs and containers from Uris that include a security token.

Version 0.1.8
--------------------
###BugFixes
* Don't skip updating the host stub assembly if it already exists to prevent issues with older versions not being updated.

###Features
* Provide static access to blob entry points to allow use without an IoC container.
* Support use outside the LightBlue hosts or actual or emulated Azure
* Return values from GetSharedAccessSignature that can be correctly parsed by StorageCredentials when in Standalone mode. This does not produce a usable StorageCredentials instance but this is never validated by Standalone mode.
* Create local resource directories on retrieval.
* Enhance StandaloneAzureBlockBlob.StartCopyFromBlob to be more robust in the face of transient IO errors.

Version 0.1.7
--------------------
###BugFixes
* Fix Autofac registration error when not running in the LightBlue hosts
 
###Features
* Set the temporary directory for roles to be a subdirectory of LightBlueData\temp
* Provide AzireEnvironment on LightBlueContext
* Make configuration method on DetermineAzureEnvironment internal

Version 0.1.6
--------------------
###Features
* Separate Autofac dependency into LightBlue.Autofac

Version 0.1.5
--------------------
### Features
* Start the background process for web roles.
* Set web role host window title.
* Improve host command line option information
* Include role name and process ID in resource directory names
* Allow the host to use an existing stub assembly if there is currently a locked 

Version 0.1.4
--------------------
Nothing of interest

Version 0.1.3
--------------------
###Features
* Set worker role host window title
* Change to Local AppData directory for LightBlueData directory
* Remove Azure trace listeners from web.config on web role startup. This is necessary to prevent exceptions on tracing.

Version 0.1.2
--------------------
###BugFixes
* Change name of GetBlockBlobReference to GetContainerReference on IAzureBlobStorageClient to correctly indicate what the method does.

Version 0.1.1
--------------------
###BugFixes
* Correct licence URLs in NuGet package

###Features
* Remove worker role host dependency on LightBlue.dll
* Add hosts NuGet package

Version 0.1.0
--------------------
###Features
* Worker role host
* Web role host
* AzureEnvironment and AzureEnvironmentSource
* Settings support
* Resource support
* Blob storage abstraction
* Create NuGet package 