using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class ParticleSpawner : MonoBehaviour
{
     [SerializeField] private GameObject particlePrefab;
     [Min(0)] [SerializeField] private int particlesToSpawn;

     private TileMap3D tilemap;
     
     private void Awake()
     {

     }

     private void Start()
     {
          tilemap = FindObjectOfType<Generator>().TileMap;
          for (int i = 0; i < particlesToSpawn; i++)
          {
               SpawnParticleAtRandomPos();
          }
     }

     private void SpawnParticleAtRandomPos()
     {
          //Generate random position that is not a tile already in the tile map
          Vector2Int randPos = new Vector2Int();
          do
          {
               int x = Random.Range(0, tilemap.Size.x);
               int y = Random.Range(0, tilemap.Size.y);
               randPos.Set(x, y);

          } while (tilemap.GetTile(randPos) != null);

          Debug.Log(randPos);
          
          GameObject partcile = Instantiate(particlePrefab, new Vector3(randPos.x, 0, randPos.y), Quaternion.identity);
     }
}
