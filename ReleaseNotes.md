LightBlue
====================


Version 1.1.15
--------------------
###Features
* Throw exception if role exits (suppress with --allowSilentFail)

###Implementation
* Update to Azure SDK 2.5
* Update to NewtonSoft.Json 6.0.7
* Update to Castle 3.3.3

Version 1.1.14
--------------------
###Implementation
* Disabled the IIS Express trace logging in the template configuration file.

Version 1.1.13
--------------------
###Features
* Handle the configuration path having a training double quote due to escaping quotes paths in the shell.
* Require worker roles to have a configuration file.

Version 1.1.12
--------------------
###Features
* Support for Castle Windsor
* Basic support for blobs with path separators in their names.
* Add IAzureBlobDirectory.ListBlobsSegmentedAsync
* Lock metadata file for the duration of an update so that concurrent access doesn't overwrite values unexpectedly.
* Add overload of IAzureBlockBlob.UploadFromByteArrayAsync that uploads the entire buffer.
* Add minimal page blob abstraction so that lists don't blow up when the container includes a page blob.
* Add flag that causes LightBlue hosts to provide the hosted storage implementation (via the LightBlue APIs)
* Add additional async variations of storage API methods for which async support makes sense.
* Listing of blobs will load metadata when the BlobListingDetails.Metadata flag is specified for standalone containers and directories.
* Added additional parameter validation to various Standalone classes

###Implementation
* Removed the StandaloneAzureBlockBlob(Uri) constructor.
* Added StandaloneEnvironment.SeparateBlobUri helper
* Added trailing separator character to StandaloneEnvironment.LightBlueDataDirectory
* Don't overwrite unknown values in the metadata file to allow for future extension of the format.

Version 0.1.11
--------------------
###BugFixes
* LightBlue package no longer requires Autofac

###Features
* Don't throw on repeated calls to LightBlueConfiguration.SetAsExternal if the environment does not change.
* If port and useSsl are not specified use the values from the first endpoint in the ServiceDefintion.csdef file
* Start hosted role processes in a new thread so that AppDomain.UnhandledException can be used by roles.
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