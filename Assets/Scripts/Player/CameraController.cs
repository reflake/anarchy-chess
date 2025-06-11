using UnityEngine;

namespace Player
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private float baseSensitivity;

		private void Update()
		{
			if (Input.GetButton("Fire2"))
			{
				float yawDelta = Input.GetAxis("Mouse X");
				float pitchDelta = Input.GetAxis("Mouse Y");
				float speed = Time.unscaledDeltaTime * baseSensitivity;
				
				float newYaw = transform.eulerAngles.y + yawDelta * speed;
				float pitch = transform.eulerAngles.x;

				// Normalize pitch
				if (pitch > 180)
				{
					pitch -= 360;
				}
				
				float newPitch = pitch - pitchDelta * speed;

				const float pitchLimit = 80;
				newPitch = Mathf.Clamp(newPitch, -pitchLimit, pitchLimit);
				
				transform.eulerAngles = new Vector3(newPitch, newYaw, 0);
			}
		}
	}
}