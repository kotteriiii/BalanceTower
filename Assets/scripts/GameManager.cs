using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // �V���O���g���C���X�^���X

    public GameObject gameOverCanvas; // �Q�[���I�[�o�[�L�����o�X
    public Button fastForwardButton; // �{���{�^��
    public TextMeshProUGUI resultText; // �Q�[���I�[�o�[���̃u���b�N���\���p�e�L�X�g
    public TextMeshProUGUI blockCountText; // �u���b�N���\���p�e�L�X�g
    public TextMeshProUGUI highScoreText; // �n�C�X�R�A�\���p�e�L�X�g
    public GameObject stage;

    public AudioClip playingBGM; // �Q�[���v���C����BGM
    public AudioClip gameOverBGM; // �Q�[���I�[�o�[����BGM

    public float gameOverYThreshold = -10f; // �Q�[���I�[�o�[�����Y���W臒l
    public int blockCount = -1; // ���݂̃u���b�N��
    public int highScore;  
    private float stopTimer = 0f; //��~����Ɏg��
    public float stopTimeRequired = 2f; // ��~�����臒l

    public bool isGameOver = false; // �Q�[���I�[�o�[��Ԃ��ǂ���
    private bool isFastForwarding = false; // �{����Ԃ��ǂ���

    void Awake()
    {
        // �C���X�^���X��ݒ�
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameOverCanvas.SetActive(false); // �Q�[���I�[�o�[�p�l�����\���ɂ���

    }

    void Start()
    {
        BGMManager.Instance.PlayBGM(playingBGM);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = $"�ō��L�^: {highScore}";

        // EventTrigger�ݒ�
        EventTriggerUtility.AddEventTrigger(fastForwardButton.gameObject, EventTriggerType.PointerDown, (data) => { isFastForwarding = true; });
        EventTriggerUtility.AddEventTrigger(fastForwardButton.gameObject, EventTriggerType.PointerUp, (data) => { isFastForwarding = false; });

        // ���u���b�N�𐶐�
        BlockSpawner.Instance.GenerateNextBlock();

        // �Q�[���J�n���Ƀu���b�N��1����
        isGameOver = false;
        SpawnNextBlock();
    }

    void Update()
    {
        if (!isGameOver)
        {
            CheckGameOver(); // �Q�[���I�[�o�[����
        }

        // �S�Ẵu���b�N����~���Ă��邩�`�F�b�N
        if (BlockManager.Instance.AreAllBlocksStopped() && !isGameOver)
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= stopTimeRequired)
            {
                AdjustCamera(BlockManager.Instance.GetHighestBlockY());
                SpawnNextBlock();
                stopTimer = 0f; // ���Z�b�g
            }
        }
        else
        {
            stopTimer = 0f; // �����Ă���Ԃ̓��Z�b�g
        }

        // �{����Ԃ̏���
        if (isFastForwarding)
        {
            Time.timeScale = 2f; // �{��
        }
        else
        {
            Time.timeScale = 1f; // �ʏ푬�x
        }
    }

    void CheckGameOver()
    {
        // Scene���Block��S����
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
        blockCount++; // �u���b�N�����J�E���g�A�b�v
        blockCountText.text = $"{blockCount}";
    }

    // �J�����̈ʒu�ƃT�C�Y�𒲐��B�X�|�[���ʒu�̒����������Ȃ��B
    public void AdjustCamera(float highestBlockY)
    {
        float high = highestBlockY + 4f;
        float stageY = stage.transform.position.y; // �X�e�[�W�̌Œ�y���W

        // �J�����T�C�Y������Ȃ��ꍇ�����g�傷��
        if (high > Camera.main.orthographicSize)
        {
            Camera.main.orthographicSize = high;

            // �J�����ʒu���AstageY ����ʉ����� desiredStageRatio �ɗ���悤�ɒ���
            float cameraY = stageY + Camera.main.orthographicSize * 0.5f;

            Camera.main.transform.position = new Vector3(0, cameraY, -10);

            // BlockSpawner �� spawnY ����ʏ�[�̏�����������ɔz�u
            BlockSpawner.Instance.spawnY = cameraY + Camera.main.orthographicSize * 0.85f;
        }
    }

    public void GameOver()
    {
        isGameOver = true; // �Q�[���I�[�o�[��Ԃɂ���

        // ����̋L�^��������΃n�C�X�R�A���X�V
        if (blockCount > highScore)
        {
            PlayerPrefs.SetInt("HighScore", blockCount);
            PlayerPrefs.Save(); // �Y�ꂸ�ɕۑ�
        }

        BGMManager.Instance.PlayBGM(gameOverBGM); // �Q�[���I�[�o�[����BGM���Đ�
        resultText.text = $"���Ȃ��̋L�^��{blockCount}�ł��I"; // �Q�[���I�[�o�[���̃u���b�N����\��
        gameOverCanvas.SetActive(true); // �Q�[���I�[�o�[�p�l����\��
    }

    // �Q�[���I�[�o�[��Ƀ��g���C���郁�\�b�h
    public void RetryGame()
    {
        // �V���O���g���j���i�����ƃ��g���C���Ă�start���\�b�h���Ă΂�Ȃ��j
        Destroy(gameObject);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

}
