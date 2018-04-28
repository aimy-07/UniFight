using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventScript : MonoBehaviour {

	public BoxCollider smallAttackCol;
	public BoxCollider bigAttackCol;

	void SmallAttackStart() {
		smallAttackCol.enabled = true;
	}

	void SmallAttackEnd() {
		smallAttackCol.enabled = false;
	}

	void BigAttackStart() {
		bigAttackCol.enabled = true;
	}

	void BigAttackEnd() {
		bigAttackCol.enabled = false;
	}

	void Skill() {
		if (transform.root.gameObject.tag == "myPlayer") {
			if (GameObject.FindWithTag("enemyPlayer").gameObject != null) {
				if (Mathf.Abs(Vector3.Distance(transform.position, GameObject.FindWithTag("enemyPlayer").gameObject.transform.position)) < 2) {
					Debug.Log("スキル 成功！");
					GameObject.FindWithTag("myPlayer").gameObject.SendMessage("SkillDamaged");

				}
			}
		} else {
			if (GameObject.FindWithTag("myPlayer").gameObject != null) {
				if (Mathf.Abs(Vector3.Distance(transform.position, GameObject.FindWithTag("myPlayer").gameObject.transform.position)) < 2) {
					Debug.Log("ダメージ！スキル!!");
					GameObject.FindWithTag("myPlayer").gameObject.SendMessage("Damaged", PhotonManager.SkillDamage);
				}
			}
		}
	}
}
