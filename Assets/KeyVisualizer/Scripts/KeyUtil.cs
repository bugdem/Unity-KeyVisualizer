using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.Util
{
	[System.Serializable]
	public enum KeyEventType : byte
	{
		KeyDown = 0,
		KeyPress,
		KeyUp
	}

	public struct KeyEventValue
	{
		public KeyEventType EventType;
		public KeyCode Key;

		public KeyEventValue(KeyEventType eventType, KeyCode key)
		{
			EventType = eventType;
			Key = key;
		}
	}

	public static class KeyUtil
	{
		static readonly KeyCode[] _keyCodes =
			System.Enum.GetValues(typeof(KeyCode))
				.Cast<KeyCode>()
				.Where(k => k < KeyCode.Mouse0)
				.ToArray();

		public static bool IsAnyKeyDown(params KeyCode[] interestingCodes)
		{
			return Enumerable.Intersect(GetCurrentKeys(), interestingCodes).Any();
		}

		public static bool IsAllKeyDown(params KeyCode[] interestingCodes)
		{
			return Enumerable.SequenceEqual(GetCurrentKeys(), interestingCodes);
		}

		public static IEnumerable<KeyCode> GetCurrentKeysDown()
		{
			for (int i = 0; i < _keyCodes.Length; i++)
				if (Input.GetKeyDown(_keyCodes[i]))
					yield return _keyCodes[i];
		}

		public static IEnumerable<KeyCode> GetCurrentKeys()
		{
			if (Input.anyKeyDown)
			{
				for (int i = 0; i < _keyCodes.Length; i++)
					if (Input.GetKey(_keyCodes[i]))
						yield return _keyCodes[i];
			}
		}

		public static IEnumerable<KeyCode> GetCurrentKeysUp()
		{
			for (int i = 0; i < _keyCodes.Length; i++)
				if (Input.GetKeyUp(_keyCodes[i]))
					yield return _keyCodes[i];
		}

		public static IEnumerable<KeyEventValue> GetCurrentKeysAllEvent()
		{
			for (int i = 0; i < _keyCodes.Length; i++)
			{
				if (Input.GetKeyDown(_keyCodes[i]))
					yield return new KeyEventValue(KeyEventType.KeyDown, _keyCodes[i]);
				else if (Input.GetKey(_keyCodes[i]))
					yield return new KeyEventValue(KeyEventType.KeyPress, _keyCodes[i]);
				else if (Input.GetKeyUp(_keyCodes[i]))
					yield return new KeyEventValue(KeyEventType.KeyUp, _keyCodes[i]);
			}
		}
	}
}