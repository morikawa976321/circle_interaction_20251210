using UnityEngine;
using System.IO;
using System.Text;
using System; // 日時取得のために追加

public class FrameDataLogger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ファイル名のベース（これに日時が付加されます）")]
    public string fileNameBase = "LogCircleInteraction.csv";
    
    [Header("Targets")]
    public Transform object1;
    public Transform object2;

    private StreamWriter writer;
    private Vector3 prevPos1;
    private Vector3 prevPos2;
    private string filePath;

    void Start()
    {
        // 1. 現在の日時を取得 (形式: yyyyMMdd_HHmmss)
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        // 2. ファイル名を生成 (例: ObjectMovementLog_20231217_153000.csv)
        // 拡張子(.csv)とファイル名の部分を分けて、間に日時を挟みます
        string nameWithoutExt = Path.GetFileNameWithoutExtension(fileNameBase);
        string extension = Path.GetExtension(fileNameBase);
        
        // もしインスペクターで拡張子が書かれていなければ自動で.csvをつける
        if (string.IsNullOrEmpty(extension)) extension = ".csv";

        string uniqueFileName = $"{nameWithoutExt}_{timestamp}{extension}";

        // 3. 保存先のフルパスを作成
        // Application.dataPath はプロジェクトのAssetsフォルダ（エディタ時）を指します
        filePath = Path.Combine(Application.dataPath, uniqueFileName);

        // ファイル書き込みの準備
        try
        {
            writer = new StreamWriter(filePath, false, Encoding.UTF8);
            
            // ヘッダー書き込み
            writer.WriteLine("Frame,Distance,Obj1_Dir_X,Obj1_Dir_Y,Obj1_Dir_Z,Obj2_Dir_X,Obj2_Dir_Y,Obj2_Dir_Z");
            
            Debug.Log($"[DataLogger] Recording started. File saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataLogger] Failed to open file: {e.Message}");
            this.enabled = false;
            return;
        }

        // 初期の位置を保存
        if (object1 != null) prevPos1 = object1.position;
        if (object2 != null) prevPos2 = object2.position;
    }

    void Update()
    {
        if (object1 == null || object2 == null) return;

        // 距離と移動方向の計算
        float distance = Vector3.Distance(object1.position, object2.position);

        Vector3 movementDelta1 = object1.position - prevPos1;
        Vector3 direction1 = (movementDelta1.magnitude > 0) ? movementDelta1.normalized : Vector3.zero;

        Vector3 movementDelta2 = object2.position - prevPos2;
        Vector3 direction2 = (movementDelta2.magnitude > 0) ? movementDelta2.normalized : Vector3.zero;

        // CSVデータの作成
        string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
            Time.frameCount,
            distance,
            direction1.x, direction1.y, direction1.z,
            direction2.x, direction2.y, direction2.z
        );

        // 書き込み
        writer.WriteLine(line);

        // 位置情報の更新
        prevPos1 = object1.position;
        prevPos2 = object2.position;
    }

    void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
            Debug.Log($"[DataLogger] Finished. Log saved: {filePath}");
        }
    }
}