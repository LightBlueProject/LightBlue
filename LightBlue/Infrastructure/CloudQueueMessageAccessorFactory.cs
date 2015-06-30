using System;
using System.Linq.Expressions;

using Microsoft.WindowsAzure.Storage.Queue;

namespace LightBlue.Infrastructure
{
    public static class CloudQueueMessageAccessorFactory
    {
        public static Action<CloudQueueMessage, string> BuildIdAssigner()
        {
            var idPropertyInfo = typeof(CloudQueueMessage).GetProperty("Id");
            var cloudQueueProperty = Expression.Parameter(typeof(CloudQueueMessage));
            var idProperty = Expression.Parameter(typeof(string));

            var assignment = Expression.Assign(Expression.MakeMemberAccess(cloudQueueProperty, idPropertyInfo), idProperty);

            return Expression.Lambda<Action<CloudQueueMessage, string>>(assignment, cloudQueueProperty, idProperty).Compile();
        }
    }
}