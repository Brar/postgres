using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PlClr
{
    internal sealed class TypeAccessInfo
    {
        public TypeAccessInfo(Type mappedType, Type accessMethodType, string accessMethodName, Type creationMethodType, string creationMethodName)
        {
            MappedType = mappedType;
            AccessMethodType = accessMethodType;
            AccessMethodName = accessMethodName;
            CreationMethodType = creationMethodType;
            CreationMethodName = creationMethodName;
        }

        public Type MappedType { get; }
        public Type AccessMethodType { get; }
        public string AccessMethodName { get; }
        public Type CreationMethodType { get; }
        public string CreationMethodName { get; }
    }
}
