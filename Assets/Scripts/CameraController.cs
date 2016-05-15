using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraController : MonoBehaviour {
	GameObject player = null;
	public float roundTime = 300;
	private PirateSpawn pSpawn;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		StartRound ();
	}
	
	// Update is called once per frame
	void Update () {
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		float cHorz = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
		float cVert = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
		Camera.main.transform.position += new Vector3(cHorz * 5, zoom * 100, cVert * 5);


		if (player != null && Input.GetKeyDown (KeyCode.Space)) {
			Camera.main.transform.position = new Vector3 (transform.position.x, Camera.main.transform.position.y, transform.position.z);
		}
	}

	void StartRound() {
		pSpawn = GameObject.FindGameObjectWithTag ("Cove").GetComponent<PirateSpawn> ();
		pSpawn.Spawn ();
		Invoke ("EndRound", roundTime);
	}

	void EndRound() {
		//pSpawn.roundEnd();
		//Go To Round Over Screen - Ask Continue or Quit.
		SceneManager.LoadScene("RoundOver");
	}
}
