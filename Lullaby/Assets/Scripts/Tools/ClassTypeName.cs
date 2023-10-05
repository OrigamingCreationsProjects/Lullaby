using System;
using UnityEngine;

namespace Lullaby
{
    public class ClassTypeName : PropertyAttribute // This is a custom attribute that can be used to display a dropdown of all classes that inherit from a base class.
    {
        public Type type;
        public ClassTypeName(Type type)
        {
            this.type = type;
        }
        
    }
}