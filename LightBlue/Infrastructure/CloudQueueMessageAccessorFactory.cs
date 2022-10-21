using Azure.Storage.Queues.Models;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LightBlue.Infrastructure
{
    public static class CloudQueueMessageAccessorFactory
    {
        public static Action<QueueMessage, string> BuildIdAssigner()
        {
            var idPropertyInfo = typeof(QueueMessage).GetProperty("MessageId");
            var cloudQueueProperty = Expression.Parameter(typeof(QueueMessage));
            var idProperty = Expression.Parameter(typeof(string));

            var assignment = Expression.Assign(Expression.MakeMemberAccess(cloudQueueProperty, idPropertyInfo), idProperty);

            return Expression.Lambda<Action<QueueMessage, string>>(assignment, cloudQueueProperty, idProperty).Compile();
        }

        public static Func<string, QueueMessage> BuildFactory()
        {
            var constructor = typeof(QueueMessage).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, new ParameterModifier[0]);
            var messageBodyParameter = Expression.Parameter(typeof(string));
            var initialisation = Expression.New(constructor, messageBodyParameter);

            return Expression.Lambda<Func<string, QueueMessage>>(initialisation, messageBodyParameter).Compile();
        }
    }
}