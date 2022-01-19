using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCollider : MonoBehaviour
{
    [SerializeField] private bool isTrigger = true;
    private BoxCollider2D coll;

    
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        coll.isTrigger = isTrigger;
        coll.size = GetComponent<SpriteRenderer>().size;
        Destroy(this);
    }
}
