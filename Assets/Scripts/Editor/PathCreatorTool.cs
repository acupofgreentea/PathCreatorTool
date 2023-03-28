using System;
using System.Collections.Generic;
using PathCreation;
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
    private SerializedProperty PathPointsProp;
    private SerializedProperty TemplateBaseProp;
    private Transform parent;
    private SerializedObject serializedObject;
    private PathCreatorController pathCreatorController;

    private PathManager pathManager;
    public GameObject TemplateBase;
    
    private void OnEnable()
    {
        serializedObject ??= new SerializedObject(this);
        pathCreatorController ??= new PathCreatorController();

        PathPointsProp = serializedObject.FindProperty("PathPoints");
        TemplateBaseProp = serializedObject.FindProperty("TemplateBase");
        SceneView.duringSceneGui += SceneGUI; 
        
        CreateTemplateBase();
    }

    private void CreateTemplateBase()
    {
        if (TemplateBase != null)
            return;
        
        GameObject template = new("Template");
        GameObject cubesParent = new("CubesParent");
        GameObject path = new("Path");
        
        path.AddComponent<PathCreator>();
        pathManager = path.AddComponent<PathManager>();

        TemplateBase = template;
        cubesParent.transform.parent = template.transform;
        path.transform.parent = template.transform;

        parent = cubesParent.transform;
        serializedObject.ApplyModifiedProperties();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= SceneGUI;
    }

    private void OnGUI()
    {
        serializedObject.Update();
        
        GUILayout.Space(15);
        
        GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        
        GUILayout.Label("Press 'P' to Create Cube.", centeredStyle);
        
        GUILayout.Space(15);
        
        EditorGUILayout.PropertyField(PathPointsProp);
        
        GUILayout.Space(15);

        parent = (Transform)EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true);
        
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Clear"))
        {
            if (parent == null)
            {
                Debug.LogError("Parent is not assigned!");
                return;
            }
            
            PathPoints.Clear();
            pathManager.ClearPath();
        }

        if (GUILayout.Button("Destroy Cubes"))
        {
            List<Transform> cubes = PathPoints;

            foreach (Transform transform in cubes)
            {
                DestroyImmediate(transform.gameObject);
            }
            
            PathPoints.Clear();
            pathManager.ClearPath();
        }
        
        
        serializedObject.ApplyModifiedProperties();
    }

    private void SceneGUI(SceneView sceneView)
    {
        if(Event.current.type == EventType.KeyDown && Event.current.character == 'p')
        {
           SpawnCube();
        }
        
        Repaint();
    }

    private void DrawPath()
    {
        pathManager.PathPoints = PathPoints;
        pathManager.GeneratePath();
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
        
        DrawPath();
    }
}
