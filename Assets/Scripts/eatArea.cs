using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eatArea : MonoBehaviour
{
    public bool CanSwallow = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!CanSwallow && 
            collision.gameObject.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            CanSwallow = true;
        }
    }
}
