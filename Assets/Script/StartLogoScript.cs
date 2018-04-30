using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLogoScript : MonoBehaviour {

	float t = 0;

	void Start () {
		
	}
	
	void Update () {
		t += Time.deltaTime;
		if (t > 2) {
			if (Input.GetMouseButton(0)) {
				SceneManager.LoadScene("Title");
			}
		}
	}
}
