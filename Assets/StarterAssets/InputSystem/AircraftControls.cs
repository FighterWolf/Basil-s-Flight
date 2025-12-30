using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class AircraftControls : MonoBehaviour
	{
		[Header("Character Input Values")]
		public float yaw;
		public float pitch;
		public float roll;
		public float throttle;
		public bool dismount;
		
		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM

		public void OnYaw(InputValue v)
		{
			YawInput(v.Get<float>());
		}

		public void OnPitch(InputValue v)
		{
			PitchInput(v.Get<float>());
		}

		public void OnRoll(InputValue v)
		{
			RollInput(v.Get<float>());
		}

		public void OnThrottle(InputValue v)
		{
			ThrottleInput(v.Get<float>());
		}

		public void OnDismount(InputValue v)
        {
			DismountInput(v.isPressed);
        }
#endif

		private void YawInput(float i)
        {
			yaw = i;
        }

		private void PitchInput(float i)
		{
			pitch = i;
		}

		private void RollInput(float i)
		{
			roll = i;
		}

		private void ThrottleInput(float i)
		{
			throttle = i;
		}

		private void DismountInput(bool b)
        {
			dismount = b;
        }

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

	}
	
}