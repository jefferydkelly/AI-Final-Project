using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MerchantShipController : ShipController {
	private List<GameObject> possibleTargets;
	// Use this for initialization
	void Start () 
	{
		myRenderer = GetComponentInChildren<Renderer> ();
		Wander ();
    }

    protected override Vector3 CalcSteeringForce()
    {
        Vector3 steeringForce = Vector3.zero;
		if (moveStatus == MovementStatus.Flee) {
            if (target != null)
            {
                steeringForce = SV_Flee(target);
            } else
            {
                steeringForce = SV_Flee(targetPos);
            }
		} else if (moveStatus == MovementStatus.Wander) {
			steeringForce = SV_Wander ();
		}

        if (moveStatus != MovementStatus.Idle)
        {
            steeringForce += AvoidObstacles();
        }

        return steeringForce;
    }

	protected override void CalcMoveState() {
		if (moveStatus == MovementStatus.Flee) {
			if (!areTargetsInRange()) {
				Wander ();
			}
		} else if (moveStatus == MovementStatus.Wander) {
			if (areTargetsInRange()) {
                GameObject tar = FindTarget();
                if (tar != null)
                {
                    Flee(tar);
                }
			}
		}

	}

	protected override void CreateFlock ()
	{
		flock = new List<SteeringVehicle> ();

		Collider[] colls = Physics.OverlapSphere(transform.position, flockRadius);
		foreach (Collider col in colls) {
			if (col.CompareTag ("Merchant")) {
				flock.Add (col.gameObject.GetComponent<SteeringVehicle>());	
			}
		}
	}

	public bool areTargetsInRange() {
		possibleTargets = new List<GameObject> ();
		Collider[] colls = Physics.OverlapSphere(transform.position, 50.0f);
		foreach (Collider col in colls) {
			if (col.CompareTag ("Pirate") || col.CompareTag ("Player")) {
				possibleTargets.Add (col.gameObject);	
			}
		}

		return possibleTargets.Count > 0;
	}

	public GameObject FindTarget() {
		GameObject closestTarget = null;
		float targetDistance = float.MaxValue;
		foreach (GameObject go in possibleTargets) {
			float dist = ((go.transform.position - transform.position).magnitude);
			if (dist < targetDistance) {
				closestTarget = go;
				targetDistance = dist;
			}
		}

		return closestTarget;
	}
}

