using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;

    private List<Block> blocks = new List<Block>();

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

    public void RegisterBlock(Block block)
    {
        blocks.Add(block);
    }

    public bool AreAllBlocksStopped()
    {
        foreach (Block block in blocks)
        {
            if (!block.IsStopped())
            {
                return false; // 1‚Â‚Å‚à“®‚¢‚Ä‚¢‚ê‚Îfalse
            }
        }
        return true;
    }

    public float GetHighestBlockY()
    {
        float highest = float.MinValue; // ‰Šú’l‚ðÅ¬’l‚ÉÝ’è
        foreach (Block block in blocks)
        {
            if (block.transform.position.y > highest)
            {
                highest = block.transform.position.y;
            }
        }
        return highest;
    }
}
