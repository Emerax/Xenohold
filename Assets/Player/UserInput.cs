using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class UserInput : MonoBehaviour {

    public float panWidth = 30;
    public float panSpeed = 25;
    public float rotateAmount = 10;
    public float rotateSpeed = 100;
    public float minCameraHeight = 10;
    public float maxCameraHeight = 40;

    private Player player;
    private bool panning = false;

    /// <summary>
    /// When the player begins to draw a selection box, this value is the pont where the mouse was first pressed down.
    /// </summary>
    private Vector3 selectPos;
    public bool selecting;

    // Use this for initialization
    void Start () {
        player = transform.root.GetComponent<Player>();
		
	}
	
	// Update is called once per frame
	void Update () {
        if (player.human) {
            MoveCamera();
            RotateCamera();
            MouseActivity();
        }
	}

    private void MoveCamera() {
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);
        panning = false;

        //horizontal movement
        if((xpos >= 0 && xpos < panWidth) || Input.GetKey(KeyCode.A)) {
            movement.x -= panSpeed;
            player.ui.SetCursorState(CursorState.PanLeft);
            panning = true;
        }else if((xpos <= Screen.width && (xpos > Screen.width - panWidth) || Input.GetKey(KeyCode.D))) {
            movement.x += panSpeed;
            player.ui.SetCursorState(CursorState.PanRight);
            panning = true;
        }

        //vertical movement
        if((ypos >= 0 && ypos < panWidth) || Input.GetKey(KeyCode.S)) {
            movement.z -= panSpeed;
            player.ui.SetCursorState(CursorState.PanDown);
            panning = true;
        }else if((ypos <= Screen.height && (ypos > Screen.height - panWidth) || Input.GetKey(KeyCode.W))) {
            movement.z += panSpeed;
            player.ui.SetCursorState(CursorState.PanUp);
            panning = true;
        }

        //ensure camera movement is perpendicular to the ground rather than camera tilt.
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        //zoom
        movement.y -= panSpeed * Input.GetAxis("Mouse ScrollWheel");

        //finally, calculate new camera position
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        //make sure the camera statys within the specified zoom-interval
        if(destination.y > maxCameraHeight) {
            destination.y = maxCameraHeight;
        }else if(destination.y < minCameraHeight) {
            destination.y = minCameraHeight;
        }

        //only update camera if it has actually moved.
        if(destination != origin) {
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * panSpeed);
        }

        if (!panning) {
            player.ui.SetCursorState(CursorState.Idle);
        }
    }

    private void RotateCamera() {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;
        if (Input.GetMouseButton(2)) {
            //destination.x -= Input.GetAxis("Mouse X") * rotateAmount;
            destination.y += Input.GetAxis("Mouse X") * rotateAmount;
        }

        if(destination != origin) {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
        }
    }

    private void MouseActivity() {
        if (selecting) {
            Vector2 pos1, pos2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(player.ui.passiveElements, selectPos, null, out pos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(player.ui.passiveElements, Input.mousePosition, null, out pos2);

            Rect mouseMovementRect = CalculateMouseMovementRect(pos1, pos2);
            player.ui.selectBoxScale = mouseMovementRect.size;
            player.ui.selectBoxPos = mouseMovementRect.center;

            if (Input.GetMouseButtonUp(0)) {
                print("LEFT UP");
                selecting = false;

                if(selectPos != Input.mousePosition) {
                    SelectMultiple();
                }
                //Make select box size zero to effectively remove it while we are not selecting anything.
                player.ui.selectBoxScale = Vector2.zero;
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            print("LEFT DOWN!");
            selectPos = Input.mousePosition;
            selecting = true;

            LeftMouseClick();
        } else if (Input.GetMouseButtonDown(1) && player.selectedUnits.Count > 0) {
            RightMouseClick();
        } else if (Input.GetMouseButtonDown(1)) {
            player.AddUnit(FindHitPoint(), new Quaternion());
        }
        MouseHover();
    }

    /// <summary>
    /// Given two points on the screen, calculates the Rect formed with them as diametrically opposed corners.
    /// </summary>
    private Rect CalculateMouseMovementRect(Vector2 pos1, Vector2 pos2) {
        Vector2 topLeft = Vector2.Min(pos1, pos2);
        Vector2 bottomRight = Vector2.Max(pos1, pos2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    /// <summary>
    /// Find all owned units within the selection box and select them.
    /// </summary>
    private void SelectMultiple() {
        player.SelectUnits(player.ownedUnits.FindAll(u => u.InSelectionBounds(GetViewPortBounds(selectPos, Input.mousePosition))));

    }

    private Bounds GetViewPortBounds(Vector2 screenPos1, Vector2 screenPos2) {
        Vector3 v1 = Camera.main.ScreenToViewportPoint(screenPos1);
        Vector3 v2 = Camera.main.ScreenToViewportPoint(screenPos2);
        Vector3 min = Vector3.Min(v1, v2);
        Vector3 max = Vector3.Max(v1, v2);
        min.z = Camera.main.nearClipPlane;
        max.z = Camera.main.farClipPlane;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    private void LeftMouseClick() {
        if(player.ui.MouseInBounds()) {
            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();
            if(hitObject && hitPoint != ResourceManager.InvalidPosition) {
                if(hitObject.name != "Ground") {
                    WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
                    if(worldObject && worldObject is Ore && (worldObject as Ore).carrier) {
                        worldObject = (worldObject as Ore).carrier;
                    }
                    if (worldObject && worldObject is Unit && worldObject.GetOwner() && worldObject.GetOwner().Equals(player)) {
                        player.SelectUnits(new List<Unit>(new Unit[] {worldObject as Unit})); //Dirty
                    }
                } else {
                    //Deselect everything on left-clicking the ground
                    player.DeselectAll();
                }
            }
        }
    }

    private void RightMouseClick() {
        if(player.ui.MouseInBounds() && player.selectedUnits.Count > 0) {
            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();
            if(hitObject && hitPoint != ResourceManager.InvalidPosition) {
                if(hitObject.name != "Ground") {
                    WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
                    if (worldObject) {
                        foreach(Unit unit in player.selectedUnits) {
                            unit.RightClickObject(worldObject);
                        }
                    } else {
                        Debug.LogError("RightMouseClick received a non-Ground, non-worldObject hitObject: " + hitObject);
                    }
                } else {
                    if (player.selectedUnits.Count > 0) {
                        foreach(Unit unit in player.selectedUnits) {
                            unit.RightClickGround(hitPoint);
                        }
                    } 
                }
            }
        }
    }

    private GameObject FindHitObject() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
        return null;
    }

    private Vector3 FindHitPoint() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) return hit.point;
        return ResourceManager.InvalidPosition;
    }

    private void MouseHover() {
        if (player.human && player.ui.MouseInBounds()) {
            GameObject hoverObject = FindHitObject();
            if (hoverObject) {
                if(hoverObject.name != "Ground") {
                    WorldObject worldObject = hoverObject.transform.parent.GetComponent<WorldObject>();
                    if (worldObject) {
                        if (player.selectedUnits.Count > 0) {
                            //First unit in selected list is leader, hopefully this leader is chosen in a sensible manner.
                            player.selectedUnits[0].SetHoverState(worldObject);
                        } else {
                            player.ui.SetDefaultHoverState(worldObject);
                        }
                    } else {
                        Debug.LogError("MouseHover received non-Ground, non-WorldObject hoverObject: " + hoverObject);
                    }
                } else {
                    if (player.selectedUnits.Count > 0) {
                        //First unit in selection list decides responses
                        player.selectedUnits[0].SetGroundHoverState();
                    } else {
                        //Hovering above ground with nothign selected
                        player.ui.SetCursorState(CursorState.Idle);
                    }
                }
            }
        }
    }
}
