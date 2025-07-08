using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;

    public float standingHeight = 2.0f;
    public float crouchingHeight = 1.0f;

    public float standingCameraY = 0.9f;
    public float crouchingCameraY = 0.45f;

    public float crouchSpeed = 5f;

    private bool isCrouching = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        // �����ύX�i�Ȃ߂炩�Ɂj
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSpeed);

        // �J�����ʒu�ύX
        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, isCrouching ? crouchingCameraY : standingCameraY, Time.deltaTime * crouchSpeed);
        cameraTransform.localPosition = camPos;
    }
}
