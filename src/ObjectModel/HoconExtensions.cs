using System.Linq;
using Hocon;
using NetAF.Targets.Text.Rendering.FrameBuilders;

namespace ObjectModel;

public static class HoconExtensions {
    extension(HoconField field)
    {
        public object GetNumber()
        {
            if (field.Value.First() is HoconLong)
            {
                return long.Parse(field.Value.Raw);
            }
            else if (field.Value.First() is HoconDouble)
            {
                return double.Parse(field.Value.Raw);
            }

            throw new System.InvalidOperationException("Field is not a number.");
        }

        public bool GetBoolean()
        {
            return bool.Parse(field.Value.Raw);
        }
    }
}