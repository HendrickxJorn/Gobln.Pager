using System;
using System.Linq;
using System.Reflection;

namespace Gobln.Pager.Infrastructure
{
    internal static class AttributeExtension
    {
        /// <summary>
        /// Get the first attribute from <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>The first attribute or empty</returns>
        public static T GetAttribute<T>(this Type source, bool inherit = false) where T : Attribute
        {
            return source.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
        }

        /// <summary>
        /// Get the first attribute from <see cref="PropertyInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>The first attribute or empty</returns>
        public static T GetAttribute<T>(this PropertyInfo source, bool inherit = false) where T : Attribute
        {
            return source.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
        }

        /// <summary>
        /// Get the first attribute from <see cref="FieldInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>The first attribute or empty</returns>
        public static T GetAttribute<T>(this FieldInfo source, bool inherit = false) where T : Attribute
        {
            return source.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
        }

        /// <summary>
        /// Get the first attribute from <see cref="MemberInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>The first attribute or empty</returns>
        public static T GetAttribute<T>(this MemberInfo source, bool inherit = false) where T : Attribute
        {
            return source.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this Type source) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T));
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this Type source, bool inherit) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T), inherit);
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="PropertyInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this PropertyInfo source) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T));
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="PropertyInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this PropertyInfo source, bool inherit) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T), inherit);
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="FieldInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this FieldInfo source) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T));
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="FieldInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this FieldInfo source, bool inherit) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T), inherit);
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="MemberInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this MemberInfo source) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T));
        }

        /// <summary>
        /// Check if an attribute exist for <see cref="MemberInfo"/>
        /// </summary>
        /// <typeparam name="T">The attribute to get</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns>Exist : true else false</returns>
        public static bool HasAttribute<T>(this MemberInfo source, bool inherit) where T : Attribute
        {
            return Attribute.IsDefined(source, typeof(T), inherit);
        }
    }
}