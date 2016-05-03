using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
public class PlayerShipController : ShipController {
    public float rotationSpeed = 10;
    private float speed = 0;
    private float acceleration = 50;
   
	// Use this for initialization
	void Start () {
		maxSpeed = 100;
		enemyTags.Add ("Pirate");
		enemyTags.Add ("Merchant");
		enemyTags.Add ("Police");
	}
	
	// Update is called once per frame
	protected override void Update () {
		lifeTime += Time.deltaTime;
        int rot = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        transform.Rotate(transform.up, rot * rotationSpeed * Time.deltaTime);

        int fwd = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        if (fwd != 0)
        {
            speed += acceleration * fwd * Time.deltaTime;

            if (Mathf.Abs(speed) > maxSpeed)
            {
                speed = Mathf.Sign(speed) * maxSpeed;
            }
        } else if (speed > 0)
        {
            speed *= 0.95f;

            if (speed < 0.01)
            {
                speed = 0;
            }
        }
        transform.position += transform.forward * speed * Time.deltaTime;

		if (Input.GetMouseButton (0)) {
			Fire ();
		}
    }
}
