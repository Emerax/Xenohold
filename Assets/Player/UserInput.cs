using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class UserInput : MonoBehaviour {

    public float panWidth = 15;
    public float panSpeed = 25;
    public bool panning = false;
    public float rotateAmount = 10;
    public float rotateSpeed = 100;
    public float minCameraHeight = 10;
    public float maxCameraHeight = 40;

    private Player player;

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
        }else if((xpos <= Screen.width && xpos > Screen.width - panWidth) || Input.GetKey(KeyCode.D)) {
            movement.x += panSpeed;
            player.ui.SetCursorState(CursorState.PanRight);
            panning = true;
        }

        //vertical movement
        if((ypos >= 0 && ypos < panWidth) || Input.GetKey(KeyCode.S)) {
            movement.z -= panSpeed;
            player.ui.SetCursorState(CursorState.PanDown);
            panning = true;
        }else if((ypos <= Screen.height && ypos > Screen.height - panWidth) || Input.GetKey(KeyCode.W)) {
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
            destination.x -= Input.GetAxis("Mouse Y") * rotateAmount;
            destination.y += Input.GetAxis("Mouse X") * rotateAmount;
        }

        if(destination != origin) {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
        }
    }

    private void MouseActivity() {
        if (Input.GetMouseButtonDown(0)) LeftMouseClick();
        else if (Input.GetMouseButtonDown(1)) RightMouseClick();
        MouseHover();
    }

    private void LeftMouseClick() {
        if(player.ui.MouseInBounds()) {
            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();
            if(hitObject && hitPoint != ResourceManager.InvalidPosition) {
                if (player.SelectedObject) {
                    player.SelectedObject.MouseClick(hitObject, hitPoint, player);
                } else if (hitObject.name != "Ground") {
                    WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
                    if (worldObject) {
                        player.SelectedObject = worldObject;
                        worldObject.SetSelection(true, player.ui.GetPlayingArea());
                    }
                }
            }
        }
    }

    private void RightMouseClick() {
        if(player.ui.MouseInBounds() && player.SelectedObject) {
            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();
            if(hitObject && hitPoint != ResourceManager.InvalidPosition) {
                player.SelectedObject.RightClick(hitObject, hitPoint, player);
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
        if (player.ui.MouseInBounds()) {
            GameObject hoverObject = FindHitObject();
            if (hoverObject) {
                if (player.SelectedObject) player.SelectedObject.SetHoverState(hoverObject);
                else if(hoverObject.name != "Ground") {
                    Player owner = hoverObject.transform.root.GetComponent<Player>();
                    if (owner) {
                        Unit unit = hoverObject.transform.parent.GetComponent<Unit>();
                        Building building = hoverObject.transform.parent.GetComponent<Building>();
                        if(owner.username == player.username && (unit || building)) {
                            player.ui.SetCursorState(CursorState.Select);
                        }
                    }
                }
            }
        }
    }
}
