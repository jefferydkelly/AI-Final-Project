using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public int dmg = 100;
	public float moveSpeed = 200;
	public float lifeSpan = 5;
	public float depth = 0;
	public Vector3 fwd = Vector3.zero;
	public ShipController myController;

	void Start() {
		depth = GetComponent<MeshRenderer> ().bounds.size.z;
		Invoke ("Remove", lifeSpan);
	}
	void Update() {
		transform.position += fwd * moveSpeed * Time.deltaTime;
	}

	void Remove() {
		ShipController sc = myController.GetComponent<ShipController> ();
		SteeringVehicle sv = myController.GetComponent<SteeringVehicle> ();
		if (sc.gameObject.CompareTag ("Cop")) 
		{
			bool obs = sc.GetComponent<PoliceShipController> ().DetectPotentialFiringObstacles ();
			sc.LogBulletStat (false, Vector3.Distance (sc.gameObject.transform.position, sv.target.transform.position), obs, 3 - sc.shotsFired);
		}
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col) {
		ShipController sc = myController.GetComponent<ShipController> ();
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
					sc.LogBulletStat (true, Vector3.Distance (sc.gameObject.transform.position, ec.gameObject.transform.position), obs, 3 - sc.shotsFired);
				}
				ec.TakeDamage (dmg);
				myController.RegisterHit ();
				Destroy (gameObject);
			}
		}
	}
}