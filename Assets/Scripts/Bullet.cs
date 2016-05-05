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
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col) {
		ShipController sc = myController.GetComponent<ShipController> ();
		ShipController ec = col.gameObject.GetComponent<ShipController> ();
		//Update hits
		if (ec != null) {
			if (sc.IsEnemy (col.tag)) {
				ec.TakeDamage (dmg);
				myController.RegisterHit ();
				Remove ();
			}
		}
	}
}