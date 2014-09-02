<Query Kind="Statements">
  <NuGetReference Prerelease="true">LightBlue</NuGetReference>
  <Namespace>LightBlue</Namespace>
  <Namespace>LightBlue.Setup</Namespace>
  <Namespace>Microsoft.WindowsAzure.Storage.Blob</Namespace>
</Query>

LightBlueConfiguration.SetAsExternal(AzureEnvironment.LightBlue);

var storage = LightBlueContext.AzureStorageFactory("DefaultEndpointsProtocol=https;AccountName=lightbluesample;AccountKey=00000000000000000000000000000000000000000000000000000000000000000000000000000000000000==");
var client = storage.CreateAzureBlobStorageClient();

var container = client.GetContainerReference("testcontainer");
container.CreateIfNotExists(BlobContainerPublicAccessType.Off);
container.Dump();

var blob = container.GetBlockBlobReference("testblob");
var buffer = Encoding.UTF8.GetBytes("Some blob content");
await blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
blob.Dump();

var listResults = await container.ListBlobsSegmentedAsync(
	"",
	BlobListing.Flat,
	BlobListingDetails.All,
	null,
	null);

listResults.Results.Dump();

await listResults.Results.OfType<IAzureBlockBlob>().First().DeleteAsync();