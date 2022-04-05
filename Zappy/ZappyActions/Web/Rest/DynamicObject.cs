using System;
using System.Reflection;

namespace Zappy.ZappyActions.Web.Rest
{
    public class DynamicObject
    {
        private System.Reflection.BindingFlags CommonBindingFlags;
        private object objectInstance;
        private Type objType;

        public DynamicObject(object obj)
        {
            this.CommonBindingFlags = BindingFlags.Public | BindingFlags.Instance;
            if (obj == null)
            {
                throw new ArgumentNullException("objectInstance");
            }
            this.objectInstance = obj;
            this.objType = obj.GetType();
        }

        public DynamicObject(Type objType)
        {
            this.CommonBindingFlags = BindingFlags.Public | BindingFlags.Instance;
            if (objType == null)
            {
                throw new ArgumentNullException("objType");
            }
            this.objType = objType;
        }

        public void CallConstructor()
        {
            this.CallConstructor(new Type[0], new object[0]);
        }

        public void CallConstructor(Type[] paramTypes, object[] paramValues)
        {
            ConstructorInfo constructor = this.objType.GetConstructor(paramTypes);
            if (constructor == null)
            {
                throw new ProxyException("The constructor matching the specified parameter types is not found.");
            }
            this.objectInstance = constructor.Invoke(paramValues);
        }

        public object CallMethod(string method, params object[] parameters) =>
            this.objType.InvokeMember(method, BindingFlags.InvokeMethod | this.CommonBindingFlags, null, this.objectInstance, parameters);

        public object CallMethod(string method, Type[] types, object[] parameters)
        {
            if (types.Length != parameters.Length)
            {
                throw new ArgumentException("The type for each parameter values must be specified.");
            }
            return this.objType.GetMethod(method, types)?.Invoke(this.objectInstance, this.CommonBindingFlags, null, parameters, null);
        }

        public object GetField(string field) =>
            this.GetMember(field, BindingFlags.GetField | this.CommonBindingFlags);

        private object GetMember(string memeberName, System.Reflection.BindingFlags flags) =>
            this.objType.InvokeMember(memeberName, flags | this.CommonBindingFlags, null, this.objectInstance, null);

        public object GetProperty(string property) =>
            this.GetMember(property, BindingFlags.GetProperty);

        public object SetField(string field, object value) =>
            this.SetMember(field, value, BindingFlags.SetField);

        private object SetMember(string memeberName, object value, System.Reflection.BindingFlags flags)
        {
            object[] args = new object[] { value };
            return this.objType.InvokeMember(memeberName, flags | this.CommonBindingFlags, null, this.objectInstance, args);
        }

        public object SetProperty(string property, object value) =>
            this.SetMember(property, value, BindingFlags.SetProperty);

        public System.Reflection.BindingFlags BindingFlags
        {
            get =>
                this.CommonBindingFlags;
            set
            {
                this.CommonBindingFlags = value;
            }
        }

        public object ObjectInstance =>
            this.objectInstance;

        public Type ObjectType =>
            this.objType;
    }
}

