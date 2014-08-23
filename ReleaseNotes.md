LightBlue
====================

Version 0.1.8
--------------------
###Bugfixes
* Don't skip updating the host stub assembly if it already exists to prevent issues with older versions not being updated.

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
###BugFix
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