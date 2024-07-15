using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VoronoiDiagram : MonoBehaviour
{
    [Tooltip("The source texture used to determine the width and height for the Voronoi diagram. If not assigned, the texture from the GameObject's SpriteRenderer will be used.")]
    public Texture2D sourceTexture;

    [Tooltip("The number of seed points for the Voronoi diagram.")]
    public int seedPointCount = 1000;

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

        Color[] colors = regionColors != null && regionColors.Length >= seedPointCount
            ? regionColors
            : GenerateRandomColors(seedPointCount);

        PopulateVoronoiTexture(width, height, seedPoints, colors);
        voronoiTexture.Apply();
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

    private Vector2[] GenerateRandomPoints(int width, int height, int count)
    {
        Vector2[] points = new Vector2[count];
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