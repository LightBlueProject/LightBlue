using System;
using System.Runtime.Remoting.Messaging;

namespace LightBlue.Setup
{
    public static class LightBlueContextStorageFactory
    {
        public static Func<T> FromAppDomainContext<T>(string name, Func<T> factory)
        {
            return () =>
            {
                if (AppDomain.CurrentDomain.GetData(name) == null)
                {
                    var data = factory();
                    AppDomain.CurrentDomain.SetData(name, data);
                }
                return (T)AppDomain.CurrentDomain.GetData(name);
            };
        }
        public static Func<T> FromCallContext<T>(string name, Func<T> factory)
        {
            return () =>
            {
                if (CallContext.LogicalGetData(name) == null)
                {
                    var data = factory();
                    CallContext.LogicalSetData(name, data);
                }
                return (T)CallContext.LogicalGetData(name);
            };
        }
    }
}