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
    public int attackDamage;
    public float attackRange;
    public float attackCooldown; //time in seconds how long the unit must wait between each attack.
    /// <summary>
    /// Distance this unit will automatically look for targets.
    /// </summary>
    public float sightRadius;
    /// <summary>
    /// Distance this units call for aid will be heard.
    /// </summary>
    public float callRange;

    protected NavMeshAgent agent;

    protected bool carrying = false;
    protected Order currentOrder = Order.NONE;
    protected WorldObject target;
    protected HealthBar healthBar;
    protected float remainingCooldown = 0;

    private Projector selectionCircle;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        selectionCircle = GetComponentInChildren<Projector>();
        selectionCircle.enabled = false;

        //Initiate the unit's health bar.
        Transform healthBarTransform = transform.root.Find("PassiveElementsRect");
        GameObject newBar = Instantiate(healthBarObjectPrefab, healthBarTransform);
        newBar.GetComponent<HealthBar>().unit = this;
        healthBar = newBar.GetComponent<HealthBar>();
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        //Cooldown attack
        if(remainingCooldown > 0) {
            remainingCooldown -= Time.deltaTime;
        }

        switch (currentOrder) {
            case Order.MOVE:
                if (agent.remainingDistance <= float.Epsilon) {
                    currentOrder = Order.NONE;
                }
                break;
            case Order.PICK_UP:
                if(target is Ore && !(target as Ore).carrier) {
                    if(DistanceToTarget(target) <= pickUpDistance) {
                        PickUp(target as Ore);
                        ClearOrder();
                    } else {
                        agent.SetDestination(target.transform.position);
                    }
                } else {
                    //Target is no longer or or has been picked up, look for other ore in vicinity
                    GetNewTarget(currentOrder);
                }
                break;
            case Order.ATTACK:
                if(target is Unit) {
                    if (target) {
                        if (DistanceToTarget(target) <= attackRange) {
                            if (remainingCooldown <= 0) {
                                remainingCooldown = attackCooldown;
                                Attack(target as Unit);
                            }
                        } else if (agent.destination != target.transform.position) {
                            //Move into attack range
                            agent.SetDestination(target.transform.position);
                        }
                    } else {
                        //Target Unit has died or is null for some other reason, look for new target to attack.
                        GetNewTarget(currentOrder);
                    }
                }
                break;
            case Order.PUT_DOWN:
                if(target is DropOff && carrying) {
                    if(DistanceToTarget(target) < pickUpDistance + 4) {
                        Deliver(target as DropOff);
                        ClearOrder();
                    } else {
                        agent.SetDestination(target.transform.position);
                    }
                }
                break;
            default:
                break;
        }
	}
    
    public void SetSelection(bool selected) {
        currentlySelected = selected;
        selectionCircle.enabled = selected;
    }

    public bool InSelectionBounds(Bounds selectionBounds) {
        return selectionBounds.Contains(Camera.main.WorldToViewportPoint(transform.position));
    }

    protected virtual float DistanceToTarget(WorldObject target) {
        return Vector3.Distance(gameObject.transform.position, target.gameObject.transform.position);
    }

    /// <summary>
    /// Places ore on top of units head, and attaches it to units transform, making it follow.
    /// </summary>
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

    protected virtual void Attack(Unit target) {
        MeleeAttack(target);
    }

    protected virtual void ClearOrder() {
        target = null;
        currentOrder = Order.NONE;
        agent.ResetPath();
    }

    protected virtual void GetNewTarget(Order order) {
        List<Collider> colliderList = new List<Collider>(Physics.OverlapSphere(transform.position, sightRadius));
        WorldObject closestTarget = null;
        switch (order) {
            case Order.ATTACK:
                foreach(Collider c in colliderList) {
                    Unit possibleTarget = c.GetComponentInParent<Unit>();
                    if(possibleTarget && possibleTarget.player != player) {
                        if(closestTarget == null || DistanceToTarget(possibleTarget) < DistanceToTarget(closestTarget)) {
                            closestTarget = possibleTarget;
                        }
                    }
                }
                break;
            case Order.PICK_UP:
                print("Looking for ore");
                foreach(Collider c in colliderList) {
                    Ore possibleTarget = c.GetComponentInParent<Ore>();
                    if (possibleTarget && !possibleTarget.carrier) {
                        if(closestTarget == null || DistanceToTarget(possibleTarget) < DistanceToTarget(closestTarget)) {
                            print(possibleTarget.transform.position);
                            closestTarget = possibleTarget;
                        }
                    }
                }
                break;
        }
        if (closestTarget) {
            //New target found, continue executing order.
            target = closestTarget;
        } else {
            //No viable targets found, cancel order.
            ClearOrder();
        }
    }

    protected virtual void OnDeath() {
        player.ownedUnits.Remove(this);
        if (currentlySelected) {
            player.Deselect(this);
        }
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    public virtual void ChangeHealth(int amount, WorldObject source) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if(currentHealth == 0) {
            OnDeath();
        }else {
            healthBar.OnHealthChange();
        }

        if(currentHealth > 0 && source is Unit && (source as Unit).player != player) {
            BeginAttack(source as Unit);
            CallForAid(source as Unit);
        }
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
        } else if (worldObject == this && carrying) {
            player.ui.SetCursorState(CursorState.PutDown);
        } else if (worldObject is DropOff && carrying) {
            player.ui.SetCursorState(CursorState.PutDown);
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
        } else if (worldObject is Unit && !(worldObject as Unit).GetOwner().Equals(GetOwner())) {
            //Right-clicking an enemy, KILL IT KILL IT DEAD
            BeginAttack(worldObject as Unit);
        } else if (worldObject == this && carrying) {
            Drop();
        } else if (worldObject is DropOff && carrying) {
            BeginDeliver(worldObject as DropOff);
        }
    }

    public override void RightClickGround(Vector3 hitPoint) {
        base.RightClickGround(hitPoint);
        //Units should move to the selected location when having the ground right-clicked.
        StartMove(hitPoint);
    }

    private void StartMove(Vector3 destination) {
        target = null;
        currentOrder = Order.MOVE;
        agent.SetDestination(destination);
    }

    /**
     * Unit is ordered to attack the target.
     * Will keep attempting to move within range of the target and attack it whenever possible.
     */
    protected virtual void BeginAttack(Unit target) {
        this.target = target;
        currentOrder = Order.ATTACK;
    }

    private void Deliver(DropOff target) {
        Ore ore = GetComponentInChildren<Ore>();
        if (ore) {
            target.ReceiveOre(ore);
        } else {
            Debug.LogError("Ore null in Deliver");
        }
        //Drop and destroy ore immediately after delivery
        Drop();
        Destroy(ore.gameObject);
    }

    protected virtual void BeginDeliver(DropOff target) {
        this.target = target;
        currentOrder = Order.PUT_DOWN;
    }

    private void MeleeAttack(Unit target) {
        target.ChangeHealth(-attackDamage, this);
    }

    /**
     * Unit is ordered to to pick upp the chosen ore object
     */
    private void BeginPickUp(Ore ore) {
        currentOrder = Order.PICK_UP;
        target = ore;
        agent.SetDestination(ore.gameObject.transform.position);
    }

    /// <summary>
    /// Ask all allied Units within callrange to attack this units target, as long as they dont already have a target.
    /// </summary>
    protected void CallForAid(Unit target) {
        List<Collider> colliderList = new List<Collider>(Physics.OverlapSphere(transform.position, callRange));
        foreach (Collider c in colliderList) {
            Unit unit = c.gameObject.GetComponentInParent<Unit>();
            if (unit && unit.player == player && !unit.target) {
                unit.BeginAttack(target);
            }
        }
    }
}
