using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool canMove = false;
    public float cameraSpeed = 8f;
    public float cameraRotateSpeed = 1f;
    public float mouseSensitivity = 0.05f;

    public Vector2 yLimits = new Vector2(5f, 20f);

    public float lastY;
    public Vector3 lastPosition;

    void Start()
    {
        lastY = transform.position.y;
    }

    void Update()
    {
        if (canMove)
        {
            if (!Input.GetMouseButton(2))
            {
                Rotate();
                Move();
                Zoom();
            }
            MoveWithMouse();
        }
    }

    public void Move()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 deltaSpeed = new Vector3(0f, 0f, 0f);

        if (hor != 0) deltaSpeed.x += hor * Time.deltaTime;
        if (ver != 0) deltaSpeed.z += ver * Time.deltaTime;

        transform.position += deltaSpeed * cameraSpeed;
    }

    public void Rotate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 centerPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, lastY));
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.RotateAround(centerPoint, Vector3.up, cameraRotateSpeed * delta.x);
            transform.position = new Vector3(transform.position.x, lastY, transform.position.z);
            lastPosition = Input.mousePosition;
        }
    }

    public void Zoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                if (transform.position.y + (transform.forward.y * Input.mouseScrollDelta.y) > yLimits.x) transform.position += transform.forward * Input.mouseScrollDelta.y;
                else transform.position = new Vector3(transform.position.x, yLimits.x, transform.position.z);
                
            } else
            {
                if (transform.position.y + (transform.forward.y * Input.mouseScrollDelta.y) < yLimits.y) transform.position += transform.forward * Input.mouseScrollDelta.y;
                else transform.position = new Vector3(transform.position.x, yLimits.y, transform.position.z);
            }
            lastY = transform.position.y;
        }
    }

    public void MoveWithMouse()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
            lastY = transform.position.y;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, 0f);
            transform.position = new Vector3(transform.position.x, lastY, transform.position.z);
            lastPosition = Input.mousePosition;
        }
    }
}
