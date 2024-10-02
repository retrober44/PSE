using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : ManagerComponent
{
    // Caching main camara, Camera.main is expensive. (== GameObject.Find())
    private Camera mainCamera;

    private TurnManager _turnManager;

    // Start is called before the first frame update
    public override void Init()
    {
        mainCamera = Camera.main;
        _turnManager = (TurnManager) GameManager.getInstance().getManager(GameManager.SubManagerTypes.TURN);
    }

    /// <summary>
    /// Waits for both touches (only the first touch is recognized by design!)
    /// or mouse clicks. Then delegates their screen positions to handleTouchEvent.
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Debug.Log("[InputManager] Touch event occured.");
                handleTouchEvent(Input.GetTouch(0).position);
            }
        }
        
        // Fallback: Mouse Input, primary button.
        // Will be used in editor, as that does not emulate touch.
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("[InputManager] Mouse button down event occured.");
            handleTouchEvent(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
    }

    /// <summary>
    /// Casts a ray at the world position of a player touch.
    /// If the ray hits an IClickable and it's the local player's turn, IClickable.onClick() is called.
    /// </summary>
    /// <param name="touchPosition">Vector2</param>
    private void handleTouchEvent(Vector2 touchPosition)
    {
        Vector2 worldTouchPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        // Debug.Log("[InputManager] World touch position: " + worldTouchPosition);
        RaycastHit2D raycastHit = Physics2D.Raycast(worldTouchPosition, Vector2.zero);
        if (raycastHit.collider != null)
        {
            IClickable touchedClickable = raycastHit.collider.gameObject.GetComponent<IClickable>();
            if (touchedClickable != null)
            {
                // Simple check. Assumes that no non-UI GameObject is clickable if it is not the local player's turn.
                if (_turnManager.IsLocalPlayersTurn())
                {
                    touchedClickable.onClick();
                }
                else
                {
                    Debug.Log("[InputManager]: It is not local player's turn, so valid input was ignored.");
                }
            }
        }
    }
}
