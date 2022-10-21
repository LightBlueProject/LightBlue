using System;

namespace LightBlue
{
    [Obsolete("remove as part of azure storage upgrade")] // TODO:
    public enum BlobListing
    {
        Flat = 0,
        Hierarchical = 1
    }
}