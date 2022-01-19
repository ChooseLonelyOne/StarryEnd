using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pos1, pos2;
    [SerializeField] private float speed;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(pos1.position, pos2.position, Mathf.PingPong(Time.time * speed, 1.0f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            new Vector2(pos1.position.x - spriteRenderer.size.x * transform.localScale.x / 2, pos1.position.y),
            new Vector2(pos2.position.x + spriteRenderer.size.x * transform.localScale.x / 2, pos2.position.y)
            );
    }
}
