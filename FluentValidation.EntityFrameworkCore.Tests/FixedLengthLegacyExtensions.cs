#if NETCOREAPP2_0
using Microsoft.EntityFrameworkCore.Metadata;

namespace FluentValidation.EntityFrameworkCore.Tests
{
    //
    // Summary:
    //     Returns a flag indicating if the property as capable of storing only fixed-length
    //     data, such as strings.
    //
    // Parameters:
    //   property:
    //     The property.
    //
    // Returns:
    //     A flag indicating if the property as capable of storing only fixed-length data,
    //     such as strings.
    public static class FixedLengthLegacyExtensions
    {
        public static bool? IsFixedLength(this IProperty property)
        {
            return (bool?)property["Relational:IsFixedLength"];
        }
    }
}
#endif