using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VoronoiDiagram : MonoBehaviour
{
    [Tooltip("The source texture used to determine the width and height for the Voronoi diagram. If not assigned, the texture from the GameObject's SpriteRenderer will be used.")]
    [SerializeField] private Texture2D sourceTexture;

    [Tooltip("Controls whether to display regions or only edges.")]
    [SerializeField] private VoronoiOutputMode outputMode = VoronoiOutputMode.Regions;

    [Tooltip("Color used for the edges between Voronoi regions.")]
    [SerializeField] private Color edgeColor = Color.black;

    [Tooltip("Thickness of the edges between Voronoi regions.")]
    [SerializeField] private int edgeThickness = 1;

    [Tooltip("Controls the falloff rate of the edge color blending into the region color.")]
    [SerializeField] private float edgeFalloff = 5.0f;  // Higher values mean a sharper transition

    [Tooltip("The number of seed points for the Voronoi diagram.")]
    [SerializeField] private int seedPointCount = 1000;

    [Tooltip("Seed for the random number generator to ensure consistent point generation.")]
    [SerializeField] private int randomSeed = 42;

    [Tooltip("Array of colors for the Voronoi regions. If not provided, random colors will be used.")]
    [SerializeField] private Color[] regionColors;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float baseAlphaChannel;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float alphaThreshold;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float noiseFactor;
    [SerializeField] private float scale;
    [SerializeField] private int octaves;
    [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private int noiseSeed;

    private Texture2D voronoiTexture;
    private Quadtree quadtree;

    void Start()
    {
        try
        {
            GenerateVoronoiTexture();
            applyNoise();
            ApplyTexture(voronoiTexture);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error generating Voronoi texture: {ex.Message}");
        }
    }
    // TODO fix me please
    /*   void OnValidate()
       {
           if (!Application.isPlaying)
           {
               try
               {
                   GenerateVoronoiTexture();
                   ApplyTexture();
               }
               catch (System.Exception ex)
               {
                   Debug.LogError($"Error generating Voronoi texture: {ex.Message}");
               }
           }
       }*/

    public void GenerateVoronoiTexture()
    {
        Texture2D texture = GetSourceTexture();
        if (texture == null)
        {
            throw new System.ArgumentNullException(nameof(texture), "No valid source texture found.");
        }

        int width = texture.width;
        int height = texture.height;

        voronoiTexture = new Texture2D(width, height);
        ColorDensityTextureGenerator generator = new ColorDensityTextureGenerator(sourceTexture, alphaThreshold);
        Texture2D colorDensityTexture = generator.GenerateColorDensityTexture();

        //ApplyTexture(colorDensityTexture);

        // Vector2[] seedPoints = GenerateRandomPoints(width, height, seedPointCount);
        Vector2[] seedPoints = CDFPointsDistribution(colorDensityTexture, seedPointCount);
        quadtree = new Quadtree(new Rect(0, 0, width, height));

        // Insert seed points into Quadtree
        foreach (Vector2 point in seedPoints)
        {
            quadtree.Insert(point);
        }

        if (sourceTexture != null)
        {
            PopulateVoronoiTexture(width, height, seedPoints, sourceTexture);
        }
        else
        {
            Color[] colors = GenerateRandomColors(seedPointCount);
            PopulateVoronoiTexture(width, height, seedPoints, colors);
        }
    }

    private Texture2D GetSourceTexture()
    {
        if (sourceTexture != null)
        {
            return sourceTexture;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            return spriteRenderer.sprite.texture;
        }

        return null;
    }

    public Texture2D GetVoronoiTexture()
    {
        if (voronoiTexture != null)
        {
            return voronoiTexture;
        }
        return null;
    }

    private Vector2[] GenerateRandomPoints(int width, int height, int count)
    {
        Vector2[] points = new Vector2[count];

        // Set the random seed
        UnityEngine.Random.InitState(randomSeed);

        for (int i = 0; i < count; i++)
        {
            points[i] = new Vector2(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        }

        return points;
    }

    private Color[] GenerateRandomColors(int count)
    {
        Color[] colors = new Color[count];
        for (int i = 0; i < count; i++)
        {
            colors[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }
        return colors;
    }

    private Vector2[] CDFPointsDistribution(Texture2D imageDensity, int count)
    {
        Color[] pixels = imageDensity.GetPixels();
        int width = imageDensity.width;
        int height = imageDensity.height;

        float[] pixelValues = new float[pixels.Length];
        float totalValue = 0f;
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0)
            {
                pixelValues[i] = pixels[i].grayscale;
                totalValue += pixelValues[i];
            }
            else
            {
                pixelValues[i] = 0f;
            }
        }

        if (totalValue == 0)
        {
            Debug.LogError("The density image has no valid values below the alpha threshold.");
            return new Vector2[0];
        }

        float[] cdf = new float[pixelValues.Length];
        float cumulative = 0f;
        for (int i = 0; i < pixelValues.Length; i++)
        {
            cumulative += pixelValues[i] / totalValue;
            cdf[i] = cumulative;
        }

        Vector2[] points = new Vector2[count];
        System.Random random = new System.Random();
        for (int i = 0; i < count; i++)
        {
            float randomValue = (float)random.NextDouble();
            int index = Array.FindIndex(cdf, value => value >= randomValue);

            int x = index % width;
            int y = index / width;

            points[i] = new Vector2(x, y);
        }

        return points;
    }

    private void PopulateVoronoiTexture(int width, int height, Vector2[] seedPoints, Color[] colors)
    {
        // TODO fix me
        // I'm eating your performance
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 queryPoint = new Vector2(x, y);
                Vector2 nearestSeedPoint = quadtree.FindNearest(queryPoint);
                int closestSeedIndex = Array.IndexOf(seedPoints, nearestSeedPoint);

                if (closestSeedIndex != -1 && closestSeedIndex < colors.Length)
                {
                    voronoiTexture.SetPixel(x, y, colors[closestSeedIndex]);
                }
                else
                {
                    // Handle case where nearestSeedPoint is not found or index is out of range
                    // This could be due to the point being outside the seedPoints range
                    // or an issue with the Quadtree implementation
                    voronoiTexture.SetPixel(x, y, Color.black); // Placeholder color or fallback solution
                    Debug.LogWarning($"No valid seed point found for query point ({x}, {y}). Setting pixel to fallback color.");
                }
            }
        }
    }

    private void PopulateVoronoiTexture(int width, int height, Vector2[] seedPoints, Texture2D sourceTexture)
    {
        int[,] regionMap = new int[width, height];
        Dictionary<int, List<Color>> regionColors = new Dictionary<int, List<Color>>();

        // First pass: Assign regions and collect colors
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = sourceTexture.GetPixel(x, y);

                // Check if the alpha value of the pixel is below the threshold
                if (pixelColor.a > alphaThreshold)
                {
                    Vector2 queryPoint = new Vector2(x, y);
                    Vector2 nearestSeedPoint = quadtree.FindNearest(queryPoint);
                    int closestSeedIndex = Array.IndexOf(seedPoints, nearestSeedPoint);

                    if (closestSeedIndex >= 0)  // Ensure the seed point was found
                    {
                        regionMap[x, y] = closestSeedIndex;

                        if (!regionColors.ContainsKey(closestSeedIndex))
                        {
                            regionColors[closestSeedIndex] = new List<Color>();
                        }

                        regionColors[closestSeedIndex].Add(pixelColor);

                        if (outputMode == VoronoiOutputMode.Regions || outputMode == VoronoiOutputMode.RegionsWithEdges)
                        {
                            // Temporarily set to any color; will overwrite with average color later
                            voronoiTexture.SetPixel(x, y, Color.clear);
                        }
                    }
                }
            }
        }

        // Calculate average color for each region
        Dictionary<int, Color> averageColors = new Dictionary<int, Color>();
        foreach (var regionIndex in regionColors.Keys)
        {
            averageColors[regionIndex] = AverageColor(regionColors[regionIndex]);
        }

        // Apply average colors and detect edges
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int regionIndex = regionMap[x, y];
                Color regionColor = averageColors[regionIndex];

                if (outputMode == VoronoiOutputMode.Edges)
                {
                    float edgeStrength = IsOnEdge(x, y, regionMap, width, height, edgeThickness, edgeFalloff);
                    //Debug.Log($"edge strength: {edgeStrength}");
                    float colorR = Mathf.Clamp01(voronoiTexture.GetPixel(x, y).r * (1 - edgeStrength) + edgeColor.r * edgeStrength);
                    float colorG = Mathf.Clamp01(voronoiTexture.GetPixel(x, y).g * (1 - edgeStrength) + edgeColor.g * edgeStrength);
                    float colorB = Mathf.Clamp01(voronoiTexture.GetPixel(x, y).b * (1 - edgeStrength) + edgeColor.b * edgeStrength);
                    voronoiTexture.SetPixel(x, y, new Color(colorR, colorG, colorB));
                }
                else if (regionColors.ContainsKey(regionIndex))  // Check if the regionIndex has been recorded
                {
                    voronoiTexture.SetPixel(x, y, regionColor);

                    if (outputMode == VoronoiOutputMode.RegionsWithEdges)
                    {
                        // Apply edge color if this pixel is on the border
                        //if (IsOnEdge(x, y, regionMap, width, height))
                        //{
                        //}
                        float edgeStrength = IsOnEdge(x, y, regionMap, width, height, edgeThickness, edgeFalloff);
                        float colorR = Mathf.Clamp01(voronoiTexture.GetPixel(x,y).r * (1 - edgeStrength) + edgeColor.r * edgeStrength);
                        float colorG = Mathf.Clamp01(voronoiTexture.GetPixel(x, y).g * (1 - edgeStrength) + edgeColor.g * edgeStrength);
                        float colorB = Mathf.Clamp01(voronoiTexture.GetPixel(x, y).b * (1 - edgeStrength) + edgeColor.b * edgeStrength);
                        voronoiTexture.SetPixel(x, y, new Color(colorR, colorG, colorB));
                    }
                }
            }
        }

        voronoiTexture.Apply();
    }

    private bool IsOnEdge(int x, int y, int[,] regionMap, int width, int height)
    {
        int currentRegion = regionMap[x, y];
        // Check adjacent pixels to see if they are in the same region
        return (x > 0 && regionMap[x - 1, y] != currentRegion) ||
               (x < width - 1 && regionMap[x + 1, y] != currentRegion) ||
               (y > 0 && regionMap[x, y - 1] != currentRegion) ||
               (y < height - 1 && regionMap[x, y + 1] != currentRegion);
    }

    private float IsOnEdge(int x, int y, int[,] regionMap, int width, int height, int edgeThickness, float edgeFalloff)
    {
        int currentRegion = regionMap[x, y];
        float edgeIntensity = 0f;

        // Check pixels within the edgeThickness distance to see if they are in the same region
        for (int dx = -edgeThickness; dx <= edgeThickness; dx++)
        {
            for (int dy = -edgeThickness; dy <= edgeThickness; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (dx != 0 || dy != 0)
                    {
                        if (regionMap[nx, ny] != currentRegion)
                        {
                            float distance = Mathf.Sqrt(dx * dx + dy * dy);
                            float weight = Mathf.Exp(-distance / edgeFalloff);
                            edgeIntensity += weight;
                        }
                    }
                }
            }
        }
        return edgeIntensity;
    }

    private Color AverageColor(List<Color> colors)
    {
        float totalR = 0, totalG = 0, totalB = 0;
        int count = colors.Count;

        foreach (var color in colors)
        {
            totalR += color.r;
            totalG += color.g;
            totalB += color.b;
        }

        return new Color(totalR / count, totalG / count, totalB / count, baseAlphaChannel);
    }

    private Color[] GenerateColorsFromTexture(int count, Vector2[] points, Texture2D texture)
    {
        Color[] colors = new Color[count];
        for (int i = 0; i < count; i++)
        {
            // Calculate texture coordinates based on the point's position relative to texture size
            int x = (int)(points[i].x / voronoiTexture.width * texture.width);
            int y = (int)(points[i].y / voronoiTexture.height * texture.height);
            colors[i] = texture.GetPixel(x, y);
        }
        return colors;
    }

    private void applyNoise()
    {
        PerlinNoise perlinNoise = new PerlinNoise(sourceTexture.width, sourceTexture.height, scale, octaves, persistence, lacunarity,noiseSeed, 0, 0);
        PerlinNoise alphaPerlinNoise = new PerlinNoise(sourceTexture.width, sourceTexture.height, scale, octaves, persistence, lacunarity, noiseSeed, -100, -100);
        float[,] noiseValues = perlinNoise.GenerateTexture();
        float[,] alphaNoiseValues = alphaPerlinNoise.GenerateTexture();

        for (int y = 0; y < sourceTexture.height; y++)
        {
            for (int x = 0; x < sourceTexture.width; x++)
            {
                Color sourceColor = voronoiTexture.GetPixel(x, y);
                if (sourceColor.a <= alphaThreshold)
                    continue;
                float noiseValue = noiseValues[x, y];
                float alphaValue = alphaNoiseValues[x, y];

                float colorR = Mathf.Clamp01(sourceColor.r * (1 - noiseFactor) + noiseValue * noiseFactor);
                float colorG = Mathf.Clamp01(sourceColor.g * (1 - noiseFactor) + noiseValue * noiseFactor);
                float colorB = Mathf.Clamp01(sourceColor.b * (1 - noiseFactor) + noiseValue * noiseFactor);
                float colorA = Mathf.Clamp01(sourceColor.a * (1 - noiseFactor) + alphaValue * noiseFactor);
                Color newColor = new Color(colorR, colorG, colorB, colorA);
                //Debug.Log($"new color: {newColor}, prev color {sourceColor}, noiseValue: {noiseValue}");
                voronoiTexture.SetPixel(x, y, newColor);
            }
        }
        voronoiTexture.Apply();
    }

    public void ApplyTexture(Texture2D texture)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector3 originalScale = transform.localScale;
            Vector2 originalSize = spriteRenderer.sprite.bounds.size;

            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);

            transform.localScale = originalScale;
            spriteRenderer.sprite = newSprite;
            spriteRenderer.size = originalSize;
        }
        else
        {
            throw new System.Exception("SpriteRenderer component not found.");
        }
    }

    public void ApplyTexture()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector3 originalScale = transform.localScale;
            Vector2 originalSize = spriteRenderer.sprite.bounds.size;

            Sprite newSprite = Sprite.Create(voronoiTexture, new Rect(0, 0, voronoiTexture.width, voronoiTexture.height), new Vector2(0.5f, 0.5f), 100);

            transform.localScale = originalScale;
            spriteRenderer.sprite = newSprite;
            spriteRenderer.size = originalSize;
        }
        else
        {
            throw new System.Exception("SpriteRenderer component not found.");
        }
    }
}

