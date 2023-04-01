using System.Collections.Generic;
using UnityEngine;

public class PathCreatorController
{
    private readonly PathManager pathManager;

    private Transform parent;

    private List<Transform> PathPoints;

    public PathCreatorController(PathManager pathManager, Transform parent, List<Transform> pathPoints)
    {
        this.PathPoints = pathPoints;
        this.parent = parent;
        this.pathManager = pathManager;
    }

    public void DrawPath()
    {
        pathManager.PathPoints = PathPoints;
        pathManager.GeneratePath();
    }

    public void InsertCube(Ray sceneRay, int index)
    {
        GameObject cube = SpawnCube(sceneRay);

        if (cube == null)
            return;

        PathPoints.Insert(index, cube.transform);

        DrawPath();
        ReOrderCubes();
    }

    public void AddCube(Ray sceneRay)
    {
        GameObject cube = SpawnCube(sceneRay);

        if (cube == null)
            return;

        PathPoints.Add(cube.transform);

        DrawPath();
        ReOrderCubes();
    }

    private void ReOrderCubes()
    {
        for (var i = 0; i < PathPoints.Count; i++)
        {
            PathPoints[i].transform.SetSiblingIndex(i);

            PathPoints[i].gameObject.name = "Cube" + "_" + i;
        }
    }


    private GameObject SpawnCube(Ray sceneRay)
    {
        if (!Physics.Raycast(sceneRay, out RaycastHit hit, Mathf.Infinity))
            return null;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        cube.transform.position = hit.point;
        cube.transform.parent = parent;

        DrawPath();

        return cube;
    }

    public void Reverse()
    {
        if (PathPoints.Count == 0)
            return;

        PathPoints.Reverse();

        DrawPath();
    }

    public void Clear()
    {
        PathPoints.Clear();
        pathManager.ClearPath();
    }
}