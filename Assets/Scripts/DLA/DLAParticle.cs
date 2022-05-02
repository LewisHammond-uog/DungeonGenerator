using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DLAParticle : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 direction;

    private void Start()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        direction = new Vector3(x, 0, y);
        direction.Normalize();
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DLAParticle _))
        {
            return;
        }

        Vector3 objectPos = other.transform.parent.parent.position;
        Vector2 pos = new Vector2(objectPos.x, objectPos.z);
        FindObjectOfType<Generator>().TileMap.DeleteTile(Vector2Int.RoundToInt(pos));
        Destroy(gameObject);
    }
}
