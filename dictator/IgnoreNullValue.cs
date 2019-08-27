﻿using System;

namespace ArangoDriver.dictator
{
    /// <summary>
    /// Ignores property if it contains null value when converting object to or from document format.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreNullValue : Attribute
    {
        public IgnoreNullValue()
        {
        }
    }
}
