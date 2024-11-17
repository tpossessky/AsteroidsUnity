using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ScreenBounds : MonoBehaviour {
    [SerializeField] private Camera mainCamera;
    private BoxCollider2D boxCollider;
    public UnityEvent<Collider2D> ExitTriggerFired;
    [SerializeField] private float teleportOffset = 0.2f;
    [SerializeField] private float cornerOffset = 1;
    
    private void Awake() 
    {
        mainCamera.transform.localScale = Vector3.one;
        
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true; 
    }

    private void Start()
    {
        transform.position = Vector3.zero;
        UpdateBoundsSize();
    }

    private void UpdateBoundsSize() 
    {
        var ySize = mainCamera.orthographicSize * 2;
        var boxColliderSize = new Vector2(ySize * mainCamera.aspect, ySize);
        boxCollider.size = boxColliderSize;
    }


    private void OnTriggerExit2D(Collider2D other) 
    {
        ExitTriggerFired?.Invoke(other);
    }

    public bool IsPositionOutOfBounds(Vector3 position) 
    {
        return Math.Abs(position.x) > Math.Abs(boxCollider.bounds.min.x) ||
               Math.Abs(position.y) > Math.Abs(boxCollider.bounds.min.y);
    }
    
    public Vector2 CalculateWrappedPosition(Vector2 worldPosition)
    {
        var xBoundResult = 
            Mathf.Abs(worldPosition.x) > Mathf.Abs(boxCollider.bounds.min.x) - cornerOffset;
        var yBoundResult = 
            Mathf.Abs(worldPosition.y) > Mathf.Abs(boxCollider.bounds.min.y) - cornerOffset;

        var signWorldPosition = 
            new Vector2(Mathf.Sign(worldPosition.x), Mathf.Sign(worldPosition.y));

        if (xBoundResult && yBoundResult)
        {
            return Vector2.Scale(worldPosition, Vector2.one * -1) 
                   + Vector2.Scale(new Vector2(teleportOffset, teleportOffset), 
                       signWorldPosition);
        }
        if (xBoundResult)
        {
            return new Vector2(worldPosition.x * -1, worldPosition.y) 
                   + new Vector2(teleportOffset * signWorldPosition.x, teleportOffset);
        }
        if (yBoundResult)
        {
            return new Vector2(worldPosition.x, worldPosition.y * -1) 
                   + new Vector2(teleportOffset, teleportOffset * signWorldPosition.y);
        }

        return worldPosition;
    }
}
