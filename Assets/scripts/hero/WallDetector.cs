using UnityEngine;

public class WallDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform _detectionPoint;
    [SerializeField] private float _detectionRadius = 0.1f;
    [SerializeField] private LayerMask _WallLayerMask;
    private float radius = 0.1f;

    private void Update()
    {
        if (radius != 0.1f)
        {
            Debug.Log(radius);
        }
    }

    public bool DetectWallNearBy()
    {

        RaycastHit2D hitResult = Physics2D.CircleCast(
            _detectionPoint.position, 
            _detectionRadius, 
            Vector2.zero, 
            0, 
            _WallLayerMask
            );

        if (hitResult.collider != null)
        {
            return true;
        }
        
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_detectionPoint.position, _detectionRadius);
    }
}
