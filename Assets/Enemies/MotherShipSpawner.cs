using System.Collections;
using UnityEngine;

/// <summary>
/// A cada 30-50 segundos (aleatório), instancia a nave chefe no canto
/// superior esquerdo do ambiente.
/// </summary>
public class MotherShipSpawner : MonoBehaviour
{
    [SerializeField] private GameObject motherShipPrefab;
    [SerializeField] private Vector2 spawnPosition = new Vector2(-9f, 4.5f);
    [SerializeField] private float minWait = 30f;
    [SerializeField] private float maxWait = 50f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(wait);

            if (motherShipPrefab != null)
            {
                Instantiate(motherShipPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
