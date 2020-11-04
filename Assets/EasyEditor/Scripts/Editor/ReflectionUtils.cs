using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyEditor.Editor
{
	public static class ReflectionUtils
	{
		public static readonly BindingFlags flagAllAccessible = BindingFlags.Instance | BindingFlags.Static |
		                                                    BindingFlags.NonPublic | BindingFlags.Public |
		                                                    BindingFlags.DeclaredOnly;

		public static IEnumerable<Type> GetInheritanceChain(object target)
		{
			Type type = target.GetType();
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}

		// 需要包含所有可能被序列化的字段
		public static IEnumerable<FieldInfo> GetAllAccessibleFields(object target)
		{
			return GetInheritanceChain(target).SelectMany(t => t.GetFields(flagAllAccessible));
		}

		public static IEnumerable<PropertyInfo> GetAllAccessibleProperties(object target)
		{
			return GetInheritanceChain(target).SelectMany(t => t.GetProperties(flagAllAccessible));
		}

		public static IEnumerable<MethodInfo> GetAllAccessibleMethods(object target)
		{
			return GetInheritanceChain(target).SelectMany(t => t.GetMethods(flagAllAccessible));
		}

		public static IEnumerable<FieldInfo> GetAllAccessibleFields(object target, Predicate<FieldInfo> filter)
		{
			return GetAllAccessibleFields(target).Where(f => filter(f));
		}

		public static IEnumerable<PropertyInfo> GetAllAccessibleProperties(object target, Predicate<PropertyInfo> filter)
		{
			return GetAllAccessibleProperties(target).Where(p => filter(p));
		}

		public static IEnumerable<MethodInfo> GetAllAccessibleMethods(object target, Predicate<MethodInfo> filter)
		{
			return GetAllAccessibleMethods(target).Where(m => filter(m));
		}

		public static FieldInfo GetFieldEx(object target, string name)
		{
			return GetAllAccessibleFields(target,f => f.Name == name).FirstOrDefault();
		}

		public static PropertyInfo GetPropertyEx(object target, string name)
		{
			return GetAllAccessibleProperties(target,p => p.Name == name).FirstOrDefault();
		}

		public static MethodInfo GetMethodEx(object target, string name)
		{
			return GetAllAccessibleMethods(target,m => m.Name == name).FirstOrDefault();
		}

		public static IEnumerable<KeyValuePair<T, MemberInfo>> GetAllAttributes<T>(object target) where T : Attribute
        {
			foreach(var f in GetAllAccessibleFields(target))
            {
				T attr = f.GetCustomAttribute<T>();
				if (attr != null)
                {
					yield return new KeyValuePair<T, MemberInfo>(attr, f);
                }
            }
			foreach (var p in GetAllAccessibleProperties(target))
			{
				T attr = p.GetCustomAttribute<T>();
				if (p.GetCustomAttribute<T>() != null)
				{
					yield return new KeyValuePair<T, MemberInfo>(attr, p);
				}
			}
			foreach (var m in GetAllAccessibleMethods(target))
			{
				T attr = m.GetCustomAttribute<T>();
				if (m.GetCustomAttribute<T>() != null)
				{
					yield return new KeyValuePair<T, MemberInfo>(attr, m);
				}
			}
		}

        public static IEnumerable<Type> FindSubTypes(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);
        }

        public static IEnumerable<Type> FindImplementations(Type interfaceType, bool interfaceIsGenericType)
        {
            if (interfaceIsGenericType)
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes()
                        .Where(t => t.GetInterfaces()
                            .Any(i => i.IsGenericType
                                      && i.GetGenericTypeDefinition() == interfaceType)));
            }
            else
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes()
                        .Where(t => t.GetInterfaces().Contains(interfaceType)));
            }
        }

        public static T[] GetCustomAttributes<T>(SerializedProperty property) where T : class
        {
            object target = property.serializedObject.targetObject;
            FieldInfo fieldInfo = GetFieldEx(target, property.name);
            if (fieldInfo == null)
            {
                return Array.Empty<T>();
            }
            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }

        public static T GetCustomAttribute<T>(SerializedProperty property) where T : class
        {
            return GetCustomAttributes<T>(property).FirstOrDefault();
        }

        public static BaseEasyEditorDrawer TryCreateDrawerInstanceForProperty(SerializedProperty serializedProperty)
        {
            BaseEasyEditorAttribute eeAttribute = GetCustomAttribute<BaseEasyEditorAttribute>(serializedProperty);
            if (eeAttribute != null)
            {
                var subTypes = FindSubTypes(typeof(BaseEasyEditorDrawer));
                foreach (var typeOfDrawer in subTypes)
                {
                    EasyEditorDrawerAttribute drawerAttribute = typeOfDrawer.GetCustomAttribute<EasyEditorDrawerAttribute>();
                    if (drawerAttribute != null && drawerAttribute.targetType == eeAttribute.GetType())
                    {
                        BaseEasyEditorDrawer drawer = Activator.CreateInstance(typeOfDrawer) as BaseEasyEditorDrawer;
                        if (drawer != null)
                        {
                            return drawer;
                        }
                    }
                }
                Debug.LogWarning($"Can not find drawer implementation for nameof{eeAttribute.GetType()}");
            }
            return null;
        }
    }
}
