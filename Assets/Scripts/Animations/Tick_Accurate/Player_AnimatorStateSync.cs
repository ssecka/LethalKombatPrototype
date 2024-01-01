using Fusion;
using UnityEngine;

namespace Animations.AnimatorStateSynchronization
{
	[OrderAfter(typeof(CharacterController))]
	public class Player_AnimatorStateSync : NetworkBehaviour
	{
		// PRIVATE MEMBERS

		private NetworkCharacterControllerPrototypeCustom _controller;
		private Animator _animator;
		private int _lastVisibleJump, _lastVisibleJab, _lastVisibleHook,_lastVisibleKick, _lastVisibleLowKick, _lastVisibleFireBall;

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
		//	_lastVisibleJump = _controller.JumpCount;
		}

		public override void Render()
		{
			UpdateAnimations();
		}

		// MONOBEHAVIOUR

		protected void Awake()
		{
			_controller = GetComponentInChildren<NetworkCharacterControllerPrototypeCustom>();
			_animator = GetComponentInChildren<Animator>();
		}

		// PRIVATE METHODS

		private void UpdateAnimations()
		{
			// ADD HERE MORE ANIMATIONS IF NEEDED
			
			HandleJump();

			HandleJab();

			HandleHook();

			HandleKick();
			
			HandleLowKick();
			
			HandleFireBall();


			//_animator.SetFloat("Speed", _controller.InterpolatedSpeed);
		}

		private void HandleKick()
		{
			if (_lastVisibleKick < _controller.InterpolatedKickCount)
			{
				_animator.SetTrigger("SideKick");
			}
			else if (_lastVisibleKick < _controller.InterpolatedKickCount)
			{
				// canccel
			}

			_lastVisibleKick = _controller.InterpolatedKickCount;
		}

		private void HandleHook()
		{
			if (_lastVisibleHook < _controller.InterpolatedHookCount)
			{
				_animator.SetTrigger("Hook");
			}
			else if (_lastVisibleHook > _controller.InterpolatedHookCount)
			{
				// cancel
			}

			_lastVisibleHook = _controller.InterpolatedHookCount;
		}

		private void HandleJab()
		{
			if (_lastVisibleJab < _controller.InterpolatedJabCount)
			{
				_animator.SetTrigger("Jab");
			}
			else if (_lastVisibleJab > _controller.InterpolatedJabCount)
			{
				//Cancel Jab
			}
			_lastVisibleJab = _controller.InterpolatedJabCount;

		}
		private void HandleLowKick()
		{
			if (_lastVisibleLowKick < _controller.InterpolatedLowKickCount)
			{
				_animator.SetTrigger("LowKick");
			}
			else if (_lastVisibleLowKick > _controller.InterpolatedLowKickCount)
			{
				//Cancel LowKick
			}
			_lastVisibleLowKick = _controller.InterpolatedLowKickCount;

		}
		private void HandleFireBall()
		{
			if (_lastVisibleFireBall < _controller.InterpolatedFireBallCount)
			{
				_animator.SetTrigger("FireBall");
			}
			else if (_lastVisibleFireBall > _controller.InterpolatedFireBallCount)
			{
				//Cancel FireBall
			}
			_lastVisibleFireBall = _controller.InterpolatedFireBallCount;

		}
		

		private void HandleJump()
		{
			if (_lastVisibleJump < _controller.InterpolatedJumpCount)
			{
				_animator.SetTrigger("Jump");
			}
			else if (_lastVisibleJump > _controller.InterpolatedJumpCount)
			{
				// Cancel Jump
			}

			_lastVisibleJump = _controller.InterpolatedJumpCount;
		}
	}
}
