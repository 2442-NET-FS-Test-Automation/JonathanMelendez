using System.ComponentModel;
using System.Reflection;

namespace DarkKitchen.Data;

public enum Units
{
    [Description("Ounces")] oz,
    [Description("Cups")] cup,
    [Description("Kilos")] kg,
    [Description("Liters")] l,
    [Description("Pieces")] pz
}

public static class EnumExtensions
{
    public static string GetAbbreviation(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString())!;
        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        
        return attribute != null ? attribute.Description : value.ToString();
    }
}