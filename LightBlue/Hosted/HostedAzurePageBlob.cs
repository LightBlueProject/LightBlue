using System;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzurePageBlob : IAzurePageBlob
    {
        private readonly CloudPageBlob _cloudPageBlob;

        public HostedAzurePageBlob(CloudPageBlob cloudPageBlob)
        {
            _cloudPageBlob = cloudPageBlob;
        }

        public Uri Uri
        {
            get { return _cloudPageBlob.Uri; }
        }
    }
}