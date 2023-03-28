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

    public List<Transform> PathPoints;
    private SerializedProperty pathPointsProp;
    private SerializedProperty isInsertProp;
    private SerializedProperty increaseInstertIndex;
    private Transform parent;
    private SerializedObject serializedObject;
    private PathCreatorController pathCreatorController;
    private int insertIndex;
    public bool IsInsert;
    public bool IncreaseInsertIndex;

    private PathManager pathManager;
    public GameObject TemplateBase;
    
    private void OnEnable()
    {
        serializedObject ??= new SerializedObject(this);
        pathCreatorController ??= new PathCreatorController();

        pathPointsProp = serializedObject.FindProperty("PathPoints");
        isInsertProp = serializedObject.FindProperty("IsInsert");
        increaseInstertIndex = serializedObject.FindProperty("IncreaseInsertIndex");
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
        
        EditorGUILayout.PropertyField(pathPointsProp);
        
        GUILayout.Space(15);

        parent = (Transform)EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true);
        
        GUILayout.Space(15);

        EditorGUILayout.PropertyField(isInsertProp);
        
        if(IsInsert)
        {
            GUILayout.Space(15);
            GUILayout.Label("Press 'I' to Insert Cube.", centeredStyle);
            EditorGUILayout.PropertyField(increaseInstertIndex);
            insertIndex = EditorGUILayout.IntField("Insert Index", insertIndex);
        }
        
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Reverse"))
        {
            Reverse();    
        }
        
        if (GUILayout.Button("Draw Path"))
        {
            DrawPath();
        }
        
        if (GUILayout.Button("Clear"))
        {
            Clear();
        }

        if (GUILayout.Button("Destroy Cubes"))
        {
            Transform[] cubes = parent.GetComponentsInChildren<Transform>();

            foreach (Transform transform in cubes)
            {
                if(transform == parent)
                    continue;
                
                DestroyImmediate(transform.gameObject);
            }
            
            Clear();
        }
        
        Repaint();
        serializedObject.ApplyModifiedProperties();
    }

    private void Clear()
    {
        insertIndex = 0;
        PathPoints.Clear();
        pathManager.ClearPath();
    }

    private void SceneGUI(SceneView sceneView)
    {
        if(Event.current.type == EventType.KeyDown && Event.current.character == 'p')
        {
           SpawnCube();
        }
        
        if(Event.current.type == EventType.KeyDown && (Event.current.character == 'i' || Event.current.character == 'ı'))
        {
            InsertCube(insertIndex);
        }
        
        
        Repaint();
    }
    
    private void Reverse()
    {
        if (PathPoints.Count == 0)
            return;
        
        PathPoints.Reverse();
        
        DrawPath();

        serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawPath()
    {
        pathManager.PathPoints = PathPoints;
        pathManager.GeneratePath();
    }

    private void InsertCube(int index)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject cube = pathCreatorController.SpawnPathPoint();

            cube.transform.position = hit.point;
            cube.transform.parent = parent;
            PathPoints.Insert(index, cube.transform);
        }

        insertIndex = PathPoints.Count;
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
    
    private void SpawnCube()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject cube = pathCreatorController.SpawnPathPoint();

            cube.gameObject.name = "Cube" + "_" + PathPoints.Count;
            cube.transform.position = hit.point;
            cube.transform.parent = parent;
            PathPoints.Add(cube.transform);
        }

        insertIndex = PathPoints.Count;
        
        DrawPath();
    }
}
