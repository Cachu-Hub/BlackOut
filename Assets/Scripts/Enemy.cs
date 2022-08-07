using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected int Health;
    public GameObject player;
    Monster mon;
    MiniBoss miniBoss;
    Player playerScript;
    public void DoInit()
    {
        Init();
    }
    protected virtual void Init()
    {
        Debug.Log("Enemy Init!");
    }
    protected virtual void ScoreAdd(int n)
    {
        playerScript.ScoreAdd(n);
    }
    protected virtual void Awake()
    {
        playerScript = player.GetComponent<Player>();
        mon = GetComponent<Monster>();
        miniBoss = GetComponent<MiniBoss>();
        if (mon)
        {
            switch (mon.type)
            {
                case Monster.MonsterType.Normal:
                    Health = 5;
                    break;
                case Monster.MonsterType.Fire:
                    Health = 10;
                    break;
                case Monster.MonsterType.Ice:
                    Health = 10;
                    break;
                case Monster.MonsterType.Thunder:
                    Health = 10;
                    break;
                case Monster.MonsterType.dummy:
                    Health = 5;
                    break;
                default:
                    break;
            }
        }
        else if (miniBoss)
        {
            Health = 30;
            switch (miniBoss.type)
            {
                case MiniBoss.BossType.Normal:
                    break;
                case MiniBoss.BossType.Fire:
                    break;
                case MiniBoss.BossType.Ice:
                    break;
                case MiniBoss.BossType.Thunder:
                    break;
                default:
                    break;
            }
        }
    }

    void PlayerScoreAdd(int n)
    {
        playerScript.Score += n;
    }

    public virtual void OnDamaged(int dmg, Vector3 dir)
    {
        Debug.Log("This mon doesnt have Enemy script!");
    }
    // Update is called once per frame
    
}
