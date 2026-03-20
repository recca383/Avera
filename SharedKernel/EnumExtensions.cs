using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel;
public static class EnumExtensions
{
    public static TFlags And<TFlags, TEnum>(this TFlags flags, TEnum value)
    where TFlags : Enum
    where TEnum : Enum
    {
        int result = Convert.ToInt32(flags, CultureInfo.InvariantCulture)
                    & Convert.ToInt32(value, CultureInfo.InvariantCulture);

        return (TFlags)Enum.ToObject(typeof(TFlags), result);
    }
}
