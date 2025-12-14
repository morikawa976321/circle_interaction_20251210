using UnityEngine;

/// <summary>
/// このスクリプトをアタッチしたオブジェクトのY座標を、
/// Y座標を一定に固定します。
/// </summary>

public class FixHeight : MonoBehaviour
{
    public float height = 0.75f;

    // Updateの後に呼ばれるLateUpdateを使用
    void LateUpdate()
    {
        // 現在のX座標とZ座標はそのまま使い、Y座標だけを起動時の値(startY)に固定する
        transform.position = new Vector3(
            transform.position.x, 
            height, 
            transform.position.z
        );
    }
}