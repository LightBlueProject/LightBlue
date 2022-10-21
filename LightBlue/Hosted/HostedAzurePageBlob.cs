using System;
using Azure.Storage.Blobs.Specialized;

namespace LightBlue.Hosted
{
    public class HostedAzurePageBlob : IAzurePageBlob
    {
        private readonly PageBlobClient _cloudPageBlob;

        public HostedAzurePageBlob(PageBlobClient cloudPageBlob)
        {
            _cloudPageBlob = cloudPageBlob;
        }

        public Uri Uri
        {
            get { return _cloudPageBlob.Uri; }
        }
    }
}