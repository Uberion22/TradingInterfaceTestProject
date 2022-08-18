using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RectsAndPath : MonoBehaviour
{
    [SerializeField] private GameObject _rectPrefab;
    [SerializeField] private LineRenderer _lineRenderer;
    
    private const float XScaleModifer = 0.1f;
    private const float YScaleModifer = 1f;
    private const float ZScaleModifer = 0.1f;

    void Start()
    {
        TestRectangleCreator();
    }

    private void DrawRect(Rectangle rectangle)
    {
        var xCenter = (rectangle.Max.x + rectangle.Min.x) / 2;
        var yCenter = (rectangle.Max.y + rectangle.Min.y) / 2;
        var width = (rectangle.Max.x - rectangle.Min.x) * XScaleModifer;
        var height = (rectangle.Max.y - rectangle.Min.y) * ZScaleModifer;
        var newRect = Instantiate(_rectPrefab, _rectPrefab.transform.position, _rectPrefab.transform.rotation);
        newRect.transform.localScale = new Vector3(width, YScaleModifer, height);
        newRect.transform.position = new Vector3(xCenter, yCenter);
    }

    private void CreateRectanglesAndPath(IEnumerable<Edge> edges, Vector2 startPoint, Vector2 endPoint)
    {
        foreach (var rect in edges)
        {
            DrawRect(rect.First);
        }
        //использовать восемь соседних точек вместо 4
        var pathFinder = new PathFinderService(true);
        var _path = pathFinder.GetPath(startPoint, endPoint, edges).ToArray();
        
        if(!_path.Any()) return;
        
        _lineRenderer.positionCount = _path.Count();
        var pathInVector3 = _path.Select(v => new Vector3(v.x, v.y)).ToArray();
        _lineRenderer.SetPositions(pathInVector3);
    }

    private void TestRectangleCreator()
    {
        var start = new Vector2(12, 8f);
        var end = new Vector2(2.5f, 3.5f);
        var testData = RectangleData.GetTestData();
        CreateRectanglesAndPath(testData, start, end);
    }
}
