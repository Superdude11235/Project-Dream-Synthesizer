using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{   
    public float moveSpeed;
    private GameObject player;
    public bool isChasing;
    public float chaseDistance;
    public Transform[] patrolPoints;
    public int patrolDestination;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        // make enemy stop chasing player if player is twice the chase distance away from enemy
        if (Vector2.Distance(transform.position, player.transform.position) > (chaseDistance * 2))
        {
            isChasing = false;
        }
        
        // fix where enemy's facing depending on where it's going
        if (patrolDestination == 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (patrolDestination == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
        if (isChasing)
        {
            if (transform.position.x > player.transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (transform.position.x < player.transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // if player and enemy are within chase distance, make enemy chase player
            if (Vector2.Distance(transform.position, player.transform.position) < chaseDistance)
            {
                isChasing = true;
            }
            
            // else, move in normal patrol route
            if (patrolDestination == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, patrolPoints[0].position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, patrolPoints[0].position) < .2f)
                {
                    patrolDestination = 1;
                }
            }
            else if (patrolDestination == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, patrolPoints[1].position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, patrolPoints[1].position) < .2f)
                {
                    patrolDestination = 0;
                }
            }
        }
    }
}
