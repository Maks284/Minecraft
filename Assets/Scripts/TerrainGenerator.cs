using UnityEngine;

public static class TerrainGenerator
{
    public static BlockType[,,] GenerateTerrain(float xOffset, float yOffset)
    {
        var resault = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = Mathf.PerlinNoise((x/4f + xOffset) * .2f, (z/4f + yOffset) * .2f) * 25 + 10;

                for (int y = 0; y < height; y++)
                {
                    resault[x, y, z] = BlockType.Dirt;
                }
            }
        }

        return resault;
    }
}
