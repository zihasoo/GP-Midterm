using UnityEngine;

// Attach to a parent GameObject with two child tiles (tileA, tileB). Each tile contains a SpriteRenderer or Mesh.
// The script repositions the far tile when the camera passes a threshold, creating an infinite loop.
public class ParallaxInfiniteBackground : MonoBehaviour
{
    public Transform cameraTransform;
    [Tooltip("0 = static background, 1 = match camera movement")] [Range(0f, 1f)] public float parallaxFactor = 0.5f;
    public float padding = 0.1f; // world units

    [Header("Tiles")]
    public Transform tileA;
    public Transform tileB;

    private Vector3 _startCamPos;
    private Vector3 _startPos;

    private float _tileWidth; // world units
    private Camera _cam;

    private void Start()
    {
        _cam = cameraTransform.GetComponent<Camera>();

        _startPos = transform.position;
        _startCamPos = cameraTransform.position;

        _tileWidth = tileA.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        // Parallax follow
        Vector3 camDelta = cameraTransform.position - _startCamPos;
        Vector3 basePos = _startPos + camDelta * parallaxFactor;
        transform.position = new Vector3(basePos.x, basePos.y, _startPos.z);

        // Infinite wrap along X, trigger before edges enter the camera view
        float camX = cameraTransform.position.x;
        float halfHeight = _cam.orthographicSize;
        float halfWidth = halfHeight * _cam.aspect;
        float camRight = camX + halfWidth;
        float camLeft = camX - halfWidth;

        Transform left = tileA.position.x < tileB.position.x ? tileA : tileB;
        Transform right = left == tileA ? tileB : tileA;

        float rightEdge = right.position.x + _tileWidth * 0.5f;
        float leftEdge = left.position.x - _tileWidth * 0.5f;

        if (camRight > rightEdge - padding)
        {
            left.position = new Vector3(right.position.x + _tileWidth, left.position.y, left.position.z);
        }
        else if (camLeft < leftEdge + padding)
        {
            right.position = new Vector3(left.position.x - _tileWidth, right.position.y, right.position.z);
        }
    }
}
