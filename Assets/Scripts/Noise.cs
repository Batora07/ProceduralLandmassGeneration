using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

	// Local = using local minimum and maximum
	// Global = estimating a global min and max
	public enum NormalizeMode { Local, Global };

	/**
	 * 
	 * seed = if we want to get the same map again, we just have to use the same seed
	 * 	// return a 2d array of float values
	 **/
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		// pnrg = seeder random number generator
		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1f;
		float frequency = 1f;

		for (int i = 0; i< octaves; i++)
		{
			float offsetX = prng.Next(-100000,100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) - offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		// used to move to the center when modifying the noiseScale
		float halfWidth = mapWidth / 2f;
		float halfHeigth = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				amplitude = 1f;
				frequency = 1f;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{

					// the higher the frequency, the further apart the sample points will be
					// the height values will change more rapidly
					float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (y - halfHeigth + octaveOffsets[i].y) / scale * frequency;

					// by default perlin noise is in the range of 0 to 1
					// but we want our perlin value to be sometimes negative than that so that
					// our noiseHeight decrease :
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					// we want to increase the noise height by the perlin value of each octave
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				} else if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				// InverseLerp method returns a value between 0 and 1, 
				// so f.e if our noiseMap value = to minNoiseHeight => returns 0
				// if it's equals to maxNoiseHeight  => returns 1
				// if it's half way between the two of it it returns 0.5, etc...
				if (normalizeMode == NormalizeMode.Local)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
				}
				// doing smtg consistent across the entire map
				else
				{
					float normalizedHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
					noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
				}
			}
		}

		return noiseMap;
	}

}
