using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
	public static bool IsAnyKeyHeld(List<KeyCode> keys)
	{
		foreach (KeyCode key in keys)
		{
			if (Input.GetKey(key))
			{
				return true;
			}
		}
		return false;
	}
}
