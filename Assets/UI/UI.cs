using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTS;

public class UI : MonoBehaviour {
    public GUISkin selectBoxSkin;
    public Player player;

    public Texture2D activeCursor;
    public Texture2D idleCursor, idleCursorSelect, panDownCursor, panLeftCursor, panRightCursor, panUpCursor;
    public Texture2D[] moveCursors, attackCursors, pickUpCursors, putDownCursors;

    public Canvas canvas;
    public RectTransform passiveElements;
    public RectTransform selectBox;
    public Text scoreText;
    public Text timeText;
    public Vector3 selectBoxScale = new Vector2(0, 0);
    public Vector2 selectBoxPos = Vector2.zero;

    private const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40, SELECTION_NAME_HEIGHT = 15;
    private Vector2 hotSpot;
    private CursorState activeCursorState;
    private int currentFrame = 0;
    private GameManager manager;

    void Awake() {
        canvas = GetComponentInChildren<Canvas>();
    }

	// Use this for initialization
	void Start () {
        manager = GetComponentInParent<GameManager>();
        player = transform.root.GetComponent<Player>();
        ResourceManager.StoreSelectBoxItems(selectBoxSkin);
        SetCursorState(CursorState.Idle);
	}

	void OnGUI () {

		if(player && player.human) {
            UpdateUI();
            UpdateSelectionBox();
            DrawMouseCursor();
        }
	}

    private void UpdateUI() {
        //Update timer
        string minutes = Mathf.Floor(manager.remainingTime / 60).ToString("00");
        string seconds = Mathf.Floor(manager.remainingTime % 60).ToString("00");
        timeText.text = minutes + ":" + seconds;
    }

    private void UpdateSelectionBox() {
        selectBox.sizeDelta = selectBoxScale;
        selectBox.anchoredPosition = selectBoxPos;
    }

    private void DrawMouseCursor() {
        //Mouse definitely is not on hud if it is panning the lower, or left edge.
        bool mouseOverHud = !MouseInBounds() && activeCursorState != CursorState.PanRight && activeCursorState != CursorState.PanUp;
        if (mouseOverHud) {
            Cursor.SetCursor(idleCursor, hotSpot, CursorMode.Auto);
        } else {
            UpdateCursorAnimation();
            Cursor.SetCursor(activeCursor, hotSpot, CursorMode.Auto);
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
                hotSpot = Vector2.zero;
                break;
            case CursorState.Select:
                activeCursor = idleCursorSelect;
                hotSpot = Vector2.zero;
                break;
            case CursorState.Attack:
                currentFrame = (int)Time.time % attackCursors.Length;
                activeCursor = attackCursors[currentFrame];
                hotSpot = new Vector2(activeCursor.width / 2, activeCursor.height / 2);
                break;
            case CursorState.PickUp:
                currentFrame = (int)Time.time % pickUpCursors.Length;
                activeCursor = pickUpCursors[currentFrame];
                hotSpot = new Vector2(activeCursor.width / 2, activeCursor.height / 2);
                break;
            case CursorState.Move:
                currentFrame = (int)Time.time % moveCursors.Length;
                activeCursor = moveCursors[currentFrame];
                hotSpot = new Vector2(activeCursor.width / 2, activeCursor.height / 2);
                break;
            case CursorState.PanLeft:
                activeCursor = panLeftCursor;
                hotSpot = new Vector2(0, activeCursor.height / 2);
                break;
            case CursorState.PanRight:
                activeCursor = panRightCursor;
                hotSpot = new Vector2(activeCursor.width, activeCursor.height / 2);
                break;
            case CursorState.PanUp:
                activeCursor = panUpCursor;
                hotSpot = new Vector2(activeCursor.width / 2, 0);
                break;
            case CursorState.PanDown:
                activeCursor = panDownCursor;
                hotSpot = new Vector2(activeCursor.width / 2, activeCursor.height);
                break;
            case CursorState.PutDown:
                currentFrame = (int)Time.time % pickUpCursors.Length;
                activeCursor = putDownCursors[currentFrame];
                hotSpot = new Vector2(activeCursor.width / 2, activeCursor.height / 2);
                break;
            default:
                break;
        }
    }

    public void SetDefaultHoverState(WorldObject worldObject) {
        Player owner = worldObject.transform.root.GetComponent<Player>();
        if (owner) {
            if (owner.username == player.username) {
                player.ui.SetCursorState(CursorState.Select);
            }
        }
    }

}
