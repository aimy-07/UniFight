using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

	public static bool leftButtonPressed;
	public static bool leftButtonPressing;
	public static bool leftButtonUpped;
	public static bool rightButtonPressed;
	public static bool rightButtonPressing;
	public static bool rightButtonUpped;

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
		leftButtonPressed = false;
		leftButtonPressing = false;
		leftButtonUpped = false;
		rightButtonPressed = false;
		rightButtonPressing = false;
		rightButtonUpped = false;
		upButtonPressing = false;
		downButtonPressed = false;
		smallAttackButtonPressed = false;
		bigAttackButtonPressed = false;
		skillButtonPressed = false;
		avoidButtonPressed = false;
	}
	
	void Update () {
		if (PhotonManager.phase == PhotonManager.PHASE.isPlaying) {
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
			smallAttackTimer = 1.0f;
		}
		if (smallAttackTimer > 0) {
			smallAttackTimer -= Time.deltaTime;
			smallAttackButton.interactable = false;
		} else {
			smallAttackTimer = 0;
			smallAttackButton.interactable = true;
		}
		if (bigAttackButtonPressed) {
			bigAttackTimer = 1.0f;
		}
		if (bigAttackTimer > 0) {
			bigAttackTimer -= Time.deltaTime;
			bigAttackButton.interactable = false;
		} else {
			bigAttackTimer = 0;
			bigAttackButton.interactable = true;
		}
		if (skillButtonPressed) {
			skillTimer = 1.0f;
		}
		if (skillTimer > 0) {
			skillTimer -= Time.deltaTime;
			skillButton.interactable = false;
		} else {
			skillTimer = 0;
			skillButton.interactable = true;
		}
		if (avoidButtonPressed) {
			avoidTimer = 1.0f;
		}
		if (avoidTimer > 0) {
			avoidTimer -= Time.deltaTime;
			avoidButton.interactable = false;
		} else {
			avoidTimer = 0;
			avoidButton.interactable = true;
		}
	}

	public void LeftButtonDown() {
		leftButtonPressed = true;
		leftButtonPressing = true;
		rightButtonPressing = false;
	}

	public void LeftButtoUp() {
		leftButtonUpped = true;
		leftButtonPressing = false;
	}

	public void RightButtonDown() {
		rightButtonPressed = true;
		rightButtonPressing = true;
		leftButtonPressing = false;
	}

	public void RightButtonUp() {
		rightButtonUpped = true;
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
