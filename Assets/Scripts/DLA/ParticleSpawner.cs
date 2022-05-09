using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class ParticleSpawner : MonoBehaviour
{
     [SerializeField] private GameObject particlePrefab;

     private TileMap3D tilemap;

     private List<DLAParticle> particles;

     public void SpawnParticles(int particlesToSpawn)
     {
          particles = new List<DLAParticle>();
          tilemap = FindObjectOfType<Generator>().TileMap;
          for (int i = 0; i < particlesToSpawn; i++)
          {
               SpawnParticleAtRandomPos();
          }
     }

     public void ResetSpawner()
     {
          //Destroy all particles
          if (particles != null)
          {
               foreach (DLAParticle particle in particles)
               {
                    Destroy(particle.gameObject);
               }
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
          
          
          GameObject particleGO = Instantiate(particlePrefab, new Vector3(randPos.x, 0, randPos.y), Quaternion.identity);

          if (particleGO.TryGetComponent(out DLAParticle particle))
          {
               particles.Add(particle);
               particle.spawner = this;
          }
     }

     public IEnumerator WaitForAllParticlesDead()
     {
          while (particles.Count > 0 || particles.Any(x => x != null))
          {
               yield return null;
          }
          particles.Clear();
     }

     public void RemoveParticleFromList(DLAParticle particle)
     {
          particles.Remove(particle);
     }
}
