using UnityEngine;
using System;

[Serializable]
public class Block
{
    [Serializable]
    public enum BlockType
    {
        Air,
        Dirt,
        Grass
    }

    public BlockType type;
    public bool isSolid;

    public Block(BlockType type)
    {
        this.type = type;
        SetBlockProperties();
    }

    private void SetBlockProperties()
    {
        switch (type)
        {
            case BlockType.Dirt:
                isSolid = true;
                break;

            case BlockType.Grass:
                isSolid = true;
                break;
        }
    }
}
