using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceShipController : ShipController {
	private List<GameObject> possibleTargets;
	// Use this for initialization

	void Start() {
		myRenderer = GetComponentInChildren<Renderer> ();
		Wander ();
		enemyTags.Add ("Player");
		enemyTags.Add ("Pirate");
	}
	protected override void Update ()
	{
		base.Update ();

		if (moveStatus == MovementStatus.Seek) {
			float dist = (target.transform.position - transform.position).magnitude;

			if (dist <= fireDistance) {
				Fire ();
			}
		}
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
		Collider[] colls = Physics.OverlapSphere(transform.position, 50.0f);
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
}
