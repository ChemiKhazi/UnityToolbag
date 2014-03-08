using UnityEngine;
using System.Collections;

public class BitmaskHelper {

	public static bool BitmaskContains(int mask, int test)
	{
		return (mask & test) > 0;
	}

	public static int SetToMask(int set, int mask)
	{
		int newMask = mask & set;
		return newMask;
	}
}
