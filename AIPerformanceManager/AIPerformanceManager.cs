using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;
using MoreMountains.TopDownEngine;

public class AIPerformanceManager : Singleton<AIPerformanceManager>
{

    public GameObject[] enemies;
    public List<Health> enemiesHealths;
    public GameObject[] players;

    // The range to activate the obj
    public float rangeToActivateEnemy = 30f;

    
    void Start()
    {
        // Find all types of enemies
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //enemiesHealth = new Health[enemies.Length];
        
        // Only add the ones with health component
        for (int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i].GetComponent<Health>() != null)
                AddEnemy(enemies[i].GetComponent<Health>());
        }

        // Cache all players
        players = GameObject.FindGameObjectsWithTag("Player");

    }

    void Update()
    {
        // Check for every player the distance to the enemies
        foreach (var player in players)
        {
            if (player.activeSelf)
            {
                CheckEnemiesDistance(player);
            }
        }
    }

    public void CheckEnemiesDistance(GameObject obj)
    {
        foreach (var eh in enemiesHealths.ToList())
        {
            // null check
            if(eh == null)
                continue;
            
            // remove the health if dead
            if (eh.CurrentHealth <= 0)
            {
                RemoveEnemy(eh);
                continue;
            }
            
            // check the distance
            float distance = Vector3.Distance(eh.transform.position, obj.transform.position);

            if (distance > rangeToActivateEnemy)
                eh.gameObject.SetActive(false);
            else
                eh.gameObject.SetActive(true);
        }
    }

    public void AddEnemy(Health enemyHealth)
    {
        enemiesHealths.Add(enemyHealth);
    }

    public void RemoveEnemy(Health enemyHealth)
    {
        enemiesHealths.Remove(enemyHealth);
    }
    
}
