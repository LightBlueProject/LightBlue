<Query Kind="Statements">
  <NuGetReference Prerelease="true">LightBlue</NuGetReference>
  <Namespace>LightBlue</Namespace>
  <Namespace>LightBlue.Setup</Namespace>
  <Namespace>Microsoft.WindowsAzure.Storage.Blob</Namespace>
</Query>

/*
If you have a LightBlue issue that you'd like to report you can do so
with this LINQPad template. Add the code that demonstrates your
problem below and upload the file to LINQPad Instant Share
(File -> Upload to Instant Share). Create an issue at 
https://github.com/LightBlueProject/LightBlue/issues and include the
Instant Share URL. Please include as much context as possible so
we know why you find the behaviour to be a problem.
*/

LightBlueConfiguration.SetAsExternal(AzureEnvironment.LightBlue);

