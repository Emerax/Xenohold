﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using UnityEngine.AI;

public class Unit : WorldObject {
    public float pickUpDistance;
    public float healthBarHoverDistance; //How far above the unit thea healthbar should be rendered.
    public GameObject healthBarObjectPrefab;
    public int maxHealth, currentHealth;

    private NavMeshAgent agent;
    private bool carrying = false;
    private Order currentOrder = Order.NONE;
    private WorldObject target;
    private HealthBar healthBar;

    protected override void Awake() {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        agent = GetComponent<NavMeshAgent>();

        //Initiate the unit's health bar.
        Transform parent = GetOwner().ui.canvas.gameObject.transform.Find("HealthBarRect");
        GameObject newBar = Instantiate(healthBarObjectPrefab, parent);
        newBar.GetComponent<HealthBar>().unit = this;
        healthBar = newBar.GetComponent<HealthBar>();
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        switch (currentOrder) {
            case Order.PICK_UP:
                if(target is Ore && !(target as Ore).carrier) {
                    if(DistanceToTarget() <= pickUpDistance) {
                        PickUp(target as Ore);
                        ClearOrder();
                    }
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

    /**
     * Places ore on top of units head, and attaches it to units transform, making it follow.
     */
    protected virtual void PickUp(Ore ore) {
        if (carrying) {
            Drop();
        }
        Vector3 uPos = transform.position;
        Vector3 attachPos = new Vector3(uPos.x, transform.localScale.y + ore.transform.localScale.y / 2, uPos.z);
        ore.transform.position = attachPos;
        ore.transform.parent = transform;

        ore.OnPickUp(this);
        carrying = true;
    }

    protected void Drop() {
        Ore ore = transform.GetComponentInChildren<Ore>();
        ore.transform.parent = null;
        Vector3 downPos = new Vector3(transform.position.x, 0, transform.position.z);
        ore.transform.position = downPos;
        ore.OnDrop();
        carrying = false;
    }

    protected virtual void ClearOrder() {
        target = null;
        currentOrder = Order.NONE;
    }

    public virtual Vector3 GetHealthBarPos() {
        Vector3 pos = transform.position;
        pos.y += (transform.localScale.y + healthBarHoverDistance);
        return pos;
    }

    public override void SetHoverState(WorldObject worldObject) {
        base.SetHoverState(worldObject);
        if(worldObject is Ore && !(worldObject as Ore).carrier) {
            player.ui.SetCursorState(CursorState.PickUp);
        } else if (worldObject is Unit && !(worldObject as Unit).GetOwner().Equals(GetOwner())) {
            player.ui.SetCursorState(CursorState.Attack);
        }
    }

    public override void SetGroundHoverState() {
        base.SetGroundHoverState();
        player.ui.SetCursorState(CursorState.Move);
    }

    public override void RightClickObject(WorldObject worldObject) {
        base.RightClickObject(worldObject);
        if(worldObject is Ore && !(worldObject as Ore).carrier) {
            BeginPickUp(worldObject as Ore);
        }
    }

    public override void RightClickGround(Vector3 hitPoint) {
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
