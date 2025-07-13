using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BlockSpawner blockSpawner; // インスペクターでBlockSpawnerをアサイン

    void Start()
    {
        // ゲーム開始時にブロックを1つ生成
        blockSpawner.SpawnRandomBlock();
    }

    // 必要に応じて、次のブロック生成やゲーム進行の管理もここで行えます
}
