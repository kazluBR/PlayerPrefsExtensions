using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPrefsExtensions {

	private static int BYTES_LIMIT = 1000000; // 1MB
	private static string END_LIST_STR = "#end";

	public static void SetDateTime(string keyName, DateTime value)
	{
		PlayerPrefs.SetString(keyName, Convert.ToString(value));
	}

	public static DateTime GetDateTime(string keyName)
	{
		if (PlayerPrefs.HasKey(keyName))
		{
			return Convert.ToDateTime(PlayerPrefs.GetString(keyName));
		}
		return DateTime.MinValue;
	}
	
	public static void SetBool(string keyName, bool value)
	{
		PlayerPrefs.SetInt(keyName, value ? 1 : 0);
	}

	public static bool GetBool(string keyName)
	{
		if (PlayerPrefs.HasKey(keyName))
		{
			return PlayerPrefs.GetInt(keyName) == 1 ? true : false;
		}
		return false;
	}

	public static void SetObject(string keyName, object obj)
	{
		string json = JsonUtility.ToJson(obj);
		PlayerPrefs.SetString(keyName, json);
	}

	public static T GetObject<T>(string keyName)
	{
		if (PlayerPrefs.HasKey(keyName))
		{
			return JsonUtility.FromJson<T>(PlayerPrefs.GetString(keyName));
		}
		return default(T);
	}

	public static void SetList<T>(string keyName, List<T> list)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.array = list.ToArray();
		IEnumerable<string> result = SplitByBytes(JsonUtility.ToJson(wrapper) + END_LIST_STR);
		int count = 0;
		foreach (string str in result)
		{
			PlayerPrefs.SetString(keyName + count, str);
			count++;
		}
	}

	public static List<T> GetList<T>(string keyName)
	{
		if (PlayerPrefs.HasKey(keyName + "0"))
		{
			string str = "";
			int count = 0;
			while (!str.Contains(END_LIST_STR))
			{
				str += PlayerPrefs.GetString(keyName + count);
				count++;
			}
			str = str.Replace(END_LIST_STR, "");
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(str);
			if (wrapper.array != null)
			{
				return wrapper.array.ToList();
			}
		}
		return new List<T>();
	}

	private static IEnumerable<string> SplitByBytes(string str)
	{
		for (int i = 0; i < str.Length; i += BYTES_LIMIT)
		{
			if (BYTES_LIMIT + i > str.Length)
			{
				BYTES_LIMIT = str.Length - i;
			}
			yield return str.Substring(i, BYTES_LIMIT);
		}
	}

	[Serializable]
	private class Wrapper<T>
	{
		public T[] array;
	}
}
