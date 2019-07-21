using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Block
{
    public Vector3 position;
    public BlockType type;

    public Block(Vector3 position, BlockType type)
    {
        this.position = position;
        this.type = type;
    }

    public Block(BlockType type)
    {
        position = Vector3.zero;
        this.type = type;
    }
}
