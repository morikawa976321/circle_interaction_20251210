using UnityEngine;
using System.IO;
using System.Text;
using System;

public class FrameDataLogger : MonoBehaviour
{
    [Header("Settings")]
    public string fileNameBase = "LogCircleInteraction.csv";
    
    [Header("Targets")]
    public Transform object1;
    public Transform object2;

    // ★ 1. SimpleCircleController への参照を追加
    [Header("Script Reference")]
    public SimpleCircleController circleController;

    private StreamWriter writer;
    private Vector3 prevPos1;
    private Vector3 prevPos2;
    private string filePath;

    void Start()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string nameWithoutExt = Path.GetFileNameWithoutExtension(fileNameBase);
        string extension = Path.GetExtension(fileNameBase);
        if (string.IsNullOrEmpty(extension)) extension = ".csv";
        string uniqueFileName = $"{nameWithoutExt}_{timestamp}{extension}";
        filePath = Path.Combine(Application.dataPath, uniqueFileName);

        try
        {
            writer = new StreamWriter(filePath, false, Encoding.UTF8);
            
            // ★ 2. ヘッダーに Mode と Room を追加
            writer.WriteLine("Frame,Distance,Obj1_Dir_X,Obj1_Dir_Y,Obj1_Dir_Z,Obj2_Dir_X,Obj2_Dir_Y,Obj2_Dir_Z,Mode,Room");
            
            Debug.Log($"[DataLogger] Recording started. File saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataLogger] Failed to open file: {e.Message}");
            this.enabled = false;
            return;
        }

        if (object1 != null) prevPos1 = object1.position;
        if (object2 != null) prevPos2 = object2.position;
    }

    void Update()
    {
        // circleController がセットされていない場合は処理しない
        if (object1 == null || object2 == null || circleController == null) return;

        float distance = Vector3.Distance(object1.position, object2.position);

        Vector3 movementDelta1 = object1.position - prevPos1;
        Vector3 direction1 = (movementDelta1.magnitude > 0) ? movementDelta1.normalized : Vector3.zero;

        Vector3 movementDelta2 = object2.position - prevPos2;
        Vector3 direction2 = (movementDelta2.magnitude > 0) ? movementDelta2.normalized : Vector3.zero;

        // ★ 3. CSVデータの作成 (末尾に {8} と {9} を追加)
        string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
            Time.frameCount,
            distance,
            direction1.x, direction1.y, direction1.z,
            direction2.x, direction2.y, direction2.z,
            circleController.mode, // 追加
            circleController.room  // 追加
        );

        writer.WriteLine(line);

        prevPos1 = object1.position;
        prevPos2 = object2.position;
    }

    void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
        }
    }
}