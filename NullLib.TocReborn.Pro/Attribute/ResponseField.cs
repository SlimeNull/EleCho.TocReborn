using System;
using System.Collections.Generic;
using System.Text;

namespace NullLib.TocReborn.Pro
{
    /// <summary>
    /// Tips: request field here
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequestField : Attribute
    {

    }

    /// <summary>
    /// Tips: response field here
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ResponseField : Attribute
    {
    }
}
