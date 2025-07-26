using UnityEngine;
using UnityEngine.UI;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance; // �V���O���g���C���X�^���X

    public GameObject[] blockPrefabs;
    public Button rightButton;
    public Button leftButton;
    public Button rotateButton;
    public Button rotateReverseButton;
    public Button dropButton;

    private GameObject nextBlockPrefab; // ���u���b�N��Prefab
    private GameObject nextBlockInstance; // ���u���b�N�̃C���X�^���X

    public AudioClip dropSE; // �u���b�N�������鎞��SE

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
        // ���݂̎��u���b�N���t�B�[���h�ɐ���
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

        // �V�������u���b�N�����߂ăv���r���[�X�V
        GenerateNextBlock();
    }

    public void GenerateNextBlock()
    {
        // ���u���b�N�������_���Ō���
        int index = Random.Range(0, blockPrefabs.Length);
        nextBlockPrefab = blockPrefabs[index];

        // �����v���r���[���폜
        if (nextBlockInstance != null)
        {
            Destroy(nextBlockInstance);
        }

        // �t�B�[���h�O��Instantiate���ĕ\��
        nextBlockInstance = Instantiate(nextBlockPrefab, new Vector3(5f, spawnY, 0), Quaternion.identity);

        // �X�P�[�������i�K�v�Ȃ�j
        nextBlockInstance.transform.localScale = Vector3.one * 0.85f;

        // Rigidbody2D���I�t�ɂ���i�����h�~�j
        Rigidbody2D rb2d = nextBlockInstance.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.simulated = false;
        }

        // Block�X�N���v�g���I�t�ɂ���i����h�~�j
        Block blockScript = nextBlockInstance.GetComponent<Block>();
        if (blockScript != null)
        {
            blockScript.enabled = false;
        }
    }

}
