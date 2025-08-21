using System.ComponentModel;
using System.Reflection;
using System;

namespace GuanajuatoAdminUsuarios.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Obtiene la descripción de los data annotations que se colocaron en los enumeradores
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return enumValue.ToString();
        }
    }
}
