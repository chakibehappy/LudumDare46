using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    GameMaster GM;

    public float deathCameraSpeed = 10.0f;
    public bool lockCursor;
    public float mouseSensitivity = 5;
    public Transform target;
    public float distFromTarget = 2;
    public Vector2 pitchRange = new Vector2(-40, 85);

    public float rotationSmoothTime = 0.12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRot;

    float yaw;
    float pitch;

    private void Start()
    {
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if(GM.HeartPoint > 0)
        {
            CameraControl();
        }
        else
        {
            GameOverCamera();
        }
    }

    void CameraControl()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchRange.x, pitchRange.y);

        currentRot = Vector3.SmoothDamp(currentRot, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRot;

        transform.position = target.position - transform.forward * distFromTarget;
    }

    void GameOverCamera()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yaw += Time.fixedDeltaTime * deathCameraSpeed;

        currentRot = Vector3.SmoothDamp(currentRot, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRot;

        transform.position = target.position - transform.forward * distFromTarget;
    }
}
