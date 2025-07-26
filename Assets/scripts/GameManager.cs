using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // シングルトンインスタンス

    public GameObject gameOverCanvas; // ゲームオーバーキャンバス
    public Button fastForwardButton; // 倍速ボタン
    public TextMeshProUGUI resultText; // ゲームオーバー時のブロック数表示用テキスト
    public TextMeshProUGUI blockCountText; // ブロック数表示用テキスト
    public TextMeshProUGUI highScoreText; // ハイスコア表示用テキスト
    public GameObject stage;

    public AudioClip playingBGM; // ゲームプレイ中のBGM
    public AudioClip gameOverBGM; // ゲームオーバー時のBGM

    public float gameOverYThreshold = -10f; // ゲームオーバー判定のY座標閾値
    public int blockCount = -1; // 現在のブロック数
    public int highScore;  
    private float stopTimer = 0f; //停止判定に使う
    public float stopTimeRequired = 2f; // 停止判定の閾値

    public bool isGameOver = false; // ゲームオーバー状態かどうか
    private bool isFastForwarding = false; // 倍速状態かどうか

    void Awake()
    {
        // インスタンスを設定
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameOverCanvas.SetActive(false); // ゲームオーバーパネルを非表示にする

    }

    void Start()
    {
        BGMManager.Instance.PlayBGM(playingBGM);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = $"最高記録: {highScore}";

        // EventTrigger設定
        EventTriggerUtility.AddEventTrigger(fastForwardButton.gameObject, EventTriggerType.PointerDown, (data) => { isFastForwarding = true; });
        EventTriggerUtility.AddEventTrigger(fastForwardButton.gameObject, EventTriggerType.PointerUp, (data) => { isFastForwarding = false; });

        // 次ブロックを生成
        BlockSpawner.Instance.GenerateNextBlock();

        // ゲーム開始時にブロックを1つ生成
        isGameOver = false;
        SpawnNextBlock();
    }

    void Update()
    {
        if (!isGameOver)
        {
            CheckGameOver(); // ゲームオーバー判定
        }

        // 全てのブロックが停止しているかチェック
        if (BlockManager.Instance.AreAllBlocksStopped() && !isGameOver)
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= stopTimeRequired)
            {
                AdjustCamera(BlockManager.Instance.GetHighestBlockY());
                SpawnNextBlock();
                stopTimer = 0f; // リセット
            }
        }
        else
        {
            stopTimer = 0f; // 動いている間はリセット
        }

        // 倍速状態の処理
        if (isFastForwarding)
        {
            Time.timeScale = 2f; // 倍速
        }
        else
        {
            Time.timeScale = 1f; // 通常速度
        }
    }

    void CheckGameOver()
    {
        // Scene上のBlockを全検索
        Block[] blocks = FindObjectsByType<Block>(FindObjectsSortMode.None);
        foreach (Block block in blocks)
        {
            if (block.IsBelowThreshold(gameOverYThreshold))
            {
                GameOver();
                break;
            }
        }
    }

    public void SpawnNextBlock()
    {
        BlockSpawner.Instance.SpawnRandomBlock();
        blockCount++; // ブロック数をカウントアップ
        blockCountText.text = $"{blockCount}";
    }

    // カメラの位置とサイズを調整。スポーン位置の調整もおこなう。
    public void AdjustCamera(float highestBlockY)
    {
        float high = highestBlockY + 4f;
        float stageY = stage.transform.position.y; // ステージの固定y座標

        // カメラサイズが足りない場合だけ拡大する
        if (high > Camera.main.orthographicSize)
        {
            Camera.main.orthographicSize = high;

            // カメラ位置を、stageY が画面下から desiredStageRatio に来るように調整
            float cameraY = stageY + Camera.main.orthographicSize * 0.5f;

            Camera.main.transform.position = new Vector3(0, cameraY, -10);

            // BlockSpawner の spawnY を画面上端の少し下あたりに配置
            BlockSpawner.Instance.spawnY = cameraY + Camera.main.orthographicSize * 0.85f;
        }
    }

    public void GameOver()
    {
        isGameOver = true; // ゲームオーバー状態にする

        // 今回の記録が高ければハイスコアを更新
        if (blockCount > highScore)
        {
            PlayerPrefs.SetInt("HighScore", blockCount);
            PlayerPrefs.Save(); // 忘れずに保存
        }

        BGMManager.Instance.PlayBGM(gameOverBGM); // ゲームオーバー時のBGMを再生
        resultText.text = $"あなたの記録は{blockCount}個です！"; // ゲームオーバー時のブロック数を表示
        gameOverCanvas.SetActive(true); // ゲームオーバーパネルを表示
    }

    // ゲームオーバー後にリトライするメソッド
    public void RetryGame()
    {
        // シングルトン破棄（無いとリトライしてもstartメソッドが呼ばれない）
        Destroy(gameObject);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

}
