using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityBasedAnimation : MonoBehaviour
{
	private Animator animator;
	private IMover mover;
	public int directionIndex;

	private string[] directions = new string[] { "Up", "UpRight", "Right", "DownRight", "Down", "DownLeft", "Left", "UpLeft" };

	public string playAnim;
	public float idleThreshold = .1f;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		mover = GetComponentInParent<IMover>();
	}

	private void Update()
	{
		if (playAnim != "")
		{
			animator.Play(playAnim);

			return;
		}

		if (Mathf.Abs(mover.velocity.x) > idleThreshold || Mathf.Abs(mover.velocity.y) > idleThreshold)
		{
			//update direction
			UpdateDirectionIndex();

			//Run anim
			animator.Play("Run" + directions[directionIndex]);
		}

		else
		{
			//Idle anim
			animator.Play("Idle" + directions[directionIndex]);
		}
	}

	private void UpdateDirectionIndex()
	{
		//up
		if (mover.velocity.normalized.x > -.35f && mover.velocity.normalized.x < .35f && mover.velocity.normalized.y > 0)
			directionIndex = 0;
		//up right
		if (mover.velocity.normalized.x > .35f && mover.velocity.normalized.y > .35f)
			directionIndex = 1;
		//right
		if (mover.velocity.normalized.x > 0 && mover.velocity.normalized.y < .35f && mover.velocity.normalized.y > -.35f)
			directionIndex = 2;
		//down right
		if (mover.velocity.normalized.x > .35f && mover.velocity.normalized.y < -.35f)
			directionIndex = 3;
		//down
		if (mover.velocity.normalized.x > -.35f && mover.velocity.normalized.x < .35f && mover.velocity.normalized.y < 0)
			directionIndex = 4;
		//down left
		if (mover.velocity.normalized.x < -.35f && mover.velocity.normalized.y < -.35f)
			directionIndex = 5;
		//left
		if (mover.velocity.normalized.x < 0 && mover.velocity.normalized.y < .35f && mover.velocity.normalized.y > -.35f)
			directionIndex = 6;
		//up left
		if (mover.velocity.normalized.x < -.35f && mover.velocity.normalized.y > .35f)
			directionIndex = 7;
	}
}
