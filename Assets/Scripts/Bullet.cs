using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public int dmg = 100;
	public float moveSpeed = 200;
	public float lifeSpan = 5;
	public float depth = 0;
	public Vector3 fwd = Vector3.zero;
	public ShipController myController;
	ShipController sc;
	SteeringVehicle sv;
	bool obs;
	int remainingshots;
	float targetdist;

	void Start() {
		depth = GetComponent<MeshRenderer> ().bounds.size.z;
		sc = myController.GetComponentInParent<ShipController> ();
		sv = myController.GetComponentInParent<SteeringVehicle> ();
		targetdist = Vector3.Distance(sc.gameObject.transform.position, sv.target.transform.position);
		obs = myController.GetComponentInParent<PoliceShipController> ().DetectPotentialFiringObstacles();
		remainingshots = myController.GetComponentInParent<PoliceShipController> ().maxbullets - sc.shotsFired;
		Invoke ("Remove", lifeSpan);
	}
	void Update() {
		transform.position += fwd * moveSpeed * Time.deltaTime;
	}

	void Remove() {
		if(sc!=null)
			if (sc.gameObject.CompareTag ("Cop"))
			{
				sc.LogBulletStat (false, targetdist, obs, remainingshots);
			}
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col) {
		ShipController ec = col.gameObject.GetComponent<ShipController> ();
		//Update hits
		if (ec == null) {
			ec = col.gameObject.GetComponentInParent<ShipController> ();
		}
		if (ec != null) {
			if (sc.IsEnemy (col.tag)) {
				Debug.Log ("Hit " + col.tag);
				if (sc.gameObject.CompareTag ("Cop"))
				{
					bool obs = sc.GetComponent<PoliceShipController> ().DetectPotentialFiringObstacles ();
					sc.LogBulletStat (true, targetdist, obs, remainingshots);
				}
				ec.TakeDamage (dmg);
				myController.RegisterHit ();
				Destroy (gameObject);
			}
		}
	}
}