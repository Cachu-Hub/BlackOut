using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Mode Var
    public enum ModeType { Normal, Fire, Ice, Thunder, Black};
    public ModeType mode = ModeType.Normal;

    //public Var
    public int Health;
    public int Score = 0;
    public float Speed;
    public float JumpPower;
    public float EatPower;
    public float EatRange;
    public float RotationPower;
    public GameObject modeDeco;
    public GameObject eatObj;
    public GameObject attackBall;
    public GameObject basicObj;
    public GameObject eatEffect;
    public GameObject jumpEffect;
    public AudioClip[] PlayerClip;

    public GameManager gameManager;

    //Input Var
    float xInput, stageInput;
    bool yInput, Zstay, Xdown, IsEat, IsDash, CanDash, IsAttack;

    //Check Var
    int Jumpping = 0;
    public bool DontJump = false;
    int Ztime = 0, Zswitch = 15;
    bool Landing = false, LeftWall = false, RightWall = false;
    bool IsHorizon = true;
    public bool IsDead = false;
    int monsterIndex = -1;

    Animator anim;
    AudioSource PlayerAudio;
    Rigidbody2D rigid, eatMon;
    SpriteRenderer sprite, decoSprite, dmgSprite;
    eatArea EatArea;
    RaycastHit2D[] LandRay, EatRay, LeftRay, RightRay;
    Color BlackCol;
    Color[] DamagedCol;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        decoSprite = modeDeco.GetComponent<SpriteRenderer>();
        dmgSprite = basicObj.GetComponent<SpriteRenderer>();
        PlayerAudio = GetComponent<AudioSource>();
        Init();
        ModeUpdate();
    }
    void PlaySound(int index)
    {
        PlayerAudio.clip = PlayerClip[index];
        PlayerAudio.Play();
        Debug.Log("Player Sound!!");
    }

    public void Init()
    {
        eatEffect.SetActive(false);
        basicObj.SetActive(true);
        BlackCol = new Color(1, 1, 1, 0.6f);
        if(dmgSprite)  
            dmgSprite.color = new Color(1, 1, 1, 0);
        DamagedCol = new Color[2];
        DamagedCol[0] = new Color(1, 1, 1, 0.2f);
        DamagedCol[1] = new Color(1, 1, 1, 0.3f);
        EatArea = eatObj.GetComponent<eatArea>();
    }
    void Command()
    {
        xInput = Input.GetAxis("Horizontal");
        stageInput = Input.GetAxis("Vertical");
        yInput = Input.GetKeyDown(KeyCode.Space);
        Zstay = Input.GetKey(KeyCode.Z);
        Xdown = Input.GetKeyDown(KeyCode.X);
    }

    void Jump()
    {
        if (!IsEat)
        {
            //Jump
            if (yInput && Jumpping < 2)
            {
                if (DontJump)
                {
                    DontJump = false;
                    return;
                }
                IsDash = false;
                PlaySound(0);
                GameObject jumpDummy = Instantiate(jumpEffect);
                jumpDummy.transform.position
                    = transform.position + new Vector3(0, -0.5f, 0);
                jumpDummy.SetActive(true);
                Destroy(jumpDummy, 0.3f);
                Jumpping++;
                rigid.velocity = Vector2.zero;
                rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
                rigid.AddTorque(-RotationPower);
            }
        }
    }
    void Move()
    {
        if (!IsEat && !IsDash)
        {
            //Left,Right Move
            if (xInput > 0 && !RightWall)
                transform.position += (Vector3.right * xInput * Speed * Time.deltaTime);
            if (xInput < 0 && !LeftWall)
                transform.position += (Vector3.right * xInput * Speed * Time.deltaTime);

            //Flip
            if (xInput != 0)
                sprite.flipX = decoSprite.flipX = xInput < 0;

            if (sprite.flipX)
            {
                eatObj.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 180);
                eatEffect.GetComponent<SpriteRenderer>().flipX = true;
                eatEffect.transform.position = transform.position + new Vector3(-1.5f, 0, 0);
            }
            else
            {
                eatObj.transform.eulerAngles = transform.eulerAngles;
                eatEffect.GetComponent<SpriteRenderer>().flipX = false;
                eatEffect.transform.position = transform.position + new Vector3(1.5f, 0, 0);
            }
            // Make Rotation 0
            if (Landing && Jumpping == 0)
            {
                rigid.angularVelocity = transform.rotation.z * (-700);
            }

        }
    }

    IEnumerator AfterDash()
    {
        GameObject after = Instantiate(basicObj);
        after.transform.SetParent(this.transform.parent, false);
        after.transform.position = transform.position;
        after.transform.rotation = transform.rotation;
        SpriteRenderer afterSprite = after.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            afterSprite.color = new Color(0, 0, 0, 0.8f - i * 0.2f);
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(after);
        yield return null;
    }
    IEnumerator DashEnd()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.04f);
            if (IsDash)
                StartCoroutine(AfterDash());
            else
                break;
        }
        if (IsDash)
            rigid.velocity = rigid.velocity / 4;
        IsDash = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return null;
    }
    void Dash()
    {
        if (Xdown && !IsDash && CanDash)
        {
            CanDash = false;
            IsDash = true;
            PlaySound(2);
            StartCoroutine(DashEnd());
        }
        if (IsDash)
        {
            rigid.angularVelocity = 1500;
            if (sprite.flipX)
                rigid.velocity = Vector2.left * 15;
            else
                rigid.velocity = Vector2.right * 15;
        }
    }
    void Eat()
    {
        if (!IsEat && (Zstay && Landing && IsHorizon && Ztime == Zswitch && Ztime >= 0))
        {
            IsEat = true;
            eatEffect.SetActive(true);
            anim.SetBool("IsSwallow", true);
        }
        if (IsEat && (!Zstay || !Landing || !IsHorizon) && Ztime >= 0)
        {
            Ztime = 0;
            IsEat = false;
            eatEffect.SetActive(false);
            anim.SetBool("IsSwallow", false);
        }
        if(IsEat)
        {
            if(monsterIndex >= 0)
            {
                eatMon = EatRay[monsterIndex].collider.gameObject.GetComponent<Rigidbody2D>();
                eatMon.AddForce((sprite.flipX ? Vector2.right:Vector2.left)*EatPower,
                    ForceMode2D.Force);
            }
        }
    }

    void ReadyAttack()
    {
        IsAttack = false;
    }
    void Attack()
    {
        if (!Zstay && Ztime > 0 && Ztime < Zswitch && !IsAttack)
        {
            IsAttack = true;
            PlaySound(1);
            Ztime = 0;
            GameObject dummyBall = Instantiate(attackBall);
            attackBall ballScript = dummyBall.GetComponent<attackBall>();
            ballScript.Init(transform.position, (int)mode, sprite.flipX);
            anim.SetTrigger("Attack");
            mode = ModeType.Normal;
            ModeUpdate();
            dummyBall.SetActive(true);
            Invoke("ReadyAttack", 1f);
        }
    }

    IEnumerator Damaged()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < DamagedCol.Length; j++)
            {
                dmgSprite.color = DamagedCol[j];
                yield return new WaitForSeconds(0.5f);
            }
        }
        gameObject.layer = LayerMask.NameToLayer("Player");
        dmgSprite.color = new Color(1, 1, 1, 0);
        yield return null;
    }
    void Dead()
    {
        IsDead = true;
        StopAllCoroutines();
        gameObject.SetActive(false);
        gameManager.PlayerDead();
    }
    void OnDamaged(Vector3 dir)
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlaySound(3);
            Health -= 1;
            gameManager.PlayerHealthUpdate();
            StartCoroutine(Damaged());
            if (Health == 0)
                Dead();
            rigid.velocity = Vector2.zero;
            if (dir.x > 0)
                rigid.AddForce(Vector2.up * 5 + Vector2.right * 2, ForceMode2D.Impulse);
            else
                rigid.AddForce(Vector2.up * 5 + Vector2.left * 2, ForceMode2D.Impulse);
            gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");
        }
    }

    void ModeUpdate()
    {
        modeDeco.SetActive(true);
        switch (mode)
        {
            case ModeType.Normal:
                modeDeco.SetActive(false);
                break;
            case ModeType.Fire:
                decoSprite.color = Color.red;
                break;
            case ModeType.Ice:
                decoSprite.color = Color.blue;
                break;
            case ModeType.Thunder:
                decoSprite.color = Color.yellow;
                break;
            case ModeType.Black:
                decoSprite.color = BlackCol;
                break;
            default:
                Debug.Log("Modetype is Ambiguous!!");
                break;
        }
    }
    void Swallow(GameObject monster)
    {
        if (monster.GetComponent<Monster>())
        {
            Monster Monster = monster.GetComponent<Monster>();
            switch (Monster.type)
            {
                case Monster.MonsterType.Normal:
                    mode = ModeType.Black;
                    break;
                case Monster.MonsterType.Fire:
                    mode = ModeType.Fire;
                    break;
                case Monster.MonsterType.Ice:
                    mode = ModeType.Ice;
                    break;
                case Monster.MonsterType.Thunder:
                    mode = ModeType.Thunder;
                    break;
                default:
                    Debug.Log("Monster's type is ambiguous!!");
                    break;
            }
            anim.SetBool("IsSwallow", false);
            eatEffect.SetActive(false);
            Monster.Dead();
            IsEat = false;
            Ztime = -10;
            EatArea.CanSwallow = false;
            ModeUpdate();
        }
        else
        {
            Debug.Log("Monster doesn't have script!!");
        }
    }

    public void ScoreAdd(int n)
    {
        Score += n;
        gameManager.PlayerScoreUpdate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.transform.name);
        if (collision.gameObject.tag == "Enemy")
        {
            if(IsEat && EatArea.CanSwallow)
            {
                Swallow(collision.gameObject);
            }
            else
            {
                OnDamaged(transform.position - collision.transform.position);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish" && stageInput != 0)
        {
            PlaySound(4);
            gameManager.StageChange(collision.gameObject.GetComponent<Finish>());
            Debug.Log("StageChange On!!");
        }
    }
    int CheckMonInRange()
    {
        for (int i = 0; i < EatRay.Length; i++)
        {
            if(EatRay[i].collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                return i;
            }
        }
        return -1;
    }

    bool PlatformCheck(RaycastHit2D[] ray)
    {
        foreach (var item in ray)
        {
            if (item.transform.tag == "Platform")
            {
                return true;
            }
        }
        return false;
    }

    public void Teleport(Vector3 pos)
    {
        this.transform.position = pos;
    }
    void Check()
    {
        if (Ztime < Zswitch && Zstay && Ztime >= 0)
            Ztime++;
        else if(!Zstay)
            Ztime = 0;

        Debug.DrawRay(transform.position, Vector2.down * 0.7f, Color.yellow);
        LandRay = Physics2D.RaycastAll(transform.position, Vector2.down, 0.8f);
        LeftRay = Physics2D.RaycastAll(transform.position, Vector2.left, 0.7f);
        RightRay = Physics2D.RaycastAll(transform.position, Vector2.right, 0.7f);

        IsHorizon = Mathf.Abs(transform.rotation.z) < 0.1f && Mathf.Abs(rigid.angularVelocity) < 1f;

        Landing = PlatformCheck(LandRay);
        if (Landing) Jumpping = 0;

        LeftWall = PlatformCheck(LeftRay);
        RightWall = PlatformCheck(RightRay);

        if (IsDash)
        {
            if (sprite.flipX && LeftWall)
                IsDash = false;
            if (!sprite.flipX && RightWall)
                IsDash = false;
        }
        EatRay = Physics2D.RaycastAll(transform.position + Vector3.down * 0.2f,
           sprite.flipX ? Vector2.left : Vector2.right, EatRange);
        monsterIndex = CheckMonInRange();

        if (IsDash)
        {
            rigid.gravityScale = 0;
        }
        else
        {
            rigid.gravityScale = 2f;
        }

        if(!IsDash && !CanDash && Landing)
        {
            Invoke("CanDashCheck", 0.5f);
        }
    }

    void CanDashCheck()
    {
        CanDash = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsDead)
        {
            Check();
            Command();
            Move();
            Jump();
            Dash();
            Eat();
            Attack();
        }
    }
}
