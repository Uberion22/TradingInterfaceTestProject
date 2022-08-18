using System.Collections.Generic;
using UnityEngine;

public struct Rectangle
{
    public Vector2 Min;
    public Vector2 Max;
}

public struct Edge
{
    public Rectangle First;
    public Rectangle Second;
    public Vector3 Start;
    public Vector3 End;
}

public class RectangleData
{
    public static IEnumerable<Edge> GetTestData()
    {
        var edges = new List<Edge>();
        var edge = new Edge();
        //1
        edge.First.Min = new Vector2(2, 0);
        edge.First.Max = new Vector2(5, 4);

        edge.Second.Min = new Vector2(4, 4);
        edge.Second.Max = new Vector2(7, 7);

        edge.Start = new Vector2(4, 4);
        edge.End = new Vector2(5, 4);
        edges.Add(edge);
        //2
        edge.First.Min = new Vector2(4, 4);
        edge.First.Max = new Vector2(7, 7);

        edge.Second.Min = new Vector2(2, 7);
        edge.Second.Max = new Vector2(6, 10);

        edge.Start = new Vector2(4, 7);
        edge.End = new Vector2(6, 7);
        edges.Add(edge);
        //3
        edge.First.Min = new Vector2(4, 4);
        edge.First.Max = new Vector2(7, 7);

        edge.Second.Min = new Vector2(7, 1);
        edge.Second.Max = new Vector2(12, 5);

        edge.Start = new Vector2(7, 4);
        edge.End = new Vector2(7, 5);
        edges.Add(edge);
        //4
        edge.First.Min = new Vector2(7, 1);
        edge.First.Max = new Vector2(12, 5);

        edge.Second.Min = new Vector2(10, 5);
        edge.Second.Max = new Vector2(13, 9);

        edge.Start = new Vector2(10, 5);
        edge.End = new Vector2(12, 5);
        edges.Add(edge);
        //5
        edge.First.Min = new Vector2(2, 7);
        edge.First.Max = new Vector2(6, 10);

        edge.Second.Min = new Vector2(6, 8);
        edge.Second.Max = new Vector2(10, 9);

        edge.Start = new Vector2(6, 8);
        edge.End = new Vector2(6, 9);
        edges.Add(edge);
        //6
        edge.First.Min = new Vector2(10, 5);
        edge.First.Max = new Vector2(13, 9);

        edge.Second.Min = new Vector2(12, 6);
        edge.Second.Max = new Vector2(17, 8);

        edge.Start = new Vector2(12, 6);
        edge.End = new Vector2(12, 8);
        edges.Add(edge);
        //7
        edge.First.Min = new Vector2(6, 8);
        edge.First.Max = new Vector2(10, 9);

        edge.Second.Min = new Vector2(10, 5);
        edge.Second.Max = new Vector2(13, 9);

        edge.Start = new Vector2(10, 8);
        edge.End = new Vector2(10, 9);
        edges.Add(edge);

        edge.First.Min = new Vector2(12, 6);
        edge.First.Max = new Vector2(17, 8);
        edges.Add(edge);
        
        return edges;
    }
}
