using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

	/**
	 * 
	 * seed = if we want to get the same map again, we just have to use the same seed
	 * 	// return a 2d array of float values
	 **/
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		// pnrg = seeder random number generator
		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i< octaves; i++)
		{
			float offsetX = prng.Next(-100000,100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		// used to move to the center when modifying the noiseScale
		float halfWidth = mapWidth / 2f;
		float halfHeigth = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1f;
				float frequency = 1f;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{

					// the higher the frequency, the further apart the sample points will be
					// the height values will change more rapidly
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeigth) / scale * frequency + octaveOffsets[i].y;

					// by default perlin noise is in the range of 0 to 1
					// but we want our perlin value to be sometimes negative than that so that
					// our noiseHeight decrease :
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					// we want to increase the noise height by the perlin value of each octave
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight)
				{
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
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
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}

}
