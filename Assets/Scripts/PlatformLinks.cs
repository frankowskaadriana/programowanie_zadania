// Optional helper script to automatically create links
using UnityEngine;
using UnityEngine.AI;

public class PlatformLink : MonoBehaviour
{
    [Header("Jump Settings")]
    public Transform targetPlatform;
    public float jumpHeight = 2f;
    public float jumpDistance = 3f;

    void Start()
    {
        // Create a link between this platform and target
        CreateNavMeshLink();
    }

    void CreateNavMeshLink()
    {
        // Find or create an OffMeshLink component
        OffMeshLink link = GetComponent<OffMeshLink>();
        if (link == null)
            link = gameObject.AddComponent<OffMeshLink>();

        // Set up the link
        link.startTransform = transform;
        link.endTransform = targetPlatform;
        link.autoUpdatePositions = true;
        link.biDirectional = true;

        // Adjust link type and properties
        link.area = 0; // Walkable area
        link.costOverride = -1; // Default cost

        // For jumps, you can adjust the link type
        // OffMeshLinkType.LinkTypeManual for custom jumps
    }

    // Optional: Visualize the link in editor
    void OnDrawGizmos()
    {
        if (targetPlatform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPlatform.position);
            Gizmos.DrawWireSphere(transform.position, 0.3f);
            Gizmos.DrawWireSphere(targetPlatform.position, 0.3f);
        }
    }
}