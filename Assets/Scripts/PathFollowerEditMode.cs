using PathCreation;
using UnityEngine;

[ExecuteAlways]
public class PathFollowerEditMode : MonoBehaviour
{
    [SerializeField] private float distanceTravelled;
    [SerializeField] private float speed = 5f;

    [SerializeField] private bool canMove = true;

    [SerializeField] private PathCreator pathCreator;

    private void Update()
    {
        if (pathCreator == null || !canMove)
            return;
        
        distanceTravelled += Time.deltaTime * speed;

        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.forward = pathCreator.path.GetDirectionAtDistance(distanceTravelled);
    }

    void OnDrawGizmos()
    {
        // Your gizmo drawing thing goes here if required...

#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }
}
