using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.CommonClass.Enum
{
    public class BaseNameAttribute : Attribute
    {
        public string Name { get; }

        public BaseNameAttribute(string name)
        {
            Name = name;
        }

        public static string GetBaseName(Languages language)
        {
            var enumType = typeof(Languages);
            var memberInfo = enumType.GetMember(language.ToString());
            var attribute = (BaseNameAttribute)Attribute.GetCustomAttribute(memberInfo[0], typeof(BaseNameAttribute));

            return attribute != null ? attribute.Name : string.Empty;
        }
    }
    public enum Languages
    {
        [BaseName("Mặc định")]
        vi,
        [BaseName("Default")]
        en


    }

    public class Program
    {
        // Helper method to get the BaseName attribute value of a Languages enum value
        
    }
}
