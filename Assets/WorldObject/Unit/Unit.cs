using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using UnityEngine.AI;

public class Unit : WorldObject {
    public float pickUpDistance;

    private NavMeshAgent agent;
    private bool carrying = false;
    private Order currentOrder = Order.NONE;
    private WorldObject target;

    protected override void Awake() {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        switch (currentOrder) {
            case Order.PICK_UP:
                if(DistanceToTarget() <= pickUpDistance) {
                    print("UNIT HAS REACHED THE ORE!");
                }
                break;
            default:
                break;
        }
	}

    protected override void OnGUI() {
        base.OnGUI();
        if(player && player.human && currentlySelected) {
            CalculateBounds();
        }
    }

    protected virtual float DistanceToTarget() {
        return Vector3.Distance(gameObject.transform.position, target.gameObject.transform.position);
    }

    public override void SetHoverState(GameObject hoverObject) {
        base.SetHoverState(hoverObject);
        if(player && player.human && currentlySelected) {
            if(hoverObject.name == "Ground") {
                player.ui.SetCursorState(CursorState.Move);
            } else {
                Ore ore = hoverObject.transform.parent.GetComponent<Ore>();
                if (ore && ore.Uncarried()) {
                    player.ui.SetCursorState(CursorState.PickUp);
                }
            }
        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        base.MouseClick(hitObject, hitPoint, controller);
        //Only handle for human players, if player is owner and this unit is currently selected
        if(player && player.human && currentlySelected) {
            if(hitObject.name == "Ground" && hitPoint != ResourceManager.InvalidPosition) {
                float x = hitPoint.x;
                //Unit y is actually in the middle of the unit, this has to be added to the zero-level or unit will sink into the ground...
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;
                Vector3 destination = new Vector3(x, y, z);
                StartMove(destination);
            }
        }
    }

    public override void RightClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        base.RightClick(hitObject, hitPoint, controller);
        Ore ore = hitObject.transform.parent.GetComponent<Ore>();
        if(ore && ore.Uncarried()) {
            BeginPickUp(ore);
        }
    }

    protected override void RightClickGround(Vector3 hitPoint) {
        base.RightClickGround(hitPoint);
        //Units should move to the selected location when having the ground right-clicked.
        StartMove(hitPoint);
    }

    private void StartMove(Vector3 destination) {
        agent.SetDestination(destination);
    }

    /**
     * Unit is ordered to to pick upp the chosen ore object
     */
    private void BeginPickUp(Ore ore) {
        currentOrder = Order.PICK_UP;
        target = ore;
        agent.SetDestination(ore.gameObject.transform.position);
    }
}
