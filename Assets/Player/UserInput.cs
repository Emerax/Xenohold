using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {

    public float scrollWidth = 15;
    public float scrollSpeed = 25;
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
        }
	}

    private void MoveCamera() {
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        //horizontal movement
        if(xpos >= 0 && xpos < scrollWidth) {
            movement.x -= scrollSpeed;
        }else if(xpos <= Screen.width && xpos > Screen.width - scrollWidth) {
            movement.x += scrollSpeed;
        }

        //vertical movement
        if(ypos >= 0 && ypos < scrollWidth) {
            movement.z -= scrollSpeed;
        }else if(ypos <= Screen.height && ypos > Screen.height - scrollWidth) {
            movement.z += scrollSpeed;
        }

        //ensure camera movement is perpendicular to the ground rather than camera tilt.
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        //zoom
        movement.y -= scrollSpeed * Input.GetAxis("Mouse ScrollWheel");

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
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * scrollSpeed);
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
}
