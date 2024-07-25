using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VolumetricFogAndMist2.Demos {

    public class FPS_Controller : MonoBehaviour {
        // References
        CharacterController characterController;
        Transform mainCamera;

        // Input Internals
        float inputHor;
        float inputVert;
        float mouseHor;
        float mouseVert;
        float mouseInvertX = 1;
        float mouseInvertY = -1;
        float camVertAngle;
        bool isGrounded = false;

        Vector3 jumpDirection = Vector3.zero;

        float sprint = 1f;
        public float sprintMax = 2f;
        public float airControl = 1.5f;
        public float jumpHeight = 10;
        public float gravity = 20f;

        // Character Stats
        public float characterHeight = 1.8f;
        public float cameraHeight = 1.7f;
        public float speed = 15;

        // Rotation Vars
        public float rotationSpeed = 2;
        public float mouseSensitivity = 1;

        // Start is called before the first frame update
        void Start() {
            // Assign refs
            characterController = gameObject.AddComponent<CharacterController>();
            mainCamera = Camera.main.transform;
            // Setup Char
            characterController.height = characterHeight;
            characterController.center = Vector3.up * characterHeight / 2;
            // Setup Cam
            mainCamera.position = transform.position + Vector3.up * characterHeight;
            mainCamera.rotation = Quaternion.identity;
            mainCamera.parent = transform;
            // Setup Cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update() {

            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height) return;

            isGrounded = characterController.isGrounded;

            // Get Input
            inputHor = Input.GetAxis("Horizontal");
            inputVert = Input.GetAxis("Vertical");
            mouseHor = Input.GetAxis("Mouse X");
            mouseVert = Input.GetAxis("Mouse Y");

            // Rotate player first
            transform.Rotate(0, mouseHor * rotationSpeed * mouseSensitivity * mouseInvertX, 0);

            // Construct the direction vector
            Vector3 moveDirection = transform.forward * inputVert + transform.right * inputHor;
            moveDirection *= speed;

            if (isGrounded) {
                // Increase sprint smoothly
                if (Input.GetKey(KeyCode.LeftShift)) {
                    if (sprint < sprintMax) sprint += 10 * Time.deltaTime;
                } else {
                    if (sprint > 1) sprint -= 10 * Time.deltaTime;
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    jumpDirection.y = jumpHeight;
                } else {
                    jumpDirection.y = -1;
                }
            } else {
                moveDirection *= airControl;
            }

            // Apply gravity continuously 
            jumpDirection.y -= gravity * Time.deltaTime;

            // Move the character using Move
            characterController.Move(moveDirection * sprint * Time.deltaTime);
            characterController.Move(jumpDirection * Time.deltaTime);

            // Rotate the camera up and down
            camVertAngle += mouseVert * rotationSpeed * mouseSensitivity * mouseInvertY;
            // Clamp value so we don't go over ourselves
            camVertAngle = Mathf.Clamp(camVertAngle, -85f, 85f);
            // Apply Rotation
            mainCamera.localEulerAngles = new Vector3(camVertAngle, 0f, 0f);
        }
    }

}