using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {

	public void toSelectCostume() {
		SceneManager.LoadScene("SelectCostume");
	}

	public void toMatching() {
		SceneManager.LoadScene("OnlineMain");
	}

	public void toTitle() {
		SceneManager.LoadScene("Title");
	}

	public void toRecord() {
		SceneManager.LoadScene("Record");
	}
}
