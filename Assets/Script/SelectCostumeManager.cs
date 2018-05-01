using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCostumeManager : MonoBehaviour {

	public static int chara;
	public GameObject[] charaObjs;
	GameObject[,] charaChildObjs = new GameObject[7, 4];
	[SerializeField] Text charaSelectText;
	public string[] charaNames;

	public static int hairColor;
	public static int eyeColor;
	[SerializeField] Image hairColorSelectImage;
	[SerializeField] Image eyeColorSelectImage;
	public Sprite[] hairEyeColorSprites;

	public static int costumeColor;
	[SerializeField] Image costumeColorSelectImage;
	public Sprite[] costumeColorSprites;

	[SerializeField] Button[] buttons;




	void Start () {
		chara = PlayerPrefs.GetInt("chara", 0);
		hairColor = PlayerPrefs.GetInt("hair", 1);
		eyeColor = PlayerPrefs.GetInt("eye", 3);
		costumeColor = PlayerPrefs.GetInt("costume", 6);
		for (int i = 0; i < 7; i++) {
			charaChildObjs[i, 0] = charaObjs[i].transform.Find("_root").gameObject;
			charaChildObjs[i, 1] = charaObjs[i].transform.Find("costume").gameObject;
			charaChildObjs[i, 2] = charaObjs[i].transform.Find("eye").gameObject;
			charaChildObjs[i, 3] = charaObjs[i].transform.Find("hair").gameObject;
		}
		SetChara();
		SetHairColor();
		SetEyeColor();
		SetCostumeColor();
		SetButtons();
	}
	
	void Update () {

	}

	void SetChara() {
		for (int i = 0; i < 7; i++) {
			if (i == chara) {
				for (int j = 0; j < 4; j++) {
					charaChildObjs[i, j].SetActive(true);
				}
			} else {
				for (int j = 0; j < 4; j++) {
					charaChildObjs[i, j].SetActive(false);
				}
			}
		}	
		charaSelectText.text = charaNames[chara];
		PlayerPrefs.SetInt("chara", chara);
	}

	void SetHairColor() {
		switch(chara) {
			case 0:
			case 1:
			case 2:
			charaChildObjs[chara, 3].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
			break;
			case 3:
			case 4:
			charaChildObjs[chara, 3].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
			break;
			case 5:
			case 6:
			charaChildObjs[chara, 3].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
			break;
		}
		hairColorSelectImage.sprite = hairEyeColorSprites[hairColor];
		PlayerPrefs.SetInt("hair", hairColor);
	}

	void SetEyeColor() {
		switch(chara) {
			case 0:
			case 1:
			case 2:
			charaChildObjs[chara, 2].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
			break;
			case 3:
			case 4:
			charaChildObjs[chara, 2].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
			break;
			case 5:
			case 6:
			charaChildObjs[chara, 2].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
			break;
		}
		eyeColorSelectImage.sprite = hairEyeColorSprites[eyeColor];
		PlayerPrefs.SetInt("eye", eyeColor);
	}

	void SetCostumeColor() {
		if (chara == 0) {
			charaChildObjs[chara, 1].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
		} else {
			charaChildObjs[chara, 1].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
		}
		if (costumeColor == 6) {
			if (chara == 0) {
				costumeColorSelectImage.sprite = costumeColorSprites[6];
			} else {
				costumeColorSelectImage.sprite = costumeColorSprites[7];
			}
		} else {
			costumeColorSelectImage.sprite = costumeColorSprites[costumeColor];
		}
		PlayerPrefs.SetInt("costume", costumeColor);
	}

	public void CharaNextButton() {
		if (chara < 6) {
			chara++;
			SetChara();
			SetHairColor();
			SetEyeColor();
			SetCostumeColor();
			SetButtons();
		}
	}

	public void CharaPrevButton() {
		if (chara > 0) {
			chara--;
			SetChara();
			SetHairColor();
			SetEyeColor();
			SetCostumeColor();
			SetButtons();
		}
	}

	public void HairNextButton() {
		if (hairColor < 6) {
			hairColor++;
			SetHairColor();
			SetButtons();
		}
	}

	public void HairPrevButton() {
		if (hairColor > 0) {
			hairColor--;
			SetHairColor();
			SetButtons();
		}
	}

	public void EyeNextButton() {
		if (eyeColor < 6) {
			eyeColor++;
			SetEyeColor();
			SetButtons();
		}
	}

	public void EyePrevButton() {
		if (eyeColor > 0) {
			eyeColor--;
			SetEyeColor();
			SetButtons();
		}
	}

	public void CostumeColorNextButton() {
		if (costumeColor < 6) {
			costumeColor++;
			SetCostumeColor();
			SetButtons();
		}
	}

	public void CostumeColorPrevButton() {
		if (costumeColor > 0) {
			costumeColor--;
			SetCostumeColor();
			SetButtons();
		}
	}

	void SetButtons() {
		//キャラ
		if (chara == 0) {
			buttons[0].interactable = false;
		} else {
			buttons[0].interactable = true;
		}
		if (chara == 6) {
			buttons[1].interactable = false;
		} else {
			buttons[1].interactable = true;
		}
		//髪
		if (hairColor == 0) {
			buttons[2].interactable = false;
		} else {
			buttons[2].interactable = true;
		}
		if (hairColor == 6) {
			buttons[3].interactable = false;
		} else {
			buttons[3].interactable = true;
		}
		//目
		if (eyeColor == 0) {
			buttons[4].interactable = false;
		} else {
			buttons[4].interactable = true;
		}
		if (eyeColor == 6) {
			buttons[5].interactable = false;
		} else {
			buttons[5].interactable = true;
		}
		//服
		if (costumeColor == 0) {
			buttons[6].interactable = false;
		} else {
			buttons[6].interactable = true;
		}
		if (costumeColor == 6) {
			buttons[7].interactable = false;
		} else {
			buttons[7].interactable = true;
		}
	}

	public void BackButton() {
		SceneManager.LoadScene("Title");
	}
}
