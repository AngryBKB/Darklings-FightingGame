using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : State
{
	private IdleState _idleState;

	void Awake()
	{
		_idleState = GetComponent<IdleState>();
	}

	public override void Enter()
	{
		base.Enter();
		_audio.Sound("Hit").Play();
		_playerAnimator.Throw();
		_playerAnimator.OnCurrentAnimationFinished.AddListener(ToIdleState);
	}

	private void ToIdleState()
	{
		if (_stateMachine.CurrentState == this)
		{
			_stateMachine.ChangeState(_idleState);
		}
	}
}
