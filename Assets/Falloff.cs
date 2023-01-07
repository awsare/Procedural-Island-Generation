using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Falloff
{
	public static Vector3[] GenerateFalloff(Vector2 regionSize, float a, float b)
	{
		int width = (int)regionSize.x + 1;
		int length = (int)regionSize.y + 1;
		int size = width * length;

		Vector3[] map = new Vector3[width * length];

		for (int index = 0, z = 0; z < length; z++)
		{
			for (int x = 0; x < width; x++, index++)
			{
				float xValue = (x / (float)width * 2) - 1;
				float yValue = (z / (float)length * 2) - 1;

				float value = Mathf.Max(Mathf.Abs(xValue), Mathf.Abs(yValue));
				map[index].y = Evaluate(value, a, b);
			}
		}

		return map;
	}

	static float Evaluate(float value, float a, float b)
	{
		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}
}