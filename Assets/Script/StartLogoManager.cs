using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLogoManager : MonoBehaviour {

	float t = 0;

	void Start () {
		// PlayerPrefs.DeleteAll();
	}
	
	void Update () {
		t += Time.deltaTime;
		if (t > 2) {
			if (Input.GetMouseButton(0)) {
				if (PlayerPrefs.GetInt("FirstOpen", 0) == 0) {
					SceneManager.LoadScene("InputName");
				} else if (PlayerPrefs.GetInt("FirstOpen", 0) == 1) {
					SceneManager.LoadScene("Title");
				}
			}
		}
	}
}
