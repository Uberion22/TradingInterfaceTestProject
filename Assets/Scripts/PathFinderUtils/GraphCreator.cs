using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphCreator
{
    private const float Step = 0.5f;

    public static Dictionary<Vector2, HashSet<Vector2>> CreateGraph(IEnumerable<Edge> edges, bool useEightNeighboringPoints = true)
    {
        var graph = new Dictionary<Vector2, HashSet<Vector2>>();
        var doorsGraph = CreateGraphByDoors(edges, useEightNeighboringPoints);
        var created = new HashSet<Rectangle>();

        foreach (var edge in edges)
        {
            if (!created.Contains(edge.First))
            {
                created.Add(edge.First);
                CreateGraphPointsByRectangle(edge.First, graph, doorsGraph, useEightNeighboringPoints);
            }

            if (created.Contains(edge.Second)) continue;
            created.Add(edge.Second);
            CreateGraphPointsByRectangle(edge.Second, graph, doorsGraph, useEightNeighboringPoints);
        }

        return graph;
    }

    private static void CreateGraphPointsByRectangle(Rectangle rect, Dictionary<Vector2, HashSet<Vector2>> graph,
        Dictionary<Vector2, HashSet<Vector2>> doorsGraph, bool useEightNeighboringPoints)
    {
        float x = rect.Min.x;
        float y = rect.Min.y;
        while (x <= rect.Max.x)
        {
            while (y <= rect.Max.y)
            {
                var newPoint = new Vector2(x, y);
                if (!graph.ContainsKey(newPoint))
                {
                    graph.Add(newPoint, new HashSet<Vector2>());
                }

                var points = GetPointNeighbors(x, y, useEightNeighboringPoints);
                foreach (var point in points)
                {
                    var isPointBad = !doorsGraph.ContainsKey(point) && !IsPointInRectangle(point, rect.Min, rect.Max);

                    if (graph[newPoint].Contains(point) || isPointBad)
                    {
                        continue;
                    }

                    graph[newPoint].Add(point);
                }

                y += Step;
            }
            x += Step;
            y = rect.Min.y;
        }
    }

    public static Dictionary<Vector2, HashSet<Vector2>> CreateGraphByDoors(IEnumerable<Edge> edges, bool useEightNeighboringPoints)
    {
        var graph = new Dictionary<Vector2, HashSet<Vector2>>();
        foreach (var edge in edges)
        {
            float x;
            float y;

            if (Math.Abs(edge.Start.x - edge.End.x) < 0.01f)
            {
                x = edge.Start.x;
                y = edge.Start.y + Step;
                while (y < edge.End.y)
                {
                    AddPointToDoorsGraph(graph, x, y, useEightNeighboringPoints);
                    y += Step;
                }
            }
            else
            {
                x = edge.Start.x + Step;
                y = edge.Start.y;
                while (x < edge.End.x)
                {
                    AddPointToDoorsGraph(graph, x, y, useEightNeighboringPoints);
                    x += Step;
                }
            }
        }

        return graph;
    }

    private static void AddPointToDoorsGraph(Dictionary<Vector2, HashSet<Vector2>> graph, float x, float y, bool useEightNeighboringPoints)
    {
        var newPoint = new Vector2(x, y);
        if (!graph.ContainsKey(newPoint))
        {
            graph.Add(newPoint, new HashSet<Vector2>());
        }

        var points = GetPointNeighbors(x, y, useEightNeighboringPoints);
        foreach (var point in points)
        {
            if (graph[newPoint].Contains(point))
            {
                continue;
            }
            graph[newPoint].Add(point);
        }
    }

    public static List<Vector2> GetPointNeighbors(float x, float y, bool useEightNeighboringPoints)
    {
        var result = new List<Vector2>
        {
            new Vector2(x - Step, y),
            new Vector2(x, y + Step),
            new Vector2(x + Step, y),
            new Vector2(x, y - Step)
        };
        if (useEightNeighboringPoints)
        {
            result.AddRange(new []
            {
                new Vector2(x - Step, y - Step),
                new Vector2(x - Step, y + Step),
                new Vector2(x + Step, y + Step),
                new Vector2(x + Step, y - Step),
            });
        }

        return result;
    }

    private static bool IsPointInRectangle(Vector2 current, Vector2 min, Vector2 max)
    {
        var isPointOnRect = current.x < max.x && current.y < max.y && current.x > min.x && current.y > min.y;

        return isPointOnRect;
    }
}