public class Quadtree
{
    private Node root;

    public Quadtree(Rect bounds)
    {
        root = new Node(bounds);
    }

    public void Insert(Vector2 point)
    {
        root.Insert(point);
    }

    public Vector2 FindNearest(Vector2 queryPoint)
    {
        var nearest = root.FindNearest(queryPoint, float.MaxValue);
        if (nearest != null)
        {
            return nearest.Value;
        }
        throw new Exception("No nearest point found, even after backtracking.");
    }

    private class Node
    {
        public Rect bounds;
        public List<Vector2> points = new List<Vector2>();
        public Node[] children;

        public Node(Rect bounds)
        {
            this.bounds = bounds;
        }

        public void Subdivide()
        {
            float halfWidth = bounds.width / 2;
            float halfHeight = bounds.height / 2;
            float overlap = 0.01f;  // Small overlap to prevent edge issues
            children = new Node[4];
            children[0] = new Node(new Rect(bounds.x - overlap, bounds.y - overlap, halfWidth + overlap, halfHeight + overlap));
            children[1] = new Node(new Rect(bounds.x + halfWidth, bounds.y - overlap, halfWidth + overlap, halfHeight + overlap));
            children[2] = new Node(new Rect(bounds.x - overlap, bounds.y + halfHeight, halfWidth + overlap, halfHeight + overlap));
            children[3] = new Node(new Rect(bounds.x + halfWidth, bounds.y + halfHeight, halfWidth + overlap, halfHeight + overlap));
        }

