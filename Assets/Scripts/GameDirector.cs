using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime
{
    public int minutes { get; set; }
    public float seconds { get; set; }

    public void AddTime(float seconds)
    {
        this.seconds += seconds;
        if(this.seconds >= 60)
        {
            this.seconds -= 60;
            this.minutes++;
        }
    }

    public string GetTimeString()
    {
        string t = string.Empty;
        t += minutes.ToString("00");
        t += " : ";
        t += Mathf.FloorToInt(seconds).ToString("00");
        return t;
    }
}

public class GameDirector : MonoBehaviour
{
    public static GameDirector instance;

    public GameObject playerPrefab;
    public ParticleSystem playerExplosion;
    private GameObject player;

    public EnemyController enemyPrefab;
    private List<EnemyController> spawnedEnemies;
    public float enemyTokenLimit;
    public float enemyTokenIncreaseSec;
    public float enemyTokensUsed;

    public GameTime gameTime;
    public bool paused;

    private void Awake()
    {
        if(instance == null) instance = this;
        if(instance != this)
        {
            Debug.LogError("GameDirector Duplicate found!");
            Destroy(this);
        }

        if(playerPrefab != null)
        {
            player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("GameDirector doesn't have a player prefab to spawn");
        }
        gameTime = new GameTime();
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnedEnemies = new List<EnemyController>();
        StartCoroutine(EnemySpawner());
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused)
        {
            gameTime.AddTime(Time.deltaTime);
            enemyTokenLimit += enemyTokenIncreaseSec * Time.deltaTime;
        }
    }

    Coroutine spawner;
    IEnumerator EnemySpawner()
    {
        while(true)
        {
            if (enemyTokensUsed < enemyTokenLimit && !paused)
            {
                if (enemyPrefab.tokensRequired <= enemyTokenLimit - enemyTokensUsed)
                {
                    Debug.Log("Spawn");
                    Vector3 randomPos = Random.insideUnitCircle.normalized * 30f;
                    EnemyController newEnemy = LeanPool.Spawn(enemyPrefab, randomPos, Quaternion.identity);
                    enemyTokensUsed += newEnemy.tokensRequired;
                    newEnemy.OnEnemyDeathEvent += () => { 
                        enemyTokensUsed -= newEnemy.tokensRequired; 
                        if(spawner == null && !paused) spawner = StartCoroutine(EnemySpawner());
                    };
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void GameOver()
    {
        paused = true;
        StopAllCoroutines();
        Instantiate(playerExplosion, player.transform.position, Quaternion.identity);
        Destroy(player);
    }
}
