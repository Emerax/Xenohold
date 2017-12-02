using System.Collections;
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

    private NavMeshAgent agent;
    private bool carrying = false;
    private Order currentOrder = Order.NONE;
    private WorldObject target;
    private HealthBar healthBar;
    private float remainingCooldown = 0;

    protected override void Awake() {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        agent = GetComponent<NavMeshAgent>();

        //Initiate the unit's health bar.
        Transform parent = GetOwner().ui.canvas.gameObject.transform.Find("PassiveElementsRect");
        GameObject newBar = Instantiate(healthBarObjectPrefab, parent);
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
                    if(DistanceToTarget() <= pickUpDistance) {
                        PickUp(target as Ore);
                        ClearOrder();
                    }
                }
                break;
            case Order.ATTACK:
                if(target is Unit) {
                    if(DistanceToTarget() <= attackRange) {
                        if(remainingCooldown <= 0) {
                            remainingCooldown = attackCooldown;
                            Attack(target as Unit);
                        }
                    } else if(agent.destination != target.transform.position) {
                        //Move into attack range
                        agent.SetDestination(target.transform.position);
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

    public bool InSelectionBounds(Bounds selectionBounds) {
        return selectionBounds.Contains(Camera.main.WorldToViewportPoint(transform.position));
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

    protected virtual void Attack(Unit target) {
        MeleeAttack(target);
    }

    protected virtual void ClearOrder() {
        target = null;
        currentOrder = Order.NONE;
    }

    public virtual void TookDamage(int damage) {
        currentHealth -= damage;
        healthBar.OnHealthChange();
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
        } else if (worldObject is Unit && !(worldObject as Unit).GetOwner().Equals(GetOwner())) {
            //Right-clicking an enemy, KILL IT KILL IT DEAD
            BeginAttack(worldObject as Unit);
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
    private void BeginAttack(Unit target) {
        this.target = target;
        currentOrder = Order.ATTACK;
    }

    private void MeleeAttack(Unit target) {
        target.TookDamage(attackDamage);
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
