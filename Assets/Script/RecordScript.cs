using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordScript : MonoBehaviour {

	[SerializeField] Text nameText;
	[SerializeField] Text countText;
	[SerializeField] Text percentText;
	[SerializeField] Image percentGraoh;

	string name;
	int nowOnlinePlayCount;
	int nowOnlineWinCount;
	float nowOnlineWinPercent;

	[SerializeField] Canvas inputCanvas;
	[SerializeField] InputField inputField;


	void Start() {
		//name = PlayerPrefs.GetString("PlayerName", "SampleUser");
		//nowOnlinePlayCount = PlayerPrefs.GetInt("OnlinePlayCount", 0);
		//nowOnlineWinCount = PlayerPrefs.GetInt("OnlineWinCount", 0);
		name = "PlayerName";
		nameText.text = name;
		nowOnlinePlayCount = 3;
		nowOnlineWinCount = 1;
		nowOnlineWinPercent = (float)nowOnlineWinCount / nowOnlinePlayCount;
		countText.text = nowOnlineWinCount.ToString().PadLeft(3, '0') + "/" + nowOnlinePlayCount.ToString().PadLeft(3, '0');
		percentGraoh.fillAmount = nowOnlineWinPercent;
		percentText.text = (nowOnlineWinPercent * 100).ToString("f1") + "%";

		inputCanvas.enabled = false;
	}

	public void EditNameButton() {
		inputCanvas.enabled = true;
		inputField.text = name;
	}

	public void FinisyEditNameButton() {
		name = inputField.text;
		nameText.text = name;
		inputCanvas.enabled = false;
		//PlayerPrefs.SetString("PlayerName", name);
	}
}
