using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class Arrow : MonoBehaviour
{
    [SerializeField] private Vector2 ShotForce = new Vector2(25, 0);
    [SerializeField] private float gravityMultiplier = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;


    const int GROUND_LAYER = 3;
    const int ENEMY_LAYER = 7;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = gravityMultiplier;
    }
    public void ShootArrow(bool isFacingRight)
    {
        if (isFacingRight)
        {
            rb.AddForce(ShotForce, ForceMode2D.Impulse);
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
            rb.AddForce(ShotForce* new Vector2(-1,1), ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == GROUND_LAYER || collision.gameObject.layer == ENEMY_LAYER)
        {
            Destroy(this.gameObject);
        }
    }
}