        public void Insert(Vector2 point)
        {
            if (children != null)
            {
                // Determine which child node the point should go into
                int index = (point.x >= bounds.x + bounds.width / 2 ? 1 : 0) +
                            (point.y >= bounds.y + bounds.height / 2 ? 2 : 0);
                children[index].Insert(point);
            }
            else
            {
                points.Add(point);
                if (points.Count > 1 && bounds.width > 10)  // Threshold to prevent over subdivision
                {
                    Subdivide();
                    foreach (var p in points)
                    {
                        Insert(p);
                    }
                    points.Clear();
                }
            }
        }

        public Vector2? FindNearest(Vector2 queryPoint, float minDist)
        {
            Vector2? nearest = null;
            float nearestDist = minDist;

            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child != null && SquaredDistanceToRect(child.bounds, queryPoint) < nearestDist * nearestDist)
                    {
                        Vector2? candidate = child.FindNearest(queryPoint, nearestDist);
                        if (candidate.HasValue)
                        {
                            float dist = Vector2.Distance(queryPoint, candidate.Value);
                            if (dist < nearestDist)
                            {
                                nearest = candidate;
                                nearestDist = dist;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var point in points)
                {
                    float dist = Vector2.Distance(queryPoint, point);
                    if (dist < nearestDist)
                    {
                        nearest = point;
                        nearestDist = dist;
                    }
                }
            }

            return nearest;
        }

