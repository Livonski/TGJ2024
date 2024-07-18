using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VoronoiDiagram : MonoBehaviour
{
    [Tooltip("The source texture used to determine the width and height for the Voronoi diagram. If not assigned, the texture from the GameObject's SpriteRenderer will be used.")]
    public Texture2D sourceTexture;

    [Tooltip("Controls whether to display regions or only edges.")]
    public VoronoiOutputMode outputMode = VoronoiOutputMode.Regions;

    [Tooltip("Color used for the edges between Voronoi regions.")]
    public Color edgeColor = Color.black;

    [Tooltip("Thickness of the edges between Voronoi regions.")]
    public float edgeThickness = 1.0f;

    [Tooltip("Controls the falloff rate of the edge color blending into the region color.")]
    public float edgeFalloff = 5.0f;  // Higher values mean a sharper transition

    [Tooltip("The number of seed points for the Voronoi diagram.")]
    public int seedPointCount = 1000;

    [Tooltip("Seed for the random number generator to ensure consistent point generation.")]
    public int randomSeed = 42;

    [Tooltip("Array of colors for the Voronoi regions. If not provided, random colors will be used.")]
    public Color[] regionColors;

    private Texture2D voronoiTexture;
    private Quadtree quadtree;

    void Start()
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
        Vector2[] seedPoints = GenerateRandomPoints(width, height, seedPointCount);
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
                Vector2 queryPoint = new Vector2(x, y);
                Vector2 nearestSeedPoint = quadtree.FindNearest(queryPoint);
                int closestSeedIndex = Array.IndexOf(seedPoints, nearestSeedPoint);
                regionMap[x, y] = closestSeedIndex;

                if (!regionColors.ContainsKey(closestSeedIndex))
                {
                    regionColors[closestSeedIndex] = new List<Color>();
                }
                Color pixelColor = sourceTexture.GetPixel(x, y);
                regionColors[closestSeedIndex].Add(pixelColor);

                if (outputMode == VoronoiOutputMode.Regions || outputMode == VoronoiOutputMode.RegionsWithEdges)
                {
                    // Temporarily set to any color; will overwrite with average color later
                    voronoiTexture.SetPixel(x, y, Color.clear);
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
                    voronoiTexture.SetPixel(x, y, edgeColor); // All pixels edge color for edge-only mode
                }
                else
                {
                    voronoiTexture.SetPixel(x, y, regionColor);

                    if (outputMode == VoronoiOutputMode.RegionsWithEdges)
                    {
                        // Apply edge color if this pixel is on the border
                        if (IsOnEdge(x, y, regionMap, width, height))
                        {
                            voronoiTexture.SetPixel(x, y, edgeColor);
                        }
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

    private Color AverageColor(List<Color> colors)
    {
        // TODO
        // exclude points with large alpha channel
        float totalR = 0, totalG = 0, totalB = 0, totalA = 0;
        int count = colors.Count;

        foreach (var color in colors)
        {
            totalR += color.r;
            totalG += color.g;
            totalB += color.b;
            totalA += color.a;
        }

        return new Color(totalR / count, totalG / count, totalB / count, totalA / count);
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