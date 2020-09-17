namespace Applitools.Utils
{
    using System;

    /// <summary>
    /// Identifies methods that validate that their arguments are not null (needed for 
    /// static analysis).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}
