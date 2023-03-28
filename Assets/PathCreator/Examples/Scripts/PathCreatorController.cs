using UnityEngine;

public class PathCreatorController
{
    public GameObject SpawnPathPoint()
    {
        return GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
}