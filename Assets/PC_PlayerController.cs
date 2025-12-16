using UnityEngine;
using UnityEngine.InputSystem; // ← これが必要です

public class PCPlayerController : MonoBehaviour
{
    public float speed = 3.0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // キーボードが接続されていない場合は何もしない（エラー防止）
        if (Keyboard.current == null) return;

        float moveZ = 0f;
        float moveX = 0f;

        // --- 新しいInput Systemでの記述 (IJKL) ---

        // 前進 (I)
        if (Keyboard.current.iKey.isPressed) moveZ = 1f;
        // 後退 (K)
        else if (Keyboard.current.kKey.isPressed) moveZ = -1f;

        // 右移動 (L)
        if (Keyboard.current.lKey.isPressed) moveX = 1f;
        // 左移動 (J)
        else if (Keyboard.current.jKey.isPressed) moveX = -1f;

        // --- 移動処理（変更なし） ---
        Vector3 movement = new Vector3(moveX, 0, moveZ);
        
        if (movement.magnitude > 1f) movement.Normalize();

        transform.Translate(movement * speed * Time.deltaTime);

        // --- アニメーション切り替え（変更なし） ---
        bool isMoving = movement.sqrMagnitude > 0;
        
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }
    }
}