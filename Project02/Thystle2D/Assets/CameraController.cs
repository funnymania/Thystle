using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float panSpeed = 20f;
	[SerializeField] private float panBorderThickness = 10f;

	private bool canPan = true;

	private void Awake()
	{

	}

	private void Update()
	{
		if (canPan)
			UpdatePanControls();

		if (Keyboard.current.f4Key.wasPressedThisFrame)
			canPan = !canPan;

		if (Keyboard.current.escapeKey.wasPressedThisFrame)
			Application.Quit();
	}

	private void UpdatePanControls()
	{
		Vector3 cameraPos = transform.position;

		if (Mouse.current.position.ReadValue().y >= Screen.height - panBorderThickness)
		{
			cameraPos.y += panSpeed * Time.deltaTime;
		}

		if (Mouse.current.position.ReadValue().y <= panBorderThickness)
		{
			cameraPos.y -= panSpeed * Time.deltaTime;
		}

		if (Mouse.current.position.ReadValue().x >= Screen.width - panBorderThickness)
		{
			cameraPos.x += panSpeed * Time.deltaTime;
		}

		if (Mouse.current.position.ReadValue().x <= panBorderThickness)
		{
			cameraPos.x -= panSpeed * Time.deltaTime;
		}

		transform.position = cameraPos;
	}

	//Gets clamped by Map Bounds every update
	public static void SetCameraPosition(Vector2 newPos)
	{
		Camera.main.transform.position = new Vector3(newPos.x, newPos.y, -10);
	}

	public static Vector2 GetCameraPosition()
	{
		return new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
	}
}
