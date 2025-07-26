using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // EventTrigger用

public class Block : MonoBehaviour
{
    public Button rightButton;
    public Button leftButton;
    public Button rotateButton;
    public Button rotateReverseButton;
    public Button dropButton;

    public AudioClip dropSE; // ブロックが落ちる時のサウンド

    public float moveSpeed;
    public float rotateAngle;

    private Rigidbody2D rb2d;
    private bool isFalling = false;

    private bool isMovingRight = false;
    private bool isMovingLeft = false;
    private bool isRotating = false;
    private bool isRotatingReverse = false; 

    public float stopThreshold = 0.01f; // これ以下の速度で停止判定



    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        // BlockManager に登録
        BlockManager.Instance.RegisterBlock(this);

        // EventTrigger設定
        EventTriggerUtility.AddEventTrigger(rightButton.gameObject, EventTriggerType.PointerDown, (data) => { isMovingRight = true; });
        EventTriggerUtility.AddEventTrigger(rightButton.gameObject, EventTriggerType.PointerUp, (data) => { isMovingRight = false; });

        EventTriggerUtility.AddEventTrigger(leftButton.gameObject, EventTriggerType.PointerDown, (data) => { isMovingLeft = true; });
        EventTriggerUtility.AddEventTrigger(leftButton.gameObject, EventTriggerType.PointerUp, (data) => { isMovingLeft = false; });

        EventTriggerUtility.AddEventTrigger(rotateButton.gameObject, EventTriggerType.PointerDown, (data) => { isRotating = true; });
        EventTriggerUtility.AddEventTrigger(rotateButton.gameObject, EventTriggerType.PointerUp, (data) => { isRotating = false; });

        EventTriggerUtility.AddEventTrigger(rotateReverseButton.gameObject, EventTriggerType.PointerDown, (data) => { isRotatingReverse = true; });
        EventTriggerUtility.AddEventTrigger(rotateReverseButton.gameObject, EventTriggerType.PointerUp, (data) => { isRotatingReverse = false; });

        // DropボタンはクリックのみでOK
        dropButton.onClick.AddListener(DropBlock);

        moveSpeed = 5f; // 移動速度
        rotateAngle = 1.2f; // 回転角度
    }

    void Update()
    {
        if (!isFalling)
        {
            if (isMovingRight)
            {
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
            if (isMovingLeft)
            {
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            if (isRotating)
            {
                transform.Rotate(Vector3.forward, rotateAngle);
            }
            if (isRotatingReverse)
            {
                transform.Rotate(Vector3.forward, -rotateAngle);
            }
        }        
    }

    public bool IsStopped() // ブロックが停止しているかどうかを判定するメソッド
    {
        if (rb2d == null)
        {
            return false;
        }

        if (!isFalling)
        {
            return false;
        }
        float speed = rb2d.linearVelocity.magnitude;
        return speed < stopThreshold;
    }

    public void DropBlock()　
    {
        if (!isFalling)
        {
            isFalling = true;
            rb2d.gravityScale = 0.5f;
            SEManager.Instance.PlaySE(dropSE); // ブロックが落ちる時のサウンドを再生

        }
    }

    public bool IsBelowThreshold(float threshold) // ゲームオーバー判定用メソッド
    {
        return transform.position.y <= threshold;
    }
}
