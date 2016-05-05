using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
public class PlayerShipController : ShipController {
	public float rotationSpeed = Mathf.PI;
    private float speed = 0;
    private float acceleration = 50;
	private float yRot = 0;
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
		yRot += rot * rotationSpeed * Time.deltaTime;
		transform.rotation = Quaternion.AngleAxis (yRot, Vector3.up);

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

	public override void Fire() {
		if (canFire) {
			GameObject laser = GameObject.Instantiate (laserBase);
			laser.transform.position = transform.position + transform.forward * (depth + laser.GetComponent<Bullet>().depth);
			laser.transform.rotation = transform.rotation;
			laser.GetComponent<Bullet>().fwd = transform.forward;
			canFire = false;
			laser.GetComponent<Bullet> ().myController = this;
			shotsFired++;
			Invoke ("ResetCooldown", cooldown);
		}
	}
}
