using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCostumeManager : MonoBehaviour {

	public static int CHARA_MAX = 7;
	public static int HAIRCOLOR_MAX = 7;
	public static int EYECOLOR_MAX = 7;
	public static int COSTUME_MAX = 7;
	[SerializeField] LockedCostume lockedCostume;

	public static int chara;
	public GameObject[] charaObjs;
	GameObject[,] charaChildObjs = new GameObject[CHARA_MAX, 4];
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

	public AudioClip bgm;


	void Start () {
		chara = PlayerPrefs.GetInt("chara", 0);
		hairColor = PlayerPrefs.GetInt("hair", 0);
		eyeColor = PlayerPrefs.GetInt("eye", 0);
		costumeColor = PlayerPrefs.GetInt("costume", 0);
		for (int i = 0; i < CHARA_MAX; i++) {
			charaChildObjs[i, 0] = charaObjs[i].transform.Find("_root").gameObject;
			charaChildObjs[i, 1] = charaObjs[i].transform.Find("costume").gameObject;
			charaChildObjs[i, 2] = charaObjs[i].transform.Find("eye").gameObject;
			charaChildObjs[i, 3] = charaObjs[i].transform.Find("hair").gameObject;
			// 0 : 顔
			// 1 : 服
			// 2 : 目
			// 3 : 髪
		}
		SetChara();
		SetHairColor();
		SetEyeColor();
		SetCostumeColor();

		AudioSourceManager.PlayBGM(bgm);
	}
	
	void SetChara() {
		for (int i = 0; i < CHARA_MAX; i++) {
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

	public void SetHairColor() {
		if (hairColor <= 5
			 || hairColor == 6 && LockedCostume.set_black_locked == 1) {
			switch(OfflineCharaSet.GetCharaNum(chara)) {
				case 0:
					charaChildObjs[chara, 3].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
					break;
				case 1:
					charaChildObjs[chara, 3].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
					break;
				case 2:
					charaChildObjs[chara, 3].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
					break;
			}
			PlayerPrefs.SetInt("hair", hairColor);
		}
		hairColorSelectImage.sprite = hairEyeColorSprites[hairColor];
		lockedCostume.LockedHairColor(hairColor, eyeColor, costumeColor);
	}

	public void SetEyeColor() {
		if (eyeColor <= 5
			 || eyeColor == 6 && LockedCostume.set_black_locked == 1) {
			switch(OfflineCharaSet.GetCharaNum(chara)) {
				case 0:
					charaChildObjs[chara, 2].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
					break;
				case 1:
					charaChildObjs[chara, 2].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
					break;
				case 2:
					charaChildObjs[chara, 2].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
					break;	
			}
			PlayerPrefs.SetInt("eye", eyeColor);
		}
		eyeColorSelectImage.sprite = hairEyeColorSprites[eyeColor];
		lockedCostume.LockedEyeColor(hairColor, eyeColor, costumeColor);
	}

	public void SetCostumeColor() {
		if (costumeColor <= 4
			 || costumeColor == 5 && LockedCostume.set_black_locked == 1
			 || costumeColor == 6 && LockedCostume.costumeColor_default_locked == 1) {
			if (chara == 0) {
				charaChildObjs[chara, 1].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
			} else {
				charaChildObjs[chara, 1].GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
			}
			PlayerPrefs.SetInt("costume", costumeColor);
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
		lockedCostume.LockedCostumeColor(hairColor, eyeColor, costumeColor);
	}

	public void CharaNextButton() {
		if (chara == CHARA_MAX - 1) {
			chara = 0;
		} else {
			chara++;
		}
		SetChara();
		SetHairColor();
		SetEyeColor();
		SetCostumeColor();
		AudioSourceManager.PlaySE(2);
	}

	public void CharaPrevButton() {
		if (chara == 0) {
			chara = 6;
		} else {
			chara--;
		}
		SetChara();
		SetHairColor();
		SetEyeColor();
		SetCostumeColor();
		AudioSourceManager.PlaySE(2);
	}

	public void HairNextButton() {
		if (hairColor == HAIRCOLOR_MAX - 1) {
			hairColor = 0;
		} else {
			hairColor++;
		}
		SetHairColor();
		AudioSourceManager.PlaySE(2);
	}

	public void HairPrevButton() {
		if (hairColor == 0) {
			hairColor = 6;
		} else {
			hairColor--;
		}
		SetHairColor();
		AudioSourceManager.PlaySE(2);
	}

	public void EyeNextButton() {
		if (eyeColor == EYECOLOR_MAX - 1) {
			eyeColor = 0;
		} else {
			eyeColor++;
		}
		SetEyeColor();
		AudioSourceManager.PlaySE(2);
	}

	public void EyePrevButton() {
		if (eyeColor == 0) {
			eyeColor = 6;
		} else {
			eyeColor--;
		}
		SetEyeColor();
		AudioSourceManager.PlaySE(2);
	}

	public void CostumeColorNextButton() {
		if (costumeColor == COSTUME_MAX - 1) {
			costumeColor = 0;
		} else {
			costumeColor++;
		}
		SetCostumeColor();
		AudioSourceManager.PlaySE(2);
	}

	public void CostumeColorPrevButton() {
		if (costumeColor == 0) {
			costumeColor = 6;
		} else {
			costumeColor--;
		}
		SetCostumeColor();
		AudioSourceManager.PlaySE(2);
	}

	public void BackButton() {
		SceneManager.LoadScene("Title");
		AudioSourceManager.PlaySE(3);
	}
}
