using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance;

	[Space(10)]
	[Header("Variables")]
	[SerializeField] private float zoomSpeed;
	[SerializeField] private float zoomSmoothing;
	[SerializeField] private float minZoom;
	[SerializeField] private float maxZoom;
	[SerializeField] private float WSADSpeed;
	[SerializeField] private float verticalScrollSpeed;
	[SerializeField] private float horizontalScrollSpeed;

	[Space(10)]
	[Header("References")]
	[SerializeField] private Camera cam;
	[SerializeField] private GridBuilder gridBuilder;

	private Vector3 dragOrigin;
	private float zoomTargetPos;
	private float screenEdgeThreshold = 100f;

	private void Awake()
	{
		Instance = this;
		zoomTargetPos = cam.orthographicSize;
	}


	private void Update()
	{
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        HandleZoom();
        HandleMovement();
        HandleDragging();
	}


  	// Drag the camera around with middle mouse button
 	private void HandleDragging()
  	{
		if(Input.GetMouseButtonDown(2))
		{
			dragOrigin = Utilities.GetMousePositionInWorldSpace();
		}
		if(Input.GetMouseButton(2))
		{
			Vector3 difference = dragOrigin - (Vector3)Utilities.GetMousePositionInWorldSpace();
			cam.transform.position += difference;
			dragOrigin = Utilities.GetMousePositionInWorldSpace();
		}
  	}

	// Zoom with scroll
	private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            zoomTargetPos -= scroll * zoomSpeed;
            zoomTargetPos = Mathf.Clamp(zoomTargetPos, minZoom, maxZoom);
        }

        if (cam.orthographicSize != zoomTargetPos)
        {
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, zoomTargetPos, zoomSmoothing * Time.deltaTime);
        }
    }

	// Move with WSAD
	private void HandleMovement()
    {
        if (Input.anyKey)
        {
            Vector3 newPosition = cam.transform.position;

            if (Input.GetKey(KeyCode.W)) { newPosition.y += WSADSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.S)) { newPosition.y -= WSADSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.A)) { newPosition.x -= WSADSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.D)) { newPosition.x += WSADSpeed * Time.deltaTime; }

            cam.transform.position = newPosition;
        }
    }

	public void PushCameraWithMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 currentPosition = cam.transform.position;

        if (mousePosition.x >= Screen.width - screenEdgeThreshold) { currentPosition.x += WSADSpeed * Time.deltaTime; }
        if (mousePosition.x <= screenEdgeThreshold) { currentPosition.x -= WSADSpeed * Time.deltaTime; }
        if (mousePosition.y >= Screen.height - screenEdgeThreshold) { currentPosition.y += WSADSpeed * Time.deltaTime; }
        if (mousePosition.y <= screenEdgeThreshold) { currentPosition.y -= WSADSpeed * Time.deltaTime; }

        cam.transform.position = currentPosition;
    }

	public void SetCameraPosition(Vector3 position, bool animate = true)
	{
		position.z = -15;

		if (animate)
		{
			LeanTween.cancel(transform.gameObject);
			LeanTween.move(cam.transform.gameObject, position, 0.5f).setEaseInOutSine();
		}
		else
		{
			cam.transform.position = position;
		}  
	}
}
