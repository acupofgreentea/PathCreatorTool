using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathCreatorTool : EditorWindow
{
    [MenuItem("Tools/PathCreator")]
    public static void OpenWindow()
    {
        GetWindow<PathCreatorTool>(false, "Path Creator");
    }

    public List<Transform> PathPoints = new List<Transform>();
    public SerializedProperty PathPointsProp;
    private Transform parent;
    private SerializedObject serializedObject;
    private PathCreatorController pathCreatorController;
    
    private void OnEnable()
    {
        serializedObject ??= new SerializedObject(this);
        pathCreatorController ??= new PathCreatorController();

        PathPointsProp = serializedObject.FindProperty("PathPoints");
        SceneView.duringSceneGui += SceneGUI; 
    }

    private void OnGUI()
    {
        serializedObject.Update();
        parent = (Transform)EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true);
        EditorGUILayout.PropertyField(PathPointsProp);

        serializedObject.ApplyModifiedProperties();
        
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Clear"))
        {
            if (parent == null)
            {
                Debug.LogError("Parent is not assigned!");
            }
            
            PathPoints.Clear();
        }

        if (GUILayout.Button("Destroy Cubes"))
        {
            List<Transform> cubes = PathPoints;

            foreach (Transform transform in cubes)
            {
                DestroyImmediate(transform.gameObject);
            }
            
            PathPoints.Clear();
        }
    }

    private void SceneGUI(SceneView sceneView)
    {
        if(Event.current.type == EventType.KeyDown && Event.current.character == 'p')
        {
           SpawnCube();
        }
    }

    private void SpawnCube()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject cube = pathCreatorController.SpawnPathPoint();
            cube.transform.position = hit.point;
            cube.transform.parent = parent;
            PathPoints.Add(cube.transform);
        }
    }
}
