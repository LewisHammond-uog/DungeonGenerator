using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DLAParticle : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 direction;

    [SerializeField]
    private GameObject spawnTile;
    
    private void Start()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        direction = new Vector3(x, 0, y);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector2Int pos = new Vector2Int((int) other.transform.position.x, (int) other.transform.position.z);
        FindObjectOfType<Generator>().TileMap.SpawnTile(pos, spawnTile);
        Destroy(gameObject);
    }
}
