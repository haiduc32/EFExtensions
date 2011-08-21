using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EFExtensions
{
	public static class EnumMapping
	{
		private static Dictionary<Type, Dictionary<object, object>> enumToValueDict;

		private static Dictionary<Type, Dictionary<object, object>> valueToEnumDict;

		static EnumMapping()
		{
			enumToValueDict = new Dictionary<Type, Dictionary<object, object>>();
			valueToEnumDict = new Dictionary<Type, Dictionary<object, object>>();
		}

		public static object TranslateFromEnum(Type enumType, object value)
		{
			Dictionary<object, object> toValueDict;
			if (!enumToValueDict.TryGetValue(enumType, out toValueDict))
			{
				ReflectOnEnum(enumType);
				toValueDict = enumToValueDict[enumType];
			}

			return toValueDict[value];
		}

		public static object TranslateToEnum(Type enumType, object value)
		{
			Dictionary<object, object> toEnumDict;
			if (!valueToEnumDict.TryGetValue(enumType, out toEnumDict))
			{
				ReflectOnEnum(enumType);
				toEnumDict = valueToEnumDict[enumType];
			}

			return toEnumDict[value];
		}

		private static void ReflectOnEnum(Type enumType)
		{
			string[] constants = Enum.GetNames(enumType);

			Dictionary<object, object> toValueDict = new Dictionary<object, object>();
			Dictionary<object, object> toEnumDict = new Dictionary<object, object>();
			
			foreach (string constant in constants)
			{
				MemberInfo memInfo = enumType.GetMember(constant).Single();
				var foundAttribute = memInfo.GetCustomAttributes(typeof(EnumValueAttribute), false).SingleOrDefault();

				object value = foundAttribute != null ? 
					(foundAttribute as EnumValueAttribute).Value : (object)(int)Enum.Parse(enumType, constant);
				
				toValueDict.Add(Enum.Parse(enumType, constant), value);
				toEnumDict.Add(value, Enum.Parse(enumType, constant));
			}

			enumToValueDict.Add(enumType, toValueDict);
			valueToEnumDict.Add(enumType, toEnumDict);
		}
	}
}
