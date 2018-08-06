using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

public class Circle
{
    private GameObject gameObgect;
    private LineRenderer lineRenderer;
    private readonly Settings settings;

    public Circle(Settings _settings)
    {
        settings = _settings;
        gameObgect = new GameObject();
        gameObgect.AddComponent<LineRenderer>();
        lineRenderer = gameObgect.GetComponent<LineRenderer>();
        lineRenderer.positionCount = settings.circleSegments;
        lineRenderer.widthMultiplier = settings.circleRadius * 10f;
    }

    public void Draw(Vector2 point)
    {
        float x;
        float y;
        float angle = 0;

        var pts = new Vector3[settings.circleSegments];
        for (var i = 0; i < (settings.circleSegments); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * settings.circleRadius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * settings.circleRadius;
            angle += (720f / settings.circleSegments);

            pts[i] = new Vector3(x + point.x, y + point.y, 0.0f);
        }
        lineRenderer.SetPositions(pts);
    }
}

public class Trajectory
{
    private GameObject gameObject;
    private LineRenderer lineRenderer;
    private readonly Settings settings;

    public Trajectory(Settings _settings)
    {
        settings = _settings;
        gameObject = new GameObject();
        gameObject.AddComponent<LineRenderer>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = settings.trajectoryWidth;
    }

    public void Draw(List<Vector2> points)
    {
        if(points.Count > 1)
        {
            lineRenderer.positionCount = points.Count;
            var pts = new Vector3[points.Count];
            for (var i = 0; i < points.Count; i++)
            {
                pts[i] = new Vector3(points[i].x, points[i].y, 0.0f);
            }
            lineRenderer.SetPositions(pts);
        }
    }
}

[Serializable]
public class Settings
{
    public float moveSpeed;
    public float moveSpeedDelta;
    public float turnDelta;
    public float circleRadius;
    public int circleSegments;
    public float trajectoryWidth;

    // Set of default values
    public Settings()
    {
        moveSpeedDelta = 4f;
        moveSpeed = 0;
        turnDelta = 0.01f;
        circleRadius = 0.04f;
        circleSegments = 100;
        trajectoryWidth = 0.02f;
    }
}

// Main object to control all others
public class Dispatcher : ITickable
{
    private Circle circle;
    private Trajectory trajectory;
    private Settings settings;
    private List<Vector2> points;

    public Dispatcher(Settings _settings, Circle _circle, Trajectory _trajectory)
    {
        settings = _settings;
        circle = _circle;
        trajectory = _trajectory;
        points = new List<Vector2>();
        points.Add(Vector2.zero);
    }

    public void Tick()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ProcessClick(new Vector2(worldPoint.x, worldPoint.y));
        }

        ProcessUpdate();
    }

    private void ProcessClick(Vector2 newPoint)
    {
        points.Add(newPoint);
    }

    public void ProcessSpeedChange(float value)
    {
        settings.moveSpeed = value * settings.moveSpeedDelta;
    }

    private void ProcessUpdate()
    {
        if (points.Count > 1)
        {
            ProcessMoving();
            trajectory.Draw(points);
        }

        circle.Draw(points[0]);
    }

    private void ProcessMoving()
    {
        if (points.Count > 1)
        {
            points[0] = Vector2.MoveTowards(points[0], points[1], settings.moveSpeed * Time.deltaTime);
            Vector2 dir = points[1] - points[0];
            if (dir.magnitude < settings.turnDelta)
            {
                points.RemoveAt(0);
            }
        }
    }
}

public class Worker : MonoBehaviour
{
    private Dispatcher disparcher;
    public Slider slider;

    [Inject]
    public void Construct(Dispatcher _disparcher)
    {
        disparcher = _disparcher;
    }

    void Start()
    {
        disparcher.ProcessSpeedChange(slider.value);
    }

    public void OnSliderValueChange()
    {
        disparcher.ProcessSpeedChange(slider.value);
    }
}