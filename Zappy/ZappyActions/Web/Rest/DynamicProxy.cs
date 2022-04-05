using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Zappy.ZappyActions.Web.Rest
{
    public class DynamicProxy : DynamicObject
    {
        public DynamicProxy(Type proxyType, Binding binding, EndpointAddress address) : base(proxyType)
        {
            Type[] paramTypes = new Type[] { typeof(Binding), typeof(EndpointAddress) };
            object[] paramValues = new object[] { binding, address };
            CallConstructor(paramTypes, paramValues);
        }

        public void Close()
        {
            CallMethod("Close", new object[0]);
        }

        public object Proxy =>
            ObjectInstance;

        public Type ProxyType =>
            ObjectType;
    }
}

