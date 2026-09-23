using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1.5f;
    public float[] laneXPositions = { -2.2f, 0f, 2.2f };
    public float spawnY = 9f;

    private float timer;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying()) return;

        timer += Time.deltaTime;
        float dynamicInterval = Mathf.Max(0.5f, spawnInterval - GameManager.Instance.GetScore() * 0.005f);

        if (timer >= dynamicInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        int lane = Random.Range(0, laneXPositions.Length);
        Vector3 pos = new Vector3(laneXPositions[lane], spawnY, 0f);
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        EnemyCar ec = enemy.GetComponent<EnemyCar>();
        if (ec != null)
        {
            ec.lane = lane;
            ec.speed = Random.Range(3f, 6f) + GameManager.Instance.GetScore() * 0.01f;
        }
    }
}
