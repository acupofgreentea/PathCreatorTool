using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreator;

    [SerializeField] private bool closedLoop = false;

    [SerializeField] private Transform pathPointsParent;

    [SerializeField] private List<Transform> pathPoints;

    [ContextMenu("Generate Path")]
    public void GeneratePath()
    {
        if (pathPoints.Count == 0)
        {
            Debug.LogError("Path points is empty!");
            return;
        }

        if (pathCreator == null)
            pathCreator = GetComponent<PathCreator>();
        
        BezierPath bezierPath = new(pathPoints, closedLoop, PathSpace.xyz)
        {
            GlobalNormalsAngle = 90f,
            ControlPointMode = BezierPath.ControlMode.Automatic
        };
        
        pathCreator.bezierPath = bezierPath;
    }

    [ContextMenu("Fill Path Points")]
    private void FillPathPoints()
    {
        pathPoints.Clear();
        
        foreach (Transform point in pathPointsParent)
        {
            pathPoints.Add(point);
        }
    }

    public Vector3 GetPointAtTime(float timeProgress, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop) =>
        pathCreator.path.GetPointAtTime(timeProgress, endOfPathInstruction);

    public Vector3 GetDirection(float timeProgress, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop) =>
        pathCreator.path.GetDirection(timeProgress, endOfPathInstruction);

    public Quaternion GetRotationAtTime(float timeProgress, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop) =>
        pathCreator.path.GetRotation(timeProgress, endOfPathInstruction);

    public float GetClosestTimeOnPath(Vector3 point) => pathCreator.path.GetClosestTimeOnPath(point);
    public Vector3 GetClosestPointOnPath(Vector3 point) => pathCreator.path.GetClosestPointOnPath(point);
    public float GetClosestDistanceAlongPath(Vector3 point) => pathCreator.path.GetClosestDistanceAlongPath(point);
    public Vector3 GetPathRight(float timeProgress) => Vector3.Cross(GetDirection(timeProgress), Vector3.up);
    public float GetLength => pathCreator.path.length;
}
