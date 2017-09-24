using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class UI : MonoBehaviour {
    public GUISkin resourceSkin, ordersSkin, selectBoxSkin;
    public Player player;

    public Texture2D activeCursor;
    public Texture2D idleCursor, idleCursorSelect, panDownCursor, panLeftCursor, panRightCursor, panUpCursor;
    public Texture2D[] moveCursors, attackCursors, pickUpCursors;

    private const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40, SELECTION_NAME_HEIGHT = 15;
    private Vector2 zv = new Vector2(0, 0);
    private CursorState activeCursorState;
    private int currentFrame = 0;

	// Use this for initialization
	void Start () {
        player = transform.root.GetComponent<Player>();
        ResourceManager.StoreSelectBoxItems(selectBoxSkin);
        SetCursorState(CursorState.Idle);
	}

	void OnGUI () {
		if(player && player.human) {
            DrawOrdersBar();
            DrawResourceBar();
            DrawMouseCursor();
        }
	}

    private void DrawOrdersBar() {
        GUI.skin = ordersSkin;
        GUI.BeginGroup(new Rect(Screen.width - ORDERS_BAR_WIDTH, RESOURCE_BAR_HEIGHT, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT), "");

        string selectionName = "";
        if (player.SelectedObject) {
            selectionName = player.SelectedObject.objectName;
        }
        if (!selectionName.Equals("")) {
            GUI.Label(new Rect(0, 10, ORDERS_BAR_WIDTH, SELECTION_NAME_HEIGHT), selectionName);
        }
        GUI.EndGroup();
    }

    private void DrawResourceBar() {
        GUI.skin = resourceSkin;
        GUI.BeginGroup(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT), "");
        GUI.EndGroup();
    }

    private void DrawMouseCursor() {
        //Mouse definitely is not on hud if it is panning the lower, or left edge.
        bool mouseOverHud = !MouseInBounds() && activeCursorState != CursorState.PanRight && activeCursorState != CursorState.PanUp;
        if (mouseOverHud) {
            Cursor.SetCursor(idleCursor, zv, CursorMode.Auto);
        } else {
            UpdateCursorAnimation();
            Cursor.SetCursor(activeCursor, zv, CursorMode.Auto);
        }
    }

    private void UpdateCursorAnimation() {
        if(activeCursorState == CursorState.Move) {
            currentFrame = (int)Time.time % moveCursors.Length;
            activeCursor = moveCursors[currentFrame];
        }else if(activeCursorState == CursorState.Attack) {
            currentFrame = (int)Time.time % attackCursors.Length;
            activeCursor = attackCursors[currentFrame];
        }else if(activeCursorState == CursorState.PickUp) {
            currentFrame = (int)Time.time % pickUpCursors.Length;
            activeCursor = pickUpCursors[currentFrame];
        }
    }

    public bool MouseInBounds() {
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width - ORDERS_BAR_WIDTH;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
        return insideWidth && insideHeight;
    }

    public Rect GetPlayingArea() {
        return new Rect(0, RESOURCE_BAR_HEIGHT, Screen.width - ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT);
    }

    public void SetCursorState(CursorState newState) {
        activeCursorState = newState;
            switch (newState) {
            case CursorState.Idle:
                activeCursor = idleCursor;
                break;
            case CursorState.Select:
                activeCursor = idleCursorSelect;
                break;
            case CursorState.Attack:
                currentFrame = (int)Time.time % attackCursors.Length;
                activeCursor = attackCursors[currentFrame];
                break;
            case CursorState.PickUp:
                currentFrame = (int)Time.time % pickUpCursors.Length;
                activeCursor = pickUpCursors[currentFrame];
                break;
            case CursorState.Move:
                currentFrame = (int)Time.time % moveCursors.Length;
                activeCursor = moveCursors[currentFrame];
                break;
            case CursorState.PanLeft:
                activeCursor = panLeftCursor;
                break;
            case CursorState.PanRight:
                activeCursor = panRightCursor;
                break;
            case CursorState.PanUp:
                activeCursor = panUpCursor;
                break;
            case CursorState.PanDown:
                activeCursor = panDownCursor;
                break;
            default:
                break;
        }
    }
}
