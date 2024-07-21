using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
    private int width;
    private int height;
    private float scale;
    private float offsetX;
    private float offsetY;
    private int octaves;
    private float persistence;
    private float lacunarity;
    private int seed;

    public PerlinNoise(int width, int height, float scale, int octaves, float persistence, float lacunarity, int seed, float offsetX, float offsetY)
    {
        this.width = width;
        this.height = height;
        this.scale = scale;
        this.octaves = octaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.seed = seed;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }

    public float[,] GenerateTexture()
    {
        float[,] noiseMap = new float[width, height];
        System.Random prng = new System.Random(seed);

        if (scale <= 0)
            scale = 0.0001f;

        float max = float.MinValue;
        float min = float.MaxValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + offsetX;
                    float sampleY = y / scale * frequency + offsetY;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > max)
                    max = noiseHeight;
                if (noiseHeight < min)
                    min = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(min, max, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}