        public static float SquaredDistanceToRect(Rect rect, Vector2 point)
        {
            float dx = Mathf.Max(rect.xMin - point.x, 0, point.x - rect.xMax);
            float dy = Mathf.Max(rect.yMin - point.y, 0, point.y - rect.yMax);
            return dx * dx + dy * dy;
        }
    }
}

public enum VoronoiOutputMode
{
    Regions,
    Edges,
    RegionsWithEdges
}

public class ColorDensityTextureGenerator
{
    private Texture2D originalTexture;
    private float alphaThreshold;

    public ColorDensityTextureGenerator(Texture2D texture, float alphaThreshold)
    {
        originalTexture = texture;
        this.alphaThreshold = alphaThreshold;
    }

    public Texture2D GenerateColorDensityTexture()
    {
        int width = originalTexture.width;
        int height = originalTexture.height;
        float alphaChannel = 1;
        Texture2D newTexture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float colorChange = CalculateColorChange(x, y);
                //if (colorChange == -1)
                alphaChannel = colorChange == -1 ? 0 : 1;
                float normalizedChange = Mathf.Clamp(colorChange, 0, 1);
                newTexture.SetPixel(x, y, new Color(normalizedChange, normalizedChange, normalizedChange, alphaChannel));
            }
        }

        newTexture.Apply();
        return newTexture;
    }

    private float CalculateColorChange(int x, int y)
    {
        Color currentColor = originalTexture.GetPixel(x, y);
        float colorChangeSum = 0;
        int count = 0;

        if (currentColor.a <= alphaThreshold)
            return -1;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int checkX = x + i;
                int checkY = y + j;
                if (checkX >= 0 && checkX < originalTexture.width && checkY >= 0 && checkY < originalTexture.height)
                {
                    Color neighborColor = originalTexture.GetPixel(checkX, checkY);
                    // Compute the color difference manually for each channel
                    float rDiff = currentColor.r - neighborColor.r;
                    float gDiff = currentColor.g - neighborColor.g;
                    float bDiff = currentColor.b - neighborColor.b;
                    float aDiff = currentColor.a - neighborColor.a;

                    // Compute the Euclidean distance between the two colors
                    float colorDiffMagnitude = Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff + aDiff * aDiff);
                    colorChangeSum += colorDiffMagnitude;
                    count++;
                }
            }
        }

        return count > 0 ? colorChangeSum / count : 0;
    }
}