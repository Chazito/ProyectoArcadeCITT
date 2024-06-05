using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

public class ActivePerk
{
    public PerkSO perk;
    public int stack;
    public ActivePerk(PerkSO perk, int stack)
    {
        this.perk = perk;
        this.stack = stack;
    }

    public void ApplyPerk(PlayerController player)
    {
        if(stack > 1)
        {
            perk.ApplyPerk(player, stack);
        }
        else
        {
            perk.ApplyPerk(player);
        }
    }
    public void RemovePerk(PlayerController player)
    {
        perk.RemovePerk(player);
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

    public Dictionary<string, ActivePerk> activePerks;
    public List<PerkSO> perkLibrary;
    public List<string> banishedPerks;
    public List<string> bannedPerks;
    [SerializeField] private LevelUpUIController levelUpUIController;


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
        activePerks = new Dictionary<string, ActivePerk>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused)
        {
            gameTime.AddTime(Time.deltaTime);
            enemyTokenLimit += enemyTokenIncreaseSec * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
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
                    Vector3 randomPos = UnityEngine.Random.insideUnitCircle.normalized * 30f;
                    EnemyController newEnemy = LeanPool.Spawn(enemyPrefab, randomPos, Quaternion.identity);
                    enemyTokensUsed += newEnemy.tokensRequired;
                    newEnemy.OnEnemyDeathEvent += () => { 
                        spawnedEnemies.Remove(newEnemy);
                        enemyTokensUsed -= newEnemy.tokensRequired; 
                        if(spawner == null && !paused) spawner = StartCoroutine(EnemySpawner());
                    };
                    spawnedEnemies.Add(newEnemy);
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

    public delegate void OnMenuPause();
    public OnMenuPause onMenuPause;
    public void MenuPause()
    {
        paused = true;
        if (onMenuPause != null) onMenuPause.Invoke();
    }

    public void PlayerLevelUp()
    {
        MenuPause();
        levelUpUIController.gameObject.SetActive(true);
        List<PerkSO> availablePerks = GetAvailablePerks();
        List<PerkSO> selectedOffers = new List<PerkSO>();

        for(int i = 0; i < 3; i++)
        {
            int r = UnityEngine.Random.Range(0, availablePerks.Count);
            selectedOffers.Add(availablePerks[r]);
            //TODO Add perks to compensate for missing perks

            //Debug.Log(availablePerks[r].perkName);
            availablePerks.RemoveAt(r);
        }
        levelUpUIController.Setup(selectedOffers);
    }

    public void ResumeGame()
    {
        levelUpUIController.gameObject.SetActive(false);
        paused = false;
    }

    private List<PerkSO> GetAvailablePerks()
    {
        List<PerkSO> availablePerks = new List<PerkSO>();
        foreach (PerkSO perk in perkLibrary)
        {
            if (banishedPerks.Exists(x => perk.perkID == x) || bannedPerks.Exists(x => perk.perkID == x))
            {
                continue;
            }
            if (perk.requirements.Count > 0)
            {
                bool containsAllRequirements = true;
                foreach (string requirement in perk.requirements)
                {
                    if (!activePerks.ContainsKey(requirement))
                    {
                        containsAllRequirements = false;
                        break;
                    }
                }
                if (containsAllRequirements)
                {
                    if (activePerks.ContainsKey(perk.perkID))
                    {
                        if (perk.canStack)
                        {
                            availablePerks.Add(perk);
                            continue;
                        }
                    }
                    else
                    {
                        availablePerks.Add(perk);
                    }
                }
                else
                {
                    continue;
                }
            }
            else
            {
                if (activePerks.ContainsKey(perk.perkID))
                {
                    if (perk.canStack)
                    {
                        availablePerks.Add(perk);
                    }
                    continue;
                }
                else
                {
                    availablePerks.Add(perk);
                }
            }
        }
        return availablePerks;
    }

    public void AddPerk(PerkSO perk)
    {
        if (!activePerks.ContainsKey(perk.perkID))
        {
            if(perk is WeaponPerk)
            {
                RemovePairsByCondition(activePerks, p => p.perk is WeaponPerk);
                activePerks.Add(perk.perkID, new ActivePerk(perk, 1));
            }
            else
            {
                activePerks.Add(perk.perkID, new ActivePerk(perk, 1));
            }
            activePerks[perk.perkID].ApplyPerk(player.GetComponent<PlayerController>());
        }
        else
        {
            activePerks[perk.perkID].stack++;
            activePerks[perk.perkID].ApplyPerk(player.GetComponent<PlayerController>());
        }
    }

    private static void RemovePairsByCondition<TKey, TValue>(Dictionary<TKey, TValue> dictionary, Func<TValue, bool> condition)
    {
        List<TKey> keysToRemove = new List<TKey>();

        foreach (var kvp in dictionary)
        {
            if (condition(kvp.Value))
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            dictionary.Remove(key);
        }
    }
}
