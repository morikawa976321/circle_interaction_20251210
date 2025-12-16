using UnityEngine;

public class SimpleCircleController : MonoBehaviour
{
    public int mode;
    public Transform targetA;
    public Transform targetB;
    public Transform quadA;
    public Transform quadB;

    // 円のデフォルト半径（メートル）
    // QuadのScaleが1のとき、直径1m(半径0.5m)なので、Scale=2で半径1mになります
    private float defaultRadius = 1.6f; 

    // 縮小の負担比率 (A: 0.5, B: 1.0)
    private float ratioA = 1.0f;
    private float ratioB = 1.0f;

    void Update()
    {
        // 0. モードと二人の位置に応じて縮小比率を変更
        if (mode == 0){
            if (0 < targetA.position.z && targetA.position.z < 9){
                defaultRadius = 1.6f;
                ratioA = 1.0f;
                ratioB = 1.5f;
            } else if (9 <= targetA.position.z && targetA.position.z <= 18){
                defaultRadius = 0.0f;
            } else if (18 < targetA.position.z && targetA.position.z < 27){
                defaultRadius = 1.6f;
                ratioA = 1.0f;
                ratioB = 1.0f;
            } else if (27 <= targetA.position.z && targetA.position.z <= 36){
                defaultRadius = 0.0f;
            } else if (36 < targetA.position.z && targetA.position.z < 45){
                defaultRadius = 1.6f;
                ratioA = 1.0f;
                ratioB = 0.5f;
            }
        } else if (mode == 1){
            if (0 < targetA.position.z && targetA.position.z < 9){
                defaultRadius = 1.6f;
                ratioA = 1.0f;
                ratioB = 0.5f;
            } else if (9 <= targetA.position.z && targetA.position.z <= 18){
                defaultRadius = 0.0f;
            } else if (18 < targetA.position.z && targetA.position.z < 27){
                defaultRadius = 1.6f;
                ratioA = 1.0f;
                ratioB = 1.0f;
            } else if (27 <= targetA.position.z && targetA.position.z <= 36){
                defaultRadius = 0.0f;
            } else if (36 < targetA.position.z && targetA.position.z < 45){
                defaultRadius = 1.6f;
                ratioA = 1.0f;
                ratioB = 1.5f;
            }
        }

        // 1. 距離を測る
        float dist = Vector2.Distance(
            new Vector2(targetA.position.x, targetA.position.z),
            new Vector2(targetB.position.x, targetB.position.z)
        );

        // 2つの円がちょうど接する距離（デフォルト半径の合計）
        float contactDist = defaultRadius * 2f;

        float radiusA = defaultRadius;
        float radiusB = defaultRadius;

        // 2. 距離が「接する距離」より近い場合のみ計算
        if (dist < contactDist)
        {
            // 重なってしまった量（オーバーラップ）を算出
            float overlap = contactDist - dist;

            // 比率の合計 (0.5 + 1.0 = 1.5)
            float totalRatio = ratioA + ratioB;

            // オーバーラップ分を比率に応じて配分し、半径から引く
            float shrinkA = overlap * (ratioA / totalRatio);
            float shrinkB = overlap * (ratioB / totalRatio);

            radiusA = Mathf.Max(0f, defaultRadius - shrinkA);
            radiusB = Mathf.Max(0f, defaultRadius - shrinkB);
        }

        // 3. 反映 (Quadのスケールは「直径」なので半径の2倍を入れる)
        Apply(quadA, targetA, radiusA * 2f);
        Apply(quadB, targetB, radiusB * 2f);
    }

    void Apply(Transform q, Transform t, float diameter)
    {
        q.position = new Vector3(t.position.x, 0.005f, t.position.z);
        q.localScale = new Vector3(diameter, diameter, 1f);
    }
}