using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceShipController : ShipController {
	private List<GameObject> possibleTargets;
	public PoliceSpawn spawner;
	// Use this for initialization

	void Start() {
        InitializeShip();
		myRenderer = GetComponentInChildren<Renderer> ();
		Wander ();
		enemyTags.Add ("Player");
		enemyTags.Add ("Pirate");
	}
	protected override void Update ()
	{
		base.Update ();

		if (moveStatus == MovementStatus.Seek) {
			Debug.Log ("Starting firing decider...");
			float dist = (target.transform.position - transform.position).magnitude;
			bool obs = DetectPotentialFiringObstacles ();
			bool shouldfire = GameObject.Find("Planet A").GetComponent<BayesScript> ().Decide (dist, obs, 3 - shotsFired);
			Debug.Log ("Should I fire? "+shouldfire+"!");
			if (shouldfire)
				Fire ();
		}
	}

	public override void Fire() {
		if (canFire) {
			if (shotsFired < 3) {
				GameObject laser = GameObject.Instantiate (laserBase);
				laser.transform.position = transform.position + transform.forward * (depth + laser.GetComponent<Bullet> ().depth);
				laser.transform.rotation = transform.rotation;
				laser.GetComponent<Bullet> ().fwd = Forward;
				canFire = false;
				laser.GetComponent<Bullet> ().myController = this;
				shotsFired++;
				Invoke ("ResetCooldown", cooldown);
			}
		}
	}

	public bool DetectPotentialFiringObstacles()
	{
		Collider[] hitColliders = Physics.OverlapSphere (this.gameObject.transform.position, 100);
		if (hitColliders.Length > 0)
			return true;
		else
			return false;
	}

    protected override Vector3 CalcSteeringForce() {
        Vector3 steeringForce = Vector3.zero;
		if (moveStatus == MovementStatus.Seek) {
			steeringForce += SV_Seek (target) + SV_Wander() * 0.5f;
		} else if (moveStatus == MovementStatus.Wander) {
			steeringForce += SV_Wander ();
		}

        if (moveStatus != MovementStatus.Idle)
        {
			steeringForce += AvoidObstacles() * obstacleAvoidanceWeight;
			steeringForce += StayInArea ();
        }

        return steeringForce;
    }

	protected override void CalcMoveState ()
	{
		if (moveStatus == MovementStatus.Wander) {
			if (areTargetsInRange ()) {
	
                GameObject tar = FindTarget();
                if (tar != null)
				{
                    Seek(tar);
                }
			}
		} else if (moveStatus == MovementStatus.Seek) {
			if (!areTargetsInRange ()) {
				Wander ();
			}
		}
	}

	protected override void CreateFlock ()
	{
		flock = new List<SteeringVehicle> ();

		Collider[] colls = Physics.OverlapSphere(transform.position, flockRadius);
		foreach (Collider col in colls) {
			if (col.CompareTag ("Cop")) {
				flock.Add (col.gameObject.GetComponent<SteeringVehicle>());	
			}
		}
	}

	public bool areTargetsInRange() {
		possibleTargets = new List<GameObject> ();
		Collider[] colls = Physics.OverlapSphere(transform.position, 500.0f);
		foreach (Collider col in colls) {
			if (col.CompareTag ("Pirate")) {
				possibleTargets.Add (col.gameObject);	
			}
		}

		return possibleTargets.Count > 0;
	}

	public GameObject FindTarget() {
		GameObject closestTarget = null;
		float targetDistance = float.MaxValue;
		foreach (GameObject go in possibleTargets) {
			float dist = (go.transform.position - transform.position).magnitude;
			if (dist < targetDistance) {
				closestTarget = go;
				targetDistance = dist;
			}
		}

        return closestTarget;
	}

	void OnDestroy() {
		if (spawner != null) {
			spawner.NumberOfPolice -= 1;
		}
	}

}
