using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject[] blockPrefabs; // インスペクターでPrefabを複数登録

    public void SpawnRandomBlock()
    {
        int index = Random.Range(0, blockPrefabs.Length);
        Instantiate(blockPrefabs[index], new Vector3(0, 10, 0), Quaternion.identity);
    }
}