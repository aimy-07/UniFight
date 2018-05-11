using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfflineCharaSet : MonoBehaviour {

	int chara;
	int hairColor;
	int eyeColor;
	int costumeColor;
	[SerializeField] Text playerNameText;
	[SerializeField] TextMesh playerNameTextMesh;

	void Start () {
		if (gameObject.tag == "myPlayer") {
			chara = PlayerPrefs.GetInt("chara", 0);
			hairColor = PlayerPrefs.GetInt("hair", 1);
			eyeColor = PlayerPrefs.GetInt("eye", 3);
			costumeColor = PlayerPrefs.GetInt("costume", 6);
			playerNameText.text = PlayerPrefs.GetString("PlayerName", "No Name (Error)");
			playerNameTextMesh.text = PlayerPrefs.GetString("PlayerName", "No Name (Error)");
		} else if (gameObject.tag == "enemyPlayer") {
			chara = Random.Range(0, 7);
			hairColor = Random.Range(0, 7);
			eyeColor = Random.Range(0, 7);
			costumeColor = Random.Range(0, 7);
			gameObject.GetComponent<OfflineEnemyController>().chara = chara;
			playerNameText.text = "NPC";
			playerNameTextMesh.text = "NPC";
		}
		SetChara(chara, hairColor, eyeColor, costumeColor);
	}
	
	void SetChara(int chara, int hairColor, int eyeColor, int costumeColor) {
		GameObject charaObj = Instantiate(Resources.Load("CharaPrefabs/chara" + chara) as GameObject);
		charaObj.transform.parent = gameObject.transform;
		charaObj.gameObject.name = "UTC_Default";
		charaObj.transform.localPosition = new Vector3(0, 0, 0);
		charaObj.transform.localRotation = new Quaternion(0, 0, 0, 1);
		charaObj.gameObject.AddComponent<FreezeCharaPosition>();
		charaObj.gameObject.GetComponent<AnimationEventScript>().smallAttackCol = transform.root.gameObject.transform.Find("SmallAttackCollider").gameObject.GetComponent<BoxCollider>();
		charaObj.gameObject.GetComponent<AnimationEventScript>().bigAttackCol = transform.root.gameObject.transform.Find("BigAttackCollider").gameObject.GetComponent<BoxCollider>();

		GameObject charaImgObj = null;
		if (transform.position.x < 0) {
			for (int i = 0; i < 7; i++) {
				if (i == chara) {
					charaImgObj = GameObject.Find("charaLeft" + i);
				} else {
					GameObject.Find("charaLeft" + i).SetActive(false);
				}
			}
		} else {
			for (int i = 0; i < 7; i++) {
				if (i == chara) {
					charaImgObj = GameObject.Find("charaRight" + i);
				} else {
					GameObject.Find("charaRight" + i).SetActive(false);
				}
			}
		}
		
		if (chara == 0) {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
			charaImgObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
		} else {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
			charaImgObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
		}
		switch(GetCharaNum(chara)) {
			case 0:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
				charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
				charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
				break;
			case 1:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
				charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
				charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
				break;
			case 2:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
				charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
				charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
				break;
		}

		Debug.Log(gameObject.tag + " : キャラのセッティングが完了しました");
	}

	public static int GetCharaNum(int chara) {
		switch(chara) {
			case 0:
			case 1:
			case 2:
				return 0;
			case 3:
			case 4:
				return 1;
			case 5:
			case 6:
				return 2;
			default:
				return 0;
		}
	}
}
