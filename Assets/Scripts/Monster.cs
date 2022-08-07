using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster: Enemy
{
    public enum MonsterType { Normal, Fire, Ice, Thunder, dummy };
    public MonsterType type;
    public GameObject Face;
    public GameObject Mode;
    public GameObject Effect;
    public GameObject Dmg;
    public GameObject basicObj;
    public AudioClip[] PlayerClip;
    int attackKind = 0;
    float speed = 1;

    int randDir, randTime, CheckPlayer;
    bool IsMode = false, CanMode = false, IsDead = false, IsDamaged = false;
    bool Landing = false, LeftLanding = false, RightLanding = false;
    bool LeftWall = false, RightWall = false, FindPlayer = false;
    bool InPlatform = false, SpecialMode = false, SuperMode = false, IsThink = true;

    Vector2 playerPos, startPos;
    Quaternion startRot;
    GameObject monFace;
    GameObject monMode;
    GameObject EffectObj;
    GameObject damagedObj;
    AudioSource MonAudio;

    Rigidbody2D rigid;
    SpriteRenderer ModeSprite, sprite, damagedSprite;
    RaycastHit2D[] LandRay, LeftRay, RightRay, leftLandRay, rightLandRay;
    Color[] DamagedCol;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        startRot = this.transform.rotation;
        startPos = this.transform.position;
        Init();
    }
    void PlaySound(int index)
    {
        MonAudio.clip = PlayerClip[index];
        MonAudio.Play();
        Debug.Log("Monster Sound!!");
    }
    void ReSpawn()
    {
        this.transform.rotation = startRot;
        this.transform.position = startPos;
        base.Awake();
        Init();
        gameObject.SetActive(true);
    }
    protected override void Init()
    {
        MonAudio = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        rigid.gravityScale = 0.5f;
        CheckPlayer = 0;
        monFace = Instantiate(Face);
        monMode = Instantiate(Mode);
        damagedObj = Instantiate(Dmg);

        monFace.transform.parent = transform;
        monMode.transform.parent = transform;
        damagedObj.transform.parent = transform;

        rigid.gravityScale = 0.5f;
        ModeSprite = monMode.GetComponent<SpriteRenderer>();
        damagedSprite = damagedObj.GetComponent<SpriteRenderer>();

        damagedSprite.color = new Color(1, 1, 1, 0);
        DamagedCol = new Color[2];
        DamagedCol[0] = new Color(1, 1, 1, 0.2f);
        DamagedCol[1] = new Color(1, 1, 1, 0.4f);
        switch (type)
        {
            case MonsterType.Normal:
                sprite.color = new Color(0, 0, 0, 1);
                ModeSprite.color = new Color(0, 0, 0, 0);
                EffectObj = Instantiate(Effect.transform.Find("NormalEffect").gameObject);
                break;
            case MonsterType.Fire:
                sprite.color = new Color(1, 0, 0, 1);
                ModeSprite.color = new Color(1, 0.3f, 0, 1);
                EffectObj = Instantiate(Effect.transform.Find("FireEffect").gameObject);
                break;
            case MonsterType.Ice:
                sprite.color = new Color(0, 0.6f, 1f, 1);
                ModeSprite.color = new Color(0, 0.4f, 1f, 1);
                EffectObj = Instantiate(Effect.transform.Find("IceEffect").gameObject);
                break;
            case MonsterType.Thunder:
                sprite.color = new Color(1, 1, 0, 1);
                ModeSprite.color = new Color(0.9f, 0.9f, 0, 1f);
                EffectObj = Instantiate(Effect.transform.Find("ThunderEffect").gameObject);
                monFace.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.9f);
                break;
            case MonsterType.dummy:
                sprite.color = new Color(0, 0, 0, 1);
                ModeSprite.color = new Color(0, 0, 0, 0);
                EffectObj = Instantiate(Effect.transform.Find("NormalEffect").gameObject);
                break;
            default:
                Debug.Log("This monster has no type!!");
                break;
        }
        EffectObj.transform.parent = transform;
        monFace.SetActive(true);
        monMode.SetActive(false);
        damagedObj.SetActive(true);
        EffectObj.SetActive(false);
        IsDead = false;
        IsDamaged = false;
        monFace.transform.position = monMode.transform.position
            = EffectObj.transform.position = transform.position;
        damagedObj.transform.position = transform.position + new Vector3(0, 0, -1);
        Invoke("Think", 2.0f);
    }

    IEnumerator AfterDash() //잔상 로브젝트 생성
    {
        GameObject after = Instantiate(basicObj);
        after.GetComponent<SpriteRenderer>().color = sprite.color;
        after.transform.SetParent(this.transform.parent, false);
        after.SetActive(true);
        after.transform.position = transform.position;
        after.transform.rotation = transform.rotation;
        SpriteRenderer afterSprite = after.GetComponent<SpriteRenderer>();
        switch (type)
        {
            case MonsterType.Normal:
                for (int i = 0; i < 5; i++)
                {
                    afterSprite.color = new Color(0, 0, 0, 0.8f - i * 0.2f);
                    yield return new WaitForSeconds(0.1f);
                }
                break;
            case MonsterType.Fire:
                for (int i = 0; i < 5; i++)
                {
                    afterSprite.color = new Color(1, 0, 0, 0.8f - i * 0.2f);
                    yield return new WaitForSeconds(0.1f);
                }
                break;
            case MonsterType.Ice:
                for (int i = 0; i < 5; i++)
                {
                    afterSprite.color = new Color(0, 0.6f, 1f, 0.8f - i * 0.2f);
                    yield return new WaitForSeconds(0.1f);
                }
                break;
            case MonsterType.Thunder:
                for (int i = 0; i < 5; i++)
                {
                    afterSprite.color = new Color(1, 1, 0, 0.8f - i * 0.2f);
                    yield return new WaitForSeconds(0.1f);
                }
                break;
            default:
                break;
        }
        Destroy(after);
        yield return null;
    }
    IEnumerator DashEnd() //대쉬
    {
        gameObject.layer = LayerMask.NameToLayer("EnemyMode");
        PlaySound(3);
        for (int i = 0; i < 12; i++)
        {
            yield return new WaitForSeconds(0.05f);
            if (IsDamaged)
                break;
            StartCoroutine(AfterDash());
        }
        SpecialMode = false;
        yield return null;
    }
    IEnumerator Attack() // update 2022.03.02
    {
        yield return new WaitForSeconds(0.5f);
        monMode.SetActive(true);
        monFace.SetActive(false);
        if (FindPlayer)
        {
            attackKind = Random.Range(1, 4);
            switch (attackKind)
            {
                case 1:
                    yield return new WaitForSeconds(0.5f);
                    SpecialMode = true;
                    if (!IsDamaged)
                    {
                        rigid.AddForce(Vector2.right * (sprite.flipX ? -15 : 15) + Vector2.up * 2,
                            ForceMode2D.Impulse);
                        StartCoroutine(DashEnd());
                        yield return new WaitForSeconds(0.6f);
                    }
                    break;
                default:
                    EffectObj.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                    SpecialMode = true;
                    if (!IsDamaged)
                        StartCoroutine(SpecialMove());
                    break;
            }
            while (InPlatform || SpecialMode)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        ModeOff();
        Invoke("Think", 1.0f);
        yield return null;
    }
    void ModeOff() //모드 해제
    {
        if (this)
        {
            rigid.velocity /= 10;
            monMode.SetActive(false);
            monFace.SetActive(true);
            EffectObj.SetActive(false);
            rigid.gravityScale = 0.5f;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            IsMode = false;
            IsThink = false;
            SuperMode = false;
            SpecialMode = false;
        }
    }

    IEnumerator SpecialMove()// 필살기
    {
        gameObject.layer = LayerMask.NameToLayer("EnemyMode");
        switch (type)
        {
            case MonsterType.Normal:
                rigid.AddForce(Vector2.right * (sprite.flipX ? -15 : 15) + Vector2.up * 2,
                    ForceMode2D.Impulse);
                StartCoroutine(DashEnd());
                yield return new WaitForSeconds(0.6f);
                break;
            case MonsterType.Fire:
                PlaySound(0);
                rigid.AddForce(Vector2.right * (sprite.flipX ? -10 : 10) + Vector2.up,
                     ForceMode2D.Impulse);
                break;
            case MonsterType.Ice:
                PlaySound(1);
                rigid.AddForce(Vector2.right * (sprite.flipX ? -8 : 8)
                    + Vector2.up * 8, ForceMode2D.Impulse);
                break;
            case MonsterType.Thunder:
                rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
                break;
            default:
                break;
        }
        switch (type)
        {
            case MonsterType.Fire:
                if (playerPos.magnitude <= 8f && !IsDamaged)
                {
                    SuperMode = true;
                    rigid.gravityScale = 0f;
                    rigid.velocity /= 5;
                    rigid.AddForce(playerPos.normalized * 10, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.5f);
                }
                break;
            case MonsterType.Ice:
                yield return new WaitForSeconds(1.5f);
                SuperMode = true;
                rigid.gravityScale = 0f;
                rigid.velocity = Vector2.zero;
                yield return new WaitForSeconds(0.1f);
                if (IsDamaged)
                    break;
                rigid.gravityScale = 5f;
                rigid.AddForce(Vector2.down * 20, ForceMode2D.Impulse);
                while (!Landing)
                    yield return new WaitForSeconds(0.1f);
                yield return new WaitForSeconds(0.2f);
                break;
            case MonsterType.Thunder:
                yield return new WaitForSeconds(0.6f);
                if (IsDamaged)
                    break;
                SuperMode = true;
                PlaySound(2);
                rigid.gravityScale = 0.2f;
                rigid.velocity = Vector2.zero;
                rigid.AddForce(new Vector2(sprite.flipX ? -30 : 30, 0),
                    ForceMode2D.Impulse);
                yield return new WaitForSeconds(0.5f);
                break;
            default:
                break;
        }
        SpecialMode = false;
    }


    void Think()
    {
        IsThink = true;
        randDir = Random.Range(-1, 2);
        randTime = Random.Range(3, 5);
        Invoke("Think", randTime);
    }
    void Move()
    {
        if (!IsDamaged && !IsMode && Landing)
        {
            if (randDir == -1 && (!LeftLanding || LeftWall))
                randDir = 1;
            else if (randDir == 1 && (!RightLanding || RightWall))
                randDir = -1;

            if (!FindPlayer || type == MonsterType.dummy)
                transform.Translate(Vector2.right * randDir * Time.deltaTime * speed);
        }

        if (randDir != 0)
            sprite.flipX = monFace.GetComponent<SpriteRenderer>().flipX = randDir < 0;

        if (Landing)
            rigid.angularVelocity = transform.rotation.z * (-500);


        if (SuperMode)
            rigid.angularVelocity = 2000 * (sprite.flipX ? 1 : -1);
        else if (SpecialMode)
            rigid.angularVelocity = 1000 * (sprite.flipX ? 1 : -1);
    }
    IEnumerator Damaged()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < DamagedCol.Length; j++)
            {
                damagedSprite.color = DamagedCol[j];
                yield return new WaitForSeconds(0.5f);
            }
        }
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        damagedSprite.color = new Color(1, 1, 1, 0);
        IsDamaged = false;
        if (!IsThink) Invoke("Think", 0.5f);
        yield return null;
    }
    public override void OnDamaged(int dmg, Vector3 dir)
    {
        IsDamaged = true;
        StopCoroutine(SpecialMove());
        StopCoroutine(DashEnd());
        StopCoroutine(Attack());
        Health -= dmg;
        ScoreAdd(5);
        PlaySound(4);
        ModeOff();
        CancelInvoke();
        rigid.gravityScale = 0.5f;
        StartCoroutine(Damaged());
        if (Health <= 0)
        {
            Debug.Log("Monster Dead!!");
            Invoke("Dead", 1.0f);
        }
        gameObject.layer = LayerMask.NameToLayer("EnemyDamaged");

        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 3 + randDir * Vector2.left, ForceMode2D.Impulse);
        if (randDir > 0)
            rigid.AddTorque(-50);
        else
            rigid.AddTorque(50);
    }
    public void Dead()
    {
        switch (type)
        {
            case MonsterType.Normal:
                ScoreAdd(10);
                break;
            case MonsterType.Fire:
                ScoreAdd(15);
                break;
            case MonsterType.Ice:
                ScoreAdd(15);
                break;
            case MonsterType.Thunder:
                ScoreAdd(15);
                break;
            case MonsterType.dummy:
                ScoreAdd(15);
                break;
            default:
                break;
        }
        IsDead = true;
        ModeOff();
        Destroy(monFace);
        Destroy(monMode);
        Destroy(damagedObj);
        Destroy(EffectObj);
        gameObject.SetActive(false);
        Invoke("ReSpawn",3);
    }

    void Look()
    {
        if (playerPos.x < 0)
            randDir = -1;
        else
            randDir = 1;
    }
    void ModeUpdate()
    {
        if (CanMode && !IsDamaged)
        {
            IsMode = true;
            CanMode = false;
            CancelInvoke();
            Look();
            StartCoroutine(Attack());
        }
    }

    bool RayCheck(RaycastHit2D[] ray, string FindTag)
    {
        foreach (var item in ray)
        {
            if (item.transform.tag == FindTag)
            {
                return true;
            }
        }
        return false;
    }

    void Check()
    {
        LandRay = Physics2D.RaycastAll(transform.position, Vector2.down, 0.8f);
        LeftRay = Physics2D.RaycastAll(transform.position, Vector2.left, 0.7f);
        RightRay = Physics2D.RaycastAll(transform.position, Vector2.right, 0.7f);
        leftLandRay = Physics2D.RaycastAll(transform.position + Vector3.left * 0.5f,
            Vector2.down, 0.5f);
        rightLandRay = Physics2D.RaycastAll(transform.position + Vector3.right * 0.5f,
            Vector2.down, 0.5f);

        LeftWall = RayCheck(LeftRay, "Platform");
        RightWall = RayCheck(RightRay, "Platform");
        Landing = RayCheck(LandRay, "Platform");
        LeftLanding = RayCheck(leftLandRay, "Platform");
        RightLanding = RayCheck(rightLandRay, "Platform");
        playerPos = player.transform.position - transform.position;


        if (sprite.flipX)
            FindPlayer = (playerPos.x > -6 && playerPos.x < 2) 
                && (playerPos.y < 2 && playerPos.y >=0 );
        else 
            FindPlayer = (playerPos.x < 6 || playerPos.x > -2)
                && (playerPos.y < 2 && playerPos.y >= 0);

        if (FindPlayer)
            CheckPlayer++;
        else
            CheckPlayer = 0;

        if (CheckPlayer > 20 && !IsMode && Landing && type != MonsterType.dummy)
        {
            CanMode = true;
        }
    }

    void Update()
    {
        Check();
        if (!IsDead && !IsDamaged)
        {
            ModeUpdate();
            Move();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("AirPlatform"))
            InPlatform = true;
        else
            InPlatform = false;

        if (collision.gameObject.tag == "Player")
        {
            if (IsMode)
            {
                StopCoroutine(Attack());
                StopCoroutine(SpecialMove());
                StopCoroutine(DashEnd());
                ModeOff();
                if (!IsThink) Invoke("Think", 1f);
            }
        }
    }
}
