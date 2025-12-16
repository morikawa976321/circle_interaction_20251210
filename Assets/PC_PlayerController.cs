using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PCPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;
    public float gravity = 9.81f;

    [Header("Look Settings")]
    public float mouseSensitivity = 15f;
    public Transform playerCamera;
    
    private float xRotation = 0f;
    private Animator animator;
    private CharacterController characterController;
    
    // Y軸（垂直）方向の速度を保持する変数
    private Vector3 verticalVelocity;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        // --- 0. 接地判定と重力リセット（ここが重要！） ---
        // 地面についているなら、Y軸の速度をリセット（完全に0にすると浮くことがあるので、わずかに下へ押す）
        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; 
        }

        // --- 1. マウス視点操作 ---
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);

        // --- 2. 移動入力 ---
        float moveZ = 0f;
        float moveX = 0f;

        if (Keyboard.current.iKey.isPressed) moveZ = 1f;
        else if (Keyboard.current.kKey.isPressed) moveZ = -1f;

        if (Keyboard.current.lKey.isPressed) moveX = 1f;
        else if (Keyboard.current.jKey.isPressed) moveX = -1f;

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        
        if (moveDirection.magnitude > 1f) moveDirection.Normalize();

        // --- 3. 移動実行 ---
        // 水平方向の移動
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 垂直方向（重力）の計算と適用
        verticalVelocity.y -= gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);

        // --- 4. アニメーション ---
        // 入力があるかどうかだけで判定（微細な滑りを無視するため）
        bool isInputting = (moveX != 0 || moveZ != 0);
        
        if (animator != null)
        {
            animator.SetBool("isWalking", isInputting);
        }
    }
}