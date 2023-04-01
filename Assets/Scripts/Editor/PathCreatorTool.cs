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

    public List<Transform> PathPoints = new();
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
        CreateTemplateBase();
        pathCreatorController ??= new PathCreatorController(pathManager, parent, PathPoints);

        pathPointsProp = serializedObject.FindProperty("PathPoints");
        isInsertProp = serializedObject.FindProperty("IsInsert");
        increaseInstertIndex = serializedObject.FindProperty("IncreaseInsertIndex");
        SceneView.duringSceneGui += SceneGUI; 
        
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
            pathCreatorController.DrawPath();
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
        pathCreatorController.Clear();
    }

    private void SceneGUI(SceneView sceneView)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        
        if(Event.current.type == EventType.KeyDown && Event.current.character == 'p')
        {
            pathCreatorController.AddCube(ray);
        }
        
        if(Event.current.type == EventType.KeyDown && Event.current.character is 'i' or 'ı')
        {
            pathCreatorController.InsertCube(ray, insertIndex);
        }
        
        Repaint();
    }
    
    private void Reverse()
    {
        pathCreatorController.Reverse();

        serializedObject.ApplyModifiedProperties();
    }
}
