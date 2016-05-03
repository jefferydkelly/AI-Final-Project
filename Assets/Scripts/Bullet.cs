using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public int dmg = 5;
	public float moveSpeed = 200;
	public float lifeSpan = 5;
	public float depth = 0;
	public Vector3 fwd = Vector3.zero;
	public ShipController myController;

	void Start() {
		depth = GetComponent<MeshRenderer> ().bounds.size.z;
	}
	void Update() {
		transform.position += fwd * moveSpeed * Time.deltaTime;
		Invoke ("Remove", lifeSpan);
	}

	void Remove() {
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col) {
		ShipController sc = myController.GetComponent<ShipController> ();
		//Update hits
		Debug.Log("Hit");
		sc.TakeDamage(dmg);
		myController.RegisterHit ();
		Remove ();
	}
}