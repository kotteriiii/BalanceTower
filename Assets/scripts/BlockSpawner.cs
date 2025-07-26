using UnityEngine;
using UnityEngine.UI;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance; // シングルトンインスタンス

    public GameObject[] blockPrefabs;
    public Button rightButton;
    public Button leftButton;
    public Button rotateButton;
    public Button rotateReverseButton;
    public Button dropButton;

    private GameObject nextBlockPrefab; // 次ブロックのPrefab
    private GameObject nextBlockInstance; // 次ブロックのインスタンス

    public AudioClip dropSE; // ブロックが落ちる時のSE

    public float spawnX;
    public float spawnY;
    public float rotateAngle;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnRandomBlock()
    {
        // 現在の次ブロックをフィールドに生成
        spawnX = Random.Range(-0.3f, 0.3f);
        rotateAngle = Random.Range(-180f, 180f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, rotateAngle);

        GameObject blockObj = Instantiate(nextBlockPrefab, new Vector3(spawnX, spawnY, 0), rotation);

        Rigidbody2D rb2d = blockObj.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.gravityScale = 0f;
        }

        Block blockScript = blockObj.GetComponent<Block>();
        blockScript.rightButton = rightButton;
        blockScript.leftButton = leftButton;
        blockScript.rotateButton = rotateButton;
        blockScript.rotateReverseButton = rotateReverseButton;
        blockScript.dropButton = dropButton;
        blockScript.dropSE = dropSE;

        // 新しい次ブロックを決めてプレビュー更新
        GenerateNextBlock();
    }

    public void GenerateNextBlock()
    {
        // 次ブロックをランダムで決定
        int index = Random.Range(0, blockPrefabs.Length);
        nextBlockPrefab = blockPrefabs[index];

        // 既存プレビューを削除
        if (nextBlockInstance != null)
        {
            Destroy(nextBlockInstance);
        }

        // フィールド外にInstantiateして表示
        nextBlockInstance = Instantiate(nextBlockPrefab, new Vector3(5f, spawnY, 0), Quaternion.identity);

        // スケール調整（必要なら）
        nextBlockInstance.transform.localScale = Vector3.one * 0.85f;

        // Rigidbody2Dをオフにする（落下防止）
        Rigidbody2D rb2d = nextBlockInstance.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.simulated = false;
        }

        // Blockスクリプトをオフにする（操作防止）
        Block blockScript = nextBlockInstance.GetComponent<Block>();
        if (blockScript != null)
        {
            blockScript.enabled = false;
        }
    }

}
