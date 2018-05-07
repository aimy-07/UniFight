using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

	public static bool leftButtonPressing;
	public static bool rightButtonPressing;
	public static bool upButtonPressing;
	public static bool downButtonPressed;
	public static bool isGround;
	public static bool smallAttackButtonPressed;
	public static bool bigAttackButtonPressed;
	public static bool skillButtonPressed;
	public static bool avoidButtonPressed;


	[SerializeField] Button leftButton;
	[SerializeField] Button rightButton;
	[SerializeField] Button upButton;
	[SerializeField] Button downButton;
	[SerializeField] Button smallAttackButton;
	[SerializeField] Button bigAttackButton;
	[SerializeField] Button skillButton;
	[SerializeField] Button avoidButton;

	float smallAttackTimer;
	float bigAttackTimer;
	float skillTimer;
	float avoidTimer;




	void Start () {
		leftButtonPressing = false;
		rightButtonPressing = false;
		upButtonPressing = false;
		downButtonPressed = false;
		smallAttackButtonPressed = false;
		bigAttackButtonPressed = false;
		skillButtonPressed = false;
		avoidButtonPressed = false;
	}
	
	void Update () {
		if (PhotonManager.phase == PhotonManager.PHASE.isPlaying || OfflineManager.isPlaying) {
			if (30 < Input.mousePosition.x && Input.mousePosition.x < 290) {
				if (220 < Input.mousePosition.y && Input.mousePosition.y < 280) {
					UpButtonDown();
				} else {
					UpButtonUp();
				}
				if (30 < Input.mousePosition.x && Input.mousePosition.x < 160) {
					LeftButtonDown();
				} else {
					RightButtonDown();
				}
			} else {
				UpButtonUp();
				RightButtonUp();
				LeftButtoUp();
			}
		}


		if (leftButtonPressing) {
			rightButton.interactable = false;
		} else {
			rightButton.interactable = true;
		}
		if (rightButtonPressing) {
			leftButton.interactable = false;
		} else {
			leftButton.interactable = true;
		}

		if (isGround) {
			upButton.interactable = true;
			downButton.interactable = false;
		} else {
			upButton.interactable = false;
			downButton.interactable = true;
		}

		if (smallAttackButtonPressed) {
			smallAttackTimer = 0.5f;
			smallAttackButton.interactable = false;
		}
		if (smallAttackTimer > 0) {
			smallAttackTimer -= Time.deltaTime;
		} else {
			smallAttackButton.interactable = true;
		}
		if (bigAttackButtonPressed) {
			bigAttackTimer = 0.7f;
			bigAttackButton.interactable = false;
		}
		if (bigAttackTimer > 0) {
			bigAttackTimer -= Time.deltaTime;
		} else {
			bigAttackButton.interactable = true;
		}
		if (skillButtonPressed) {
			skillTimer = 1.0f;
			skillButton.interactable = false;
		}
		if (skillTimer > 0) {
			skillTimer -= Time.deltaTime;
		} else {
			skillButton.interactable = true;
		}
		if (avoidButtonPressed) {
			avoidTimer = 1.0f;
			avoidButton.interactable = false;
		}
		if (avoidTimer > 0) {
			avoidTimer -= Time.deltaTime;
		} else {
			avoidButton.interactable = true;
		}
	}

	public void LeftButtonDown() {
		leftButtonPressing = true;
		rightButtonPressing = false;
	}

	public void LeftButtoUp() {
		leftButtonPressing = false;
	}

	public void RightButtonDown() {
		rightButtonPressing = true;
		leftButtonPressing = false;
	}

	public void RightButtonUp() {
		rightButtonPressing = false;
	}

	public void UpButtonDown() {
		upButtonPressing = true;
	}

	public void UpButtonUp() {
		upButtonPressing = false;
	}

	public void DownButtonDown() {
		downButtonPressed = true;
	}

	public void SmallAttackButtonDown() {
		smallAttackButtonPressed = true;
	}

	public void BigAttackButtonDown() {
		bigAttackButtonPressed = true;
	}

	public void SkillButtonDown() {
		skillButtonPressed = true;
	}

	public void AvoidButtonDown() {
		avoidButtonPressed = true;
	}
}
