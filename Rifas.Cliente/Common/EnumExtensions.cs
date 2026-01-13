using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Rifas.Client.Common
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            if (value == null) return string.Empty;
            var type = value.GetType();
            var mem = type.GetMember(value.ToString()).FirstOrDefault();
            if (mem == null) return value.ToString();

            // Prefer DisplayAttribute
            var displayAttr = mem.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null && !string.IsNullOrWhiteSpace(displayAttr.Name))
                return displayAttr.Name;

            // Fallback DescriptionAttribute
            var descAttr = mem.GetCustomAttribute<DescriptionAttribute>();
            if (descAttr != null && !string.IsNullOrWhiteSpace(descAttr.Description))
                return descAttr.Description;

            return value.ToString();
        }
    }
}
