using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackBall : MonoBehaviour
{
    public enum ModeType { Normal, Fire, Ice, Thunder, Black };
    public ModeType mode = ModeType.Normal;
    public bool IsLeft = true;
    int dmg;
    SpriteRenderer sprite;
    Rigidbody2D rigid;

    void OnDestroy()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        ModeCheck();
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = 50;
        Invoke("OnDestroy", 2.0f);
    }
    private void Update()
    {
        transform.position += (IsLeft ? Vector3.left : Vector3.right) * 20 * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamaged(dmg, collision.transform.position - transform.position);
            OnDestroy();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            OnDestroy();
        }
    }
    void ModeCheck()
    {
        switch (mode)
        {
            case ModeType.Normal:
                sprite.color = new Color(0, 0, 0, 1);
                dmg = 2;
                break;
            case ModeType.Fire:
                sprite.color = new Color(1, 0, 0, 1);
                dmg = 3;
                break;
            case ModeType.Ice:
                sprite.color = new Color(0, 0.6f, 1f, 1);
                dmg = 3;
                break;
            case ModeType.Thunder:
                sprite.color = new Color(1, 1, 0, 1);
                dmg = 3;
                break;
            case ModeType.Black:
                sprite.color = new Color(0, 0, 0, 1);
                dmg = 3;
                break;
            default:
                break;
        }
    }
    public void Init(Vector3 pos, int modeIndex, bool isLeft)
    {
        IsLeft = isLeft;
        mode = (ModeType)modeIndex;
        transform.position = pos + new Vector3((IsLeft ? (-1) : 1) * 0.5f, -0.2f, 1);
    }

    
}
