namespace SGAT.Web.ModelBinding
{
    [AttributeUsage(
        AttributeTargets.Property |
        AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class PreserveStringAttribute : Attribute
    {
    }
}