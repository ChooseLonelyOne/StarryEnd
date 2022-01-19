using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public Action OnDead;
    public Action OnEndLevel;

    public Rigidbody2D rb;
    public Animator animator;
    public bool canMove = true;
    public bool startMove = false;
    public bool end = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheckRadius = groundCheck.GetComponent<CircleCollider2D>().radius;

        wallCheckRadiusDown = wallCheckDown.GetComponent<CircleCollider2D>().radius;
        gravityDef = rb.gravityScale;

        transform.position = spawnPoint[0].position;
        StartCheckMoves();
    }

    private void Update()
    {
        if (canMove)
        {
            WalkControl();
            if (!tackle)
            {
                Walk();
                Reflect();
                Jump();
            }

            MoveOnWall();
            Tackle();
        }
    }
    private void FixedUpdate()
    {
        if (!dead)
        {
            CheckingGround();
            CheckingWall();
            CheckingLedge();
        }
    }

    //--------------------------------------------CheckMoves-----------------------------------------------------------
    #region CheckMoves
    public void StartCheckMoves()
    {
        StartCoroutine(CheckMoves());
    }
    private IEnumerator CheckMoves()
    {
        dead = false;
        end = false;
        canMove = false;
        yield return new WaitForSeconds(.1f);
        canMove = true;
        while (!startMove)
        {
            yield return null;
            if (rb.velocity != new Vector2(0, 0))
            {
                startMove = true;
                StopWatch.StartTime();
            }
        }
    }
    #endregion
    //--------------------------------------------Dead-----------------------------------------------------------------
    #region Dead
    public Transform playerGo;
    public List<Transform> spawnPoint;
    public int selectedPoint = 0;
    private bool dead;
    private void Dead()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        dead = true;
        left = right = up = down = false;
        startMove = canMove = false;
        rb.gravityScale = gravityDef;
        if (onLedge)
            animator.SetFloat("deadX", -1);
        animator.SetBool("onDead", true);
        OnDead?.Invoke();
        StopWatch.StopTime();
        GameManager.Instance.data.Deaths++;
        GameManager.Instance.countOfAttempts++;
    }
    #endregion
    //--------------------------------------------Walk-----------------------------------------------------------------
    #region Walk
    public Vector3 moveVector;
    public bool left;
    public bool right;
    public float speed = 2f;
    private void Walk()
    {
        if (blockMoveXYForLedge)
            moveVector.x = 0;
        else
            rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
        animator.SetFloat("moveX", Mathf.Abs(moveVector.x));
    }
    private void WalkControl()
    {
        if (Input.GetKey(KeyCode.A) || left)
            moveVector.x = -1;
        else
        {
            if (Input.GetKey(KeyCode.D) || right)
                moveVector.x = 1;
            else
                moveVector.x = 0;
        }
        if (Input.GetKey(KeyCode.W) || up)
            moveVector.y = 1;
        else
        {
            if (Input.GetKey(KeyCode.S) || down)
                moveVector.y = -1;
            else
                moveVector.y = 0;
        }
    }
    #endregion
    //--------------------------------------------Reflect--------------------------------------------------------------
    #region Reflect
    public bool faceRight = true;
    private void Reflect()
    {
        if ((moveVector.x > 0 && !faceRight) || (moveVector.x < 0 && faceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
        }
    }
    #endregion
    //--------------------------------------------Jump-----------------------------------------------------------------
    #region Jump
    public float jumpForce = 7f;
    public bool up;
    private void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || up) && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            GameManager.Instance.data.Jumps++;
        }
    }
    #endregion
    // -------------------------------------------CheckingGround-------------------------------------------------------
    #region CheckinGround
    public bool onGround;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask ground;
    private void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
        animator.SetBool("onGround", onGround);

        if (!dead)
        {
            if (tackle)
            {
                animator.SetFloat("deadX", 2f);
                return;
            }
            animator.SetFloat("deadX", Math.Sign(rb.velocity.y));
        }
    }
    #endregion
    //--------------------------------------------CheckingWall---------------------------------------------------------
    #region CheckingWall
    public bool onWall;
    public bool onWallUp;
    public bool onWallDown;
    public Transform wallCheckUp;
    public Transform wallCheckDown;
    public float wallCheckRayDistance = 1f;
    public float wallCheckRadiusDown;
    public LayerMask wall;
    public bool onLedge;
    public float ledgeRayCorrectY = .5f;
    private void CheckingWall()
    {
        onWallUp = Physics2D.Raycast(wallCheckUp.position, new Vector2(transform.localScale.x, 0), wallCheckRayDistance, wall);
        onWallDown = Physics2D.OverlapCircle(wallCheckDown.position, wallCheckRadiusDown, wall);
        onWall = (onWallUp && onWallDown) && !onGround;

        animator.SetFloat("moveY", rb.velocity.y);
    }
    #endregion
    //--------------------------------------------LedgeClimb-----------------------------------------------------------
    #region LedgeClimb
    private void CheckingLedge()
    {
        animator.SetBool("onLedge", onLedge);
        if (onWall)
        {
            var position = wallCheckUp.position;
            onLedge = !Physics2D.Raycast
            (
                new Vector2(position.x, position.y + ledgeRayCorrectY),
                new Vector2(transform.localScale.x, 0),
                wallCheckRayDistance,
                wall
            );
            LedgeGo();
        }
        else
            onLedge = false;

        if ((onLedge && moveVector.y != -1) || blockMoveXYForLedge)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);
            OffestCalculateAndCorrect();
        }
    }

    public float minCorrectDistance = 0.01f;
    public float offsetY;
    private void OffestCalculateAndCorrect()
    {
        var position = wallCheckUp.position;
        offsetY = Physics2D.Raycast
        (
            new Vector2(position.x + wallCheckRayDistance * transform.localScale.x, position.y + ledgeRayCorrectY),
            Vector2.down,
            ledgeRayCorrectY,
            ground
        ).distance;

        if (offsetY > minCorrectDistance * 1.5f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - offsetY + minCorrectDistance, transform.position.z);
        }
    }

    public bool blockMoveXYForLedge;
    public bool checkCollider;
    private void LedgeGo()
    {
        var position = wallCheckUp.position;
        checkCollider = Physics2D.Raycast
        (
            new Vector2(position.x, position.y), 
            Vector2.up,
            3f,
            ground
        );

        if (onLedge && (moveVector.y > 0) && !checkCollider)
        {
            blockMoveXYForLedge = true;
            up = false;

            animator.SetBool("onFinishLedge", true);
            canMove = false;
        }
    }

    public Transform finishLedgePosition;
    private void FinishLedge()
    {
        animator.SetBool("onFinishLedge", false);
        transform.position = new Vector3(finishLedgePosition.position.x, finishLedgePosition.position.y, finishLedgePosition.position.z);
        blockMoveXYForLedge = false;
        rb.velocity = new Vector2(0, 0);
        canMove = true;
    }
    #endregion
    //--------------------------------------------Tackle---------------------------------------------------------------
    #region Tackle
    public bool tackle = false;
    public bool cooldownTackle = false;
    public bool down;
    public CapsuleCollider2D capsuleCollider2D;
    public CircleCollider2D circleCollider2D;
    private void Tackle()
    {
        if ((Input.GetKeyDown(KeyCode.E) || down) && !cooldownTackle && Mathf.Abs(rb.velocity.x) > 2 && onGround)
        {
            StartCoroutine(TackleMove());
        }
        animator.SetBool("onTackle", tackle);
    }

    private IEnumerator TackleMove()
    {
        float axis = Mathf.Sign(rb.velocity.x);
        RaycastHit2D raycastHit2D = Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.size, CapsuleDirection2D.Vertical, 0, new Vector2(axis, 0), 0.2f , ground);
        //RaycastHit2D raycastHit2D = Physics2D.CircleCast(headCollider2D.bounds.center, headCollider2D.radius, new Vector2(axis, 0), 0.2f, ground);
        //print(raycastHit2D);
        if (raycastHit2D.collider == null)
        {
            circleCollider2D.enabled = true;
            capsuleCollider2D.enabled = false;
            down = false;
            speed *= 1.5f;
            rb.velocity = new Vector2(axis * speed, 0);
            tackle = true;
            cooldownTackle = true;
            animator.SetFloat("tackleMoves", Mathf.Abs(axis));
            GameManager.Instance.data.Tackles++;

            for (var i = 0; i < 6; i++)
            {
                yield return new WaitForSeconds(.1f);

                RaycastHit2D hit2D = Physics2D.CircleCast(circleCollider2D.bounds.center, circleCollider2D.radius, new Vector2(axis, 0), 0.2f, ground);
                if (!onGround || moveVector.x != axis || rb.velocity.x == 0 || hit2D.collider != null)
                {
                    animator.SetFloat("tackleMoves", Mathf.Abs(moveVector.y));
                    break;
                }
            }

            animator.SetFloat("tackleMoves", Mathf.Abs(moveVector.y));
            tackle = false;
            speed /= 1.5f;
            RaycastHit2D check = Physics2D.CircleCast(circleCollider2D.bounds.center, circleCollider2D.radius, Vector2.up, .3f, ground);
            if (check.collider != null)
            {
                Dead();
            }
            capsuleCollider2D.enabled = true;
            circleCollider2D.enabled = false;

            yield return new WaitForSeconds(1f);
            cooldownTackle = false;
        }
    }
    #endregion
    //--------------------------------------------MoveOnWall-----------------------------------------------------------
    #region MoveOnWall
    public float upDownSpeed = 12f;
    private float gravityDef;
    private void MoveOnWall()
    {
        if (onWall && moveVector.x == transform.localScale.x)
        {
            animator.SetBool("onWall", true);
            if (moveVector.y == 0)
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(0, 0);
            }
            if (!blockMoveXYForLedge)
            {
                if (moveVector.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, moveVector.y * upDownSpeed / 2);
                }
                else if (moveVector.y < 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, moveVector.y * upDownSpeed);
                }
            }
        }
        else
        {
            rb.gravityScale = gravityDef;
            animator.SetBool("onWall", false);
        }
    }
    #endregion
    //--------------------------------------------Triggers-------------------------------------------------------------
    #region Triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Kill") && !dead)
        {
            Dead();
        }
        if (collision.gameObject.CompareTag("SpawnPoint"))
        {
            for (int i = selectedPoint; i < spawnPoint.Count; i++)
            {
                if (collision.transform == spawnPoint[i])
                {
                    selectedPoint = i;
                    break;
                }
            }
        }
        if (collision.gameObject.CompareTag("End") && !end)
        {
            OnEndLevel?.Invoke();
            end = true;
        }
        if (collision.gameObject.CompareTag("JumpTrampline"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 27);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.parent = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.parent = null;
        }
    }
    #endregion
    //--------------------------------------------OnDestroy------------------------------------------------------------
    #region OnDestroy
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallCheckUp.position, new Vector2(wallCheckUp.position.x + wallCheckRayDistance * transform.localScale.x, wallCheckUp.position.y));

        Gizmos.color = Color.red;
        Gizmos.DrawLine
        (
            new Vector2(wallCheckUp.position.x, wallCheckUp.position.y + ledgeRayCorrectY),
            new Vector2(wallCheckUp.position.x + wallCheckRayDistance * transform.localScale.x, wallCheckUp.position.y + ledgeRayCorrectY)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine
        (
            new Vector2(wallCheckUp.position.x + wallCheckRayDistance * transform.localScale.x, wallCheckUp.position.y + ledgeRayCorrectY),
            new Vector2(wallCheckUp.position.x + wallCheckRayDistance * transform.localScale.x, wallCheckUp.position.y)
        );
    }
    #endregion
}
