using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 25;
    public const int ChunkHeight = 128;
    public const float BlockScale = .25f;

    public ChunkData ChunkData;
    public GameWorld ParenWorld;

    private Mesh chunkMesh;

    private List<Vector3> verticies= new();   
    private List<Vector2> uvs= new();
    private List<int> triangles= new();

    void Start()
    {
        chunkMesh = new Mesh();

        RegenerateMesh();

        GetComponent<MeshFilter>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Stone;
        RegenerateMesh();
    }

    public void DestroyBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }

    private void RegenerateMesh()
    {
        verticies.Clear();
        uvs.Clear();
        triangles.Clear();

        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.triangles = triangles.ToArray();
        chunkMesh.uv= uvs.ToArray();    

        chunkMesh.Optimize();
        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }


    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new(x, y, z);

        BlockType blockType = GetBlockAtPosition(blockPosition);

        if (GetBlockAtPosition(blockPosition) == 0)
        {
            return;
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateRightSide(blockPosition);
            AddUvs(blockType, Vector3Int.right);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(blockType, Vector3Int.left);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(blockType, Vector3Int.forward);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(blockType, Vector3Int.back);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(blockType, Vector3Int.up);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(blockType, Vector3Int.down);
        }

    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 ||  blockPosition.y >= ChunkHeight)
            {
                return BlockType.Air;
            }

            Vector2Int adjacementChunkPosition = ChunkData.ChunkPosition;

            if (blockPosition.x < 0)
            {
                adjacementChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if (blockPosition.x >= ChunkWidth)
            {
                adjacementChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }

            if (blockPosition.z < 0)
            {
                adjacementChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if (blockPosition.z >= ChunkWidth)
            {
                adjacementChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }

            if (ParenWorld.ChunkDatas.TryGetValue(adjacementChunkPosition, out ChunkData adjacementChunk))
            {
                return adjacementChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockType.Air;
            }
      
        }
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddVerticiesSquare();
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddVerticiesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddVerticiesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        
        AddVerticiesSquare();
    }

    private void GenerateTopSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddVerticiesSquare();
    }

    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);

        AddVerticiesSquare();
    }

    private void AddVerticiesSquare()
    {
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }

    private void AddUvs(BlockType blockType, Vector3Int normal)
    {
        Vector2 uv;
        if (blockType == BlockType.Dirt)
        {
            uv = new Vector2(32f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Stone)
        {
            uv = new Vector2(16f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Grass)
        {
            uv = normal == Vector3Int.up ? new Vector2(32f / 256, 240f / 256) :
                normal == Vector3Int.down ? new Vector2(64f / 256, 240f / 256) :
                new Vector2(64f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Wood)
        {
            uv = new Vector2(64f / 256, 240f / 256);
        }
        else
        {
            uv = new Vector2(160f / 256, 224f / 256);
        }

        for (int i = 0; i < 4; i++)
        {
            uvs.Add(uv);
        }
    }
}
