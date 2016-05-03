using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PirateShipController : ShipController {
	public float chaseMerchDistance = 50;
	public float fleeCopsDistance = 50;

	private List<GameObject> possibleTargets;
	// Use this for initialization
	void Start () 
	{
		myRenderer = GetComponentInChildren<Renderer> ();
		//Wander ();
		Flock();
		enemyTags.Add ("Merchant");
		enemyTags.Add ("Police");
		enemyTags.Add ("Player");
    }
	protected override void Update ()
	{
		base.Update ();
		if (moveStatus == MovementStatus.Seek && areTargetsInRange(true)) {
			float dist = (target.transform.position - transform.position).magnitude;

			if (dist <= fireDistance) {
				Fire ();
			}
		}
	}
    protected override Vector3 CalcSteeringForce()
    {
        Vector3 steeringForce = Vector3.zero;
		if (moveStatus == MovementStatus.Flee) {
			steeringForce = SV_Flee (target) + (SV_Wander () * 0.5f);
		} else if (moveStatus == MovementStatus.Seek) {
            steeringForce = SV_Seek (target) + (SV_Wander () * 0.5f);
		} else if (moveStatus == MovementStatus.Wander) {
            steeringForce = SV_Wander ();
		} else if (moveStatus == MovementStatus.Flock) {
            steeringForce = SV_Flock () + SV_Wander ();
		}
        if (moveStatus != MovementStatus.Idle)
        {
            steeringForce += AvoidObstacles();
        }

        return steeringForce;
    }

	protected override void CalcMoveState ()
	{
		if (moveStatus == MovementStatus.Wander || moveStatus == MovementStatus.Flock) {
			if (AlliesAndEnemiesInRadius (chaseMerchDistance) > aggressiveness && areTargetsInRange(false)) {
                GameObject tar = FindTarget();

                if (tar != null)
                {
                    Flee(tar);
                }
				
			} else if (areTargetsInRange (true)) {
                GameObject tar = FindTarget();

                if (tar != null)
                {
                    Seek(tar);
                }
            }
		} else if (moveStatus == MovementStatus.Flee) {
			if (!areTargetsInRange (false)) {
				Wander ();
			}
		} else if (moveStatus == MovementStatus.Seek) {
			if (!areTargetsInRange (true)) {
				Wander ();
			}
		}
	}

	protected override void CreateFlock ()
	{
		flock = new List<SteeringVehicle> ();
		List<PirateShipController> pFlock = new List<PirateShipController> ();
		Collider[] colls = Physics.OverlapSphere(transform.position, flockRadius);
		foreach (Collider col in colls) {
			if (col.CompareTag("Pirate") && col.gameObject != gameObject) {
				if (!pFlock.Contains(col.GetComponent<PirateShipController>())) {
					pFlock.Add (col.GetComponent<PirateShipController> ());
					flock.Add (col.GetComponent<SteeringVehicle> ());
				}
			}
		}
	}
	public bool areTargetsInRange(bool seek) {
		possibleTargets = new List<GameObject> ();
		Collider[] colls = Physics.OverlapSphere(transform.position, 50.0f);
		List<GameObject> nearbyMerchants = new List<GameObject> ();
		List<GameObject> nearbyPolice = new List<GameObject> ();

		foreach (Collider c in colls) {
			if (c.CompareTag ("Merchant") && seek) {
				possibleTargets.Add (c.gameObject);
			} else if (c.CompareTag("Cop") && !seek) {
				possibleTargets.Add (c.gameObject);
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

	public float AlliesAndEnemiesInRadius(float rad) {
		float alliesInRadius = 0;
		float enemiesInRadius = 0;

		Collider[] colls = Physics.OverlapSphere (transform.position, rad);

		foreach (Collider c in colls) {
			if (c.CompareTag ("Pirate")) {
				alliesInRadius++;
			} else if (c.CompareTag ("Cop")) {
				enemiesInRadius++;
			}
		}

		return alliesInRadius / enemiesInRadius;
	}
}