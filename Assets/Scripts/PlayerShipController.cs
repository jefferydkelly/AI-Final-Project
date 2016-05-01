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

        float zoom = Input.GetAxis("Mouse ScrollWheel");
        float cHorz = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
        float cVert = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
        Camera.main.transform.position += new Vector3(cHorz * 5, zoom * 100, cVert * 5);

		if (Input.GetMouseButton (0)) {
			Fire ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			Camera.main.transform.position = new Vector3 (transform.position.x, Camera.main.transform.position.y, transform.position.z);
		}
    }
}
