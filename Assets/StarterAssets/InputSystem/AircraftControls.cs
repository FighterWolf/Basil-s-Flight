using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class AircraftControls : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 look;
		public bool allowLook;
		public float yaw;
		public float pitch;
		public float roll;
		public float throttle;
		public bool dismount;
		public bool fire;
		public bool switchWeapon;
		
		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnAllowLook(InputValue v)
		{
			AllowLookInput(v.isPressed);
		}

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

		public void OnFire(InputValue v)
        {
			FireInput(v.isPressed);
        }

		public void OnSwitchWeapon(InputValue v)
        {
			SwitchWeaponInput(v.isPressed);
        }
#endif
		private void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		private void AllowLookInput(bool b)
        {
			allowLook = b;
        }

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

		private void FireInput(bool b)
        {
			fire = b;
        }

		private void SwitchWeaponInput(bool b)
		{
			switchWeapon = b;
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