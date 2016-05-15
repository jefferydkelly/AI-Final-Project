using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoundOverScreen : MonoBehaviour {

	public void NextRound() {
		SceneManager.LoadScene ("solsystem");
	}

	public void QuitGame() {
		Application.Quit ();
	}
}
