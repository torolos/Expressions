using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace ExpressionsDemo
{
    public static class Extensibility
    {
        /// <summary>
        /// Extension method. Checks whether type is anonymous
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>true if anonymous</returns>
        public static Boolean IsAnonymousType(this Type type)
        {
            return type.Name.Contains("AnonymousType");
        }
        /// <summary>
        /// Invokes the Parse Method of a value type. The only restriction that applies is, when
        /// the return object type is Boolean. Boolean Parse or TryParse accept a string which has 
        /// to be "true" or "false". An exception will be thrown otherwise
        /// </summary>
        /// <param name="value">the value to parse</param>
        /// <param name="type">the type to which value has to be converted</param>
        /// <returns>object</returns>
        public static object InvokeParse(this object value, Type type)
        {
            try
            {
                Type t = null;
                // if type is Nullable the store generic argument type, otherwise the type itself
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    t = type.GetGenericArguments().First();
                else
                    t = type;
                // Invoke parse method on the type. Because method is static target will be ignored
                return t.InvokeMember("Parse", BindingFlags.InvokeMethod, null, null, new object[] { value });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Iterates through instance public properties of a chosen type, returning either the 
        /// property with selected name or property whose column attribute name value equals to name.
        /// The iteration is not case sensitive
        /// </summary>
        /// <param name="type">the type in question</param>
        /// <param name="name">the name of the property</param>
        /// <returns>The PropertyInfo object for the requested property</returns>
        public static PropertyInfo FindProperty(this Type type, string name)
        {
            return type.FindInstanceProperties().FindProperty(name);
        }
        /// <summary>
        /// Iterates through an array properties of a chosen type, returning either the 
        /// property with selected name or property whose column attribute name value equals to name.
        /// The iteration is not case sensitive
        /// </summary>
        /// <param name="type">the type in question</param>
        /// <param name="name">the name of the property</param>
        /// <returns>The PropertyInfo object for the requested property</returns>
        public static PropertyInfo FindProperty(this PropertyInfo[] properties, string name)
        {
            PropertyInfo result = null;
            result = properties.Where(c => c.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (result == null)
            {
                foreach (PropertyInfo item in properties)
                {
                    if (Attribute.IsDefined(item, typeof(ColumnAttribute)))
                    {
                        string val = ((ColumnAttribute)item.GetCustomAttributes(typeof(ColumnAttribute), true)[0]).Name;
                        if (!string.IsNullOrEmpty(val) && val == name)
                            result = item;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Retrieves all public instance properties for a specific type
        /// </summary>
        /// <param name="type">the type in question</param>
        /// <returns>A PropertyInfo array</returns>
        public static PropertyInfo[] FindInstanceProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
