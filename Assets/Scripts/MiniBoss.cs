using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : Enemy
{
    public enum BossType { Normal, Fire, Ice, Thunder };
    public BossType type;
    public GameObject Face;
    public GameObject Mode;
    public GameObject Effect;
    public GameObject Dmg;
    public GameObject basicObj;
    GameObject monFace;
    GameObject monMode;
    GameObject EffectObj;
    GameObject damagedObj;

    Rigidbody2D rigid;
    SpriteRenderer ModeSprite, sprite, damagedSprite;
    Color[] DamagedCol;
    bool IsDead = false, IsDamaged = false, IsThink = true;
    public int attackKind = 0;
    public float speed = 1;
    int randDir, randTime;
    void Think()
    {
        randDir = Random.Range(-1, 2);
        randTime = Random.Range(3, 5);
        Invoke("Think", randTime);
    }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Start()
    {
        monFace.transform.position = monMode.transform.position
            = EffectObj.transform.position = transform.position;
        damagedObj.transform.position = transform.position + new Vector3(0, 0, -1);
        Invoke("Think", 2.0f);
    }


    protected override void Init()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        rigid.gravityScale = 2f;
        monFace = Instantiate(Face);
        monMode = Instantiate(Mode);
        EffectObj = Instantiate(Effect);
        damagedObj = Instantiate(Dmg);

        monFace.transform.parent = transform;
        monMode.transform.parent = transform;
        EffectObj.transform.parent = transform;
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
            case BossType.Normal:
                sprite.color = new Color(0, 0, 0, 1);
                ModeSprite.color = new Color(0, 0, 0, 0);
                break;
            case BossType.Fire:
                sprite.color = new Color(1, 0, 0, 1);
                ModeSprite.color = new Color(1, 0.3f, 0, 1);
                break;
            case BossType.Ice:
                sprite.color = new Color(0, 0.6f, 1f, 1);
                ModeSprite.color = new Color(0, 0.4f, 1f, 1);
                break;
            case BossType.Thunder:
                sprite.color = new Color(1, 1, 0, 1);
                ModeSprite.color = new Color(0.9f, 0.9f, 0, 1f);
                monFace.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.9f);
                break;
            default:
                Debug.Log("This monster has no type!!");
                break;
        }
        monFace.SetActive(true);
        monMode.SetActive(false);
        damagedObj.SetActive(true);
        EffectObj.SetActive(false);
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
        Health -= dmg;
        ScoreAdd(7);
        IsDamaged = true;
        CancelInvoke();
        rigid.gravityScale = 0.5f;
        if (Health <= 0)
            Invoke("Dead", 1.0f);
        gameObject.layer = LayerMask.NameToLayer("EnemyDamaged");

        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 3 + randDir * Vector2.left, ForceMode2D.Impulse);
        if (randDir > 0)
            rigid.AddTorque(-50);
        else
            rigid.AddTorque(50);
        StartCoroutine(Damaged());
    }
    public void Dead()
    {
        IsDead = true;
        ScoreAdd(30);
        Debug.Log("Boss Dead!!");
        Destroy(monFace);
        Destroy(monMode);
        Destroy(ModeSprite);
        Destroy(EffectObj);
        Destroy(gameObject,1);
    }
    void ModeUpdate()
    {

    }
    void Move()
    {

    }
    void BossCheck()
    {

    }
    // Update is called once per frame
    void Update()
    {
        BossCheck();
        if (!IsDead && !IsDamaged)
        {
            ModeUpdate();
            Move();
        }
    }

}
