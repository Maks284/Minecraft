using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new();
    public ChunkRenderer ChunkPrefab;

    private Camera mainCamera;

     private void Start()
     {
        mainCamera= Camera.main;

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                float xPos = x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                float zPos = z * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;

                var chunkData = new ChunkData
                {
                    ChunkPosition = new Vector2Int(x, z),
                    Blocks = TerrainGenerator.GenerateTerrain(xPos, zPos)
                };

                ChunkDatas.Add(new Vector2Int(x, z), chunkData);

                var chunk = Instantiate(ChunkPrefab, new Vector3(xPos, 0 , zPos), Quaternion.identity, transform);

                chunk.ChunkData= chunkData;
                chunk.ParenWorld = this;

                chunkData.Renderer = chunk;
            }
        }
     }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            bool isDestroying = Input.GetMouseButtonDown(0);

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(.5f, .5f));

            if (Physics.Raycast(ray, out var hitInfo))
            {
                Vector3 blockCenter;

                if (isDestroying)
                {
                    blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.BlockScale / 2;
                }
                else
                {
                    blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2;
                }

                Vector3Int blockWorldPosition = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale);
                Vector2Int chunkPosition = GetChunkConteinigBlock(blockWorldPosition);

                if (ChunkDatas.TryGetValue(chunkPosition, out ChunkData chunkData))
                {
                    Vector3Int firstChunkBlock = new Vector3Int(chunkPosition.x, 0, chunkPosition.y) * ChunkRenderer.ChunkWidth;
                    if (isDestroying)
                    {
                        chunkData.Renderer.DestroyBlock(blockWorldPosition - firstChunkBlock);
                    }
                    else
                    {
                        chunkData.Renderer.SpawnBlock(blockWorldPosition - firstChunkBlock);
                    }
                }
            }
        }  
    }

    public Vector2Int GetChunkConteinigBlock(Vector3Int blockWorldPosition)
    {
        return new Vector2Int(blockWorldPosition.x / ChunkRenderer.ChunkWidth, blockWorldPosition.z / ChunkRenderer.ChunkWidth); 
    }
}
