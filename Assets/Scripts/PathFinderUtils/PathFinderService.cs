using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderService : IPathFinder
{
    private readonly bool _useEightNeighboringPoints;
    
    public PathFinderService(bool useEightNeighboringPoints)
    {
        _useEightNeighboringPoints = useEightNeighboringPoints;
    }
    
    public IEnumerable<Vector2> GetPath(Vector2 a, Vector2 c, IEnumerable<Edge> edges)
    {
        a = new Vector2(RoundToFraction(a.x, 0.5f), RoundToFraction(a.y, 0.5f));
        c = new Vector2(RoundToFraction(c.x, 0.5f), RoundToFraction(c.y, 0.5f));
        var pointsIsCorrect = IsPointOnEdges(a, edges) && IsPointOnEdges(c, edges);
        
        if (!pointsIsCorrect)
        {
            Debug.LogWarning("Point A or C not in edges");
            return new List<Vector2>();
        }

        var graph = GraphCreator.CreateGraph(edges, _useEightNeighboringPoints);
        var path = SearchPath(a, c, graph);

        return path;
    }

    private bool IsPointOnEdges(Vector2 point, IEnumerable<Edge> edges)
    {
        foreach (var edge in edges)
        {
            var correct = IsPointInRectangle(point, edge.First.Min, edge.First.Max)
                          || IsPointInRectangle(point, edge.Second.Min, edge.Second.Max);
            if (correct)
            {
                return true;
            }
        }

        return false;
    }

    private List<Vector2> SearchPath(Vector2 startPoint, Vector2 endPoint, Dictionary<Vector2, HashSet<Vector2>> pointGraph)
    {
        var previousPoints = new Dictionary<Vector2, Vector2>();
        var pointsCost = new Dictionary<Vector2, float>();
        var pointsQueue = new PriorityQueue<float, Vector2>();
        pointsQueue.Enqueue(0, startPoint);
        pointsCost.Add(startPoint, 0);
        var path = new List<Vector2>();
        var lastPoint = startPoint; 

        while (pointsQueue.Count > 0)
        {
            var currentPoint = pointsQueue.Dequeue();
            var pointNeighbors = pointGraph[currentPoint];
            
            if (currentPoint.Equals(endPoint))
            {
                break;
            }

            foreach (var nextPoint in pointNeighbors)
            {
                var newCost = pointsCost[currentPoint] + GetCoast(endPoint, nextPoint) + GetCoast(startPoint, nextPoint);;
                if (!pointsCost.ContainsKey(nextPoint) || newCost < pointsCost[nextPoint])
                {
                    pointsCost[nextPoint] = newCost;
                    var priority = newCost  + GetPriority(lastPoint, currentPoint, nextPoint);
                    previousPoints[nextPoint] = currentPoint;
                    pointsQueue.Enqueue(priority, nextPoint);
                }
            }

            lastPoint = currentPoint;
        }

        var current = endPoint;
        while (!current.Equals(startPoint))
        {
            path.Add(current);
            current = previousPoints[current];
        };
        path.Add(current);
        path.Reverse();

        return path;
    }

    private float GetCoast(Vector2 end, Vector2 start)
    {
        return Math.Abs(end.x - start.x) + Math.Abs(end.y - start.y);
    }

    private float GetPriority(Vector2 last, Vector2 current, Vector2 next)
    {
        var pointsOnTheSameLine = PointsOnTheSameLine(last, current, next);
        if (pointsOnTheSameLine)
        {
            return 0; //0.0001f;
        }

        return GetCoast(current, next);
    }

    private bool PointsOnTheSameLine(Vector2 last, Vector2 current, Vector2 next)
    {
        var isPointsOnTheSameLine = Math.Abs(0.5 * ((last.x - next.x) * (current.y - next.y) - (current.x - next.x) * (last.y - next.y))) < 0.001 && last != next;

        return isPointsOnTheSameLine;
    }

    private bool IsPointInRectangle(Vector2 current, Vector2 min, Vector2 max)
    {
        var isPointOnRect = current.x < max.x && current.y < max.y && current.x > min.x && current.y > min.y;

        return isPointOnRect;
    }

    private float RoundToFraction(float value, float fraction)
    {
        return (float)Math.Round((double)value / fraction) * fraction;
    }
}
