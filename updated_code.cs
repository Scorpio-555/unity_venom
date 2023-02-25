using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Venom : MonoBehaviour
{
    Animator anim;
    Vector2 pos;
    Rigidbody2D rigidbody;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D attackCollider;
    private bool special_enable;
    private bool standby;
    private bool attack;
    private bool punch;
    private bool kick;
    private bool hard_punch; // cost 100 energy
    private bool uppercut; // cost 100 energy
    private bool chomp; // cost 2500 energy
    private bool whip;
    private bool walk;
    private bool facing_left;
    private bool standing;
    private bool crawling;
    private string crawl_position; //NA, Left Wall, Right Wall, Floor, Ceiling
    private float crawlAxisValue;
    private bool aerial;
    private bool standing_jump;
    private bool run_jump;
    private bool sprint_jump;
    private bool landing;
    private float speed;
    private int energy;
    private bool switchDirection;
    private int switchDirectionTimer;
    private int whipAnimationPhase;
    private bool flip;
    private float flipZ;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        pos = transform.position;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        bodyCollider = gameObject.GetComponent<CapsuleCollider2D>();
        attackCollider = gameObject.GetComponent<BoxCollider2D>();
        standby = false;
        attack = false;
        punch = false;
        hard_punch = false;
        chomp = false;
        walk = false;
        facing_left = false;
        uppercut = false;
        standing = true;
        crawling = false;
        aerial = false;
        speed = 0.04f;
        energy = 10000;
        switchDirection = false;
        switchDirectionTimer = 0;
        whipAnimationPhase = 0;
        flip = false;
        flipZ = 180f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(flipZ);
        GetInputs();
        MovePlayer();
        AnimatePlayer();
        UpdateColliders();
    }

    //The game calls this function similar to Update
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !standing)
        {
            if (crawling)
            {
                if (crawl_position == "Right Wall")
                {
                    pos.x = pos.x - 0.5f;
                }

                if (crawl_position == "Left Wall")
                {
                    pos.x = pos.x + 0.5f;
                }

                pos.y = collision.gameObject.transform.position.y;
                transform.position = pos;
            }

            standing = true;
            crawling = false;
            crawl_position = "NA";
            aerial = false;
            standing_jump = false;
            run_jump = false;
            sprint_jump = false;
            landing = true;
            flip = false;
        }
        else if (collision.gameObject.CompareTag("Right Wall") && crawl_position != "Right Wall")
        {
            if (standing)
            {
                pos.y = pos.y + 0.5f;
                transform.position = pos;
            }

            switchDirectionTimer = 40;
            crawlAxisValue = collision.gameObject.transform.position.x;
            standing = false;
            walk = false;
            crawling = true;
            crawl_position = "Right Wall";
            aerial = false;
            standing_jump = false;
            run_jump = false;
            sprint_jump = false;
            flip = false;
        }
        else if (collision.gameObject.CompareTag("Left Wall") && crawl_position != "Left Wall")
        {
            if (standing)
            {
                pos.y = pos.y + 0.5f;
                transform.position = pos;
            }

            switchDirectionTimer = 40;
            crawlAxisValue = collision.gameObject.transform.position.x;
            standing = false;
            walk = false;
            crawling = true;
            crawl_position = "Left Wall";
            aerial = false;
            standing_jump = false;
            run_jump = false;
            sprint_jump = false;
            flip = false;
        }
        else if (collision.gameObject.CompareTag("Ceiling") && crawl_position != "Ceiling")
        {
            Debug.Log("hit Ceiling");

            if (crawl_position == "Right Wall")
            {
                pos.x = pos.x - 0.5f;
            }

            if (crawl_position == "Left Wall")
            {
                pos.x = pos.x + 0.5f;
            }

            if (aerial)
            {
                if (facing_left)
                {
                    pos.x = pos.x - 0.5f;
                    facing_left = false;
                }
                else
                {
                    pos.x = pos.x + 0.5f;
                    facing_left = true;
                }
            }

            pos.y = collision.gameObject.transform.position.y;
            transform.position = pos;

            switchDirectionTimer = 40;
            crawlAxisValue = collision.gameObject.transform.position.y;
            standing = false;
            walk = false;
            crawling = true;
            crawl_position = "Ceiling";
            aerial = false;
            standing_jump = false;
            run_jump = false;
            sprint_jump = false;
            flip = false;
        }

    }

    void GetInputs()
    {
        standby = IsStandby();
        ToggleSpecialEnable();
        CheckToStopMovement();
        CheckToMove();
        CheckToStopAttack();
        CheckToAttack();
    }

    void MovePlayer()
    {
        pos = transform.position;

        if ((standing || aerial) && rigidbody.gravityScale != 1.0f && switchDirectionTimer == 0)
        {
            rigidbody.gravityScale = 1.0f;
        }

        if (switchDirection)
        {
            switchDirection = false;
            switchDirectionTimer = 40;

            if (standing)
            {
                if (facing_left)
                {
                    pos.x = pos.x + 1;
                }
                else
                {
                    pos.x = pos.x - 1;
                }
            }

            if (crawling)
            {
                if (crawl_position == "Right Wall")
                {
                    if (facing_left)
                    {
                        pos.y = pos.y + 1;
                    }
                    else
                    {
                        pos.y = pos.y - 1;
                    }
                }

                if (crawl_position == "Left Wall")
                {
                    if (facing_left)
                    {
                        pos.y = pos.y - 1;
                    }
                    else
                    {
                        pos.y = pos.y + 1;
                    }
                }

                if (crawl_position == "Ceiling")
                {
                    if (facing_left)
                    {
                        pos.x = pos.x - 1;
                    }
                    else
                    {
                        pos.x = pos.x + 1;
                    }
                }
            }
        }

        if (walk)
        {
            if (standing)
            {
                if (facing_left == true)
                {
                    pos.x = (float)(pos.x - speed);
                }
                else
                {
                    pos.x = (float)(pos.x + speed);
                }
            }

            if (crawling)
            {
                if (crawl_position == "Right Wall")
                {
                    if (facing_left == true)
                    {
                        pos.y = (float)(pos.y - speed);
                    }
                    else
                    {
                        pos.y = (float)(pos.y + speed);
                    }
                }

                if (crawl_position == "Left Wall")
                {
                    if (facing_left == true)
                    {
                        pos.y = (float)(pos.y + speed);
                    }
                    else
                    {
                        pos.y = (float)(pos.y - speed);

                    }
                }

                if (crawl_position == "Ceiling")
                {
                    if (facing_left == true)
                    {
                        pos.x = (float)(pos.x + speed);
                    }
                    else
                    {
                        pos.x = (float)(pos.x - speed);

                    }
                }
            }
        }

        if (TransitionToJump())
        {
            if (facing_left)
            {
                pos.x = pos.x - 0.05f;
            }
            else
            {
                pos.x = pos.x + 0.05f;
            }

        }

        if (TransitionToLand())
        {
            if (facing_left)
            {
                pos.x = pos.x + 0.05f;
            }
            else
            {
                pos.x = pos.x - 0.05f;
            }

            pos.y = pos.y - 0.1f;
        }

        if (run_jump)
        {
            if (facing_left == true)
            {
                pos.x = (float)(pos.x - 0.02f);
            }
            else
            {
                pos.x = (float)(pos.x + 0.02f);
            }
        }

        if (sprint_jump)
        {
            if (facing_left == true)
            {
                pos.x = (float)(pos.x - 0.04f);
            }
            else
            {
                pos.x = (float)(pos.x + 0.04f);
            }
        }

        if (TransitionToStandbyCrawl())
        {
            rigidbody.gravityScale = 0.0f;
            rigidbody.velocity = Vector3.zero;
        }

        if (crawling)
        {
            if (crawl_position == "Right Wall" || crawl_position == "Left Wall")
            {
                if (Math.Abs(pos.x - crawlAxisValue) > 0.3f)
                {
                    pos.x = crawlAxisValue;
                }
            }
            else
            {
                if (Math.Abs(pos.y - crawlAxisValue) > 0.1f && flip == false)
                {
                    pos.y = crawlAxisValue;
                }
            }
        }

        transform.position = pos;

        if ((standing || aerial) && flip == false)
        {
            if (facing_left)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if (crawling)
        {
            if (crawl_position == "Right Wall")
            {
                if (facing_left)
                {
                    transform.eulerAngles = new Vector3(180, 0, 90);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 90);
                }
            }

            if (crawl_position == "Left Wall")
            {
                if (facing_left)
                {
                    transform.eulerAngles = new Vector3(180, 0, -90);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, -90);
                }
            }

            if (crawl_position == "Ceiling" && flip == false)
            {
                if (facing_left)
                {
                    transform.eulerAngles = new Vector3(0, 180, 180);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 180);
                }
            }
        }
        else if (flip)
        {
            if (flipZ < 270f)
            {
                Debug.Log(flipZ);
                //rigidbody.velocity = new Vector3(0, -2, 0);
                //rigidbody.AddForce(transform.down * );
            }

            if (flipZ < 360f)
            {
                flipZ = flipZ + 2.4f;
            }

            if (facing_left)
            {
                transform.eulerAngles = new Vector3(0, 180, flipZ);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, flipZ);
            }
        }


        if (switchDirectionTimer > 0)
        {
            switchDirectionTimer = switchDirectionTimer - 1;
        }
    }

    void AnimatePlayer()
    {
        if (special_enable && walk)
        {
            anim.speed = 2.0f;
        }

        if (special_enable == false || walk == false)
        {
            anim.speed = 1.0f;
        }

        if (punch == true)
        {
            anim.SetBool("Punch", true);
        }
        else
        {
            anim.SetBool("Punch", false);
        }

        if (hard_punch == true)
        {
            anim.SetBool("Hard Punch", true);
        }
        else
        {
            anim.SetBool("Hard Punch", false);
        }

        if (kick == true)
        {
            anim.SetBool("Kick", true);
        }
        else
        {
            anim.SetBool("Kick", false);
        }

        if (uppercut == true)
        {
            anim.SetBool("Upper", true);
        }
        else
        {
            anim.SetBool("Upper", false);
        }

        if (chomp == true)
        {
            anim.SetBool("Chomp", true);
        }
        else
        {
            anim.SetBool("Chomp", false);
        }

        if (whip == true)
        {
            anim.SetBool("Whip", true);
        }
        else
        {
            anim.SetBool("Whip", false);
        }

        if (walk == true)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        if (aerial == true)
        {
            anim.SetBool("Jump", true);
        }
        else
        {
            anim.SetBool("Jump", false);
        }

        if (crawling == true)
        {
            anim.SetBool("Crawl", true);
        }
        else
        {
            anim.SetBool("Crawl", false);
        }
    }

    void UpdateColliders()
    {

        if (TransitionToStandby())
        {
            ResetBodyCollider();
            ResetAttackCollider();
        }

        if (TransitionToWalk())
        {
            bodyCollider.offset = new Vector2(0.7f, 0.6f);
            bodyCollider.size = new Vector2(1.5f, 1.3f);
        }

        if (TransitionToPunch())
        {
            attackCollider.offset = new Vector2(1.7f, 0.65f);
            attackCollider.size = new Vector2(1.0f, 0.35f);
        }

        if (TransitionToKick())
        {
            attackCollider.offset = new Vector2(1.7f, 0.2f);
            attackCollider.size = new Vector2(0.6f, 0.45f);
        }

        if (TransitionToUppercut())
        {
            bodyCollider.offset = new Vector2(0.7f, 0.6f);
            bodyCollider.size = new Vector2(1.3f, 1.3f);

            attackCollider.offset = new Vector2(1.1f, 1.2f);
            attackCollider.size = new Vector2(0.8f, 0.6f);
            attackCollider.edgeRadius = .2f;
        }

        if (TransitionToHardPunch())
        {
            attackCollider.offset = new Vector2(1.7f, 0.35f);
            attackCollider.size = new Vector2(0.5f, 0.75f);
        }

        if (TransitionToChomp())
        {
            attackCollider.offset = new Vector2(2.2f, 0.8f);
            attackCollider.size = new Vector2(1.8f, 1.0f);
        }

        if (TransitionToWhip())
        {
            bodyCollider.offset = new Vector2(0.8f, 0.8f);
            bodyCollider.size = new Vector2(1.2f, 1.5f);
            bodyCollider.direction = (CapsuleDirection2D)(0);
        }

        if (whip)
        {
            float frame = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

            if (frame > 0.16f && frame < 0.57f && whipAnimationPhase == 0)
            {
                whipAnimationPhase = 1;

                attackCollider.offset = new Vector2(1.3f, 1.2f);
                attackCollider.size = new Vector2(2.2f, 1.5f);
            }
            else if (frame > 0.57f && whipAnimationPhase == 1)
            {
                whipAnimationPhase = 0;
                ResetAttackCollider();
            }
        }

        if (TransitionToJump())
        {
            bodyCollider.offset = new Vector2(0.1f, 0.1f);
            bodyCollider.size = new Vector2(1.0f, 1.5f);
            bodyCollider.direction = (CapsuleDirection2D)(0);
        }

        if (landing)
        {
            landing = false;
            ResetBodyCollider();
        }

        if (TransitionToStandbyCrawl())
        {
            bodyCollider.offset = new Vector2(0.7f, 0.4f);
            bodyCollider.size = new Vector2(1.5f, 0.9f);
        }

        if (TransitionToCrawling())
        {
            bodyCollider.offset = new Vector2(0.9f, 0.4f);
            bodyCollider.size = new Vector2(1.8f, 0.9f);
        }
    }

    bool IsStandby()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Standby"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ToggleSpecialEnable()
    {
        if (special_enable)
        {
            uppercut = false;
            hard_punch = false;
            punch = false;
            kick = false;
        }
        else
        {
            chomp = false;
            whip = false;
        }

        if (Input.GetButtonDown("Special Enable") && energy > 0)
        {
            special_enable = true;
            speed = speed + 0.02f;
        }

        if (Input.GetButtonUp("Special Enable") && energy > 0)
        {
            special_enable = false;
            speed = speed - 0.02f;
        }

        if (special_enable && energy == 0)
        {
            special_enable = false;
            speed = speed - 0.02f;
        }
    }

    void CheckToStopMovement()
    {
        if (flip == false && flipZ != 180f)
        {
            flipZ = 180f;
        }

        if (attack || switchDirectionTimer > 0 || aerial)
        {
            walk = false;
        }

        if (standing)
        {
            if (Input.GetButton("Right") == false && Input.GetButton("Left") == false)
            {
                walk = false;
            }

            if (Input.GetButton("Right") && Input.GetButton("Left"))
            {
                walk = false;
            }
        }
        else if (crawling)
        {
            if (crawl_position == "Right Wall" || crawl_position == "Left Wall")
            {
                if (Input.GetButton("Up") == false && Input.GetButton("Down") == false)
                {
                    walk = false;
                }

                if (Input.GetButton("Up") && Input.GetButton("Down"))
                {
                    walk = false;
                }
            }
            else if (crawl_position == "Ceiling")
            {
                if (Input.GetButton("Right") == false && Input.GetButton("Left") == false)
                {
                    walk = false;
                }

                if (Input.GetButton("Right") && Input.GetButton("Left"))
                {
                    walk = false;
                }
            }

        }

        if (aerial)
        {
            crawling = false;
            crawl_position = "NA";
        }

        //To check if we are done doing free fall, we check to see if you've collided with ground.
        //This is taken care of in the fuction named void OnCollisionEnter2D(Collision2D collision){...}
    }

    void CheckToMove()
    {
        if (standby)
        {
            if (Input.GetButton("Right") && !Input.GetButton("Left") && switchDirectionTimer < 1)
            {
                walk = true;

                if (facing_left)
                {
                    switchDirection = true;
                    facing_left = false;
                }

            }
            else if (Input.GetButton("Left") && !Input.GetButton("Right") && switchDirectionTimer < 1)
            {
                walk = true;

                if (facing_left == false)
                {
                    switchDirection = true;
                    facing_left = true;
                }
            }
        }

        if (standby || (walk && standing))
        {
            if ((Input.GetButtonDown("Up") || Input.GetButtonDown("Jump")) && aerial == false) // NOTE: I would like to split the up vs space button
            {
                aerial = true;
                standing = false;
                rigidbody.AddForce(new Vector2(0f, 7f), ForceMode2D.Impulse);

                if (standby)
                {
                    standing_jump = true;
                }

                if (walk)
                {
                    if (speed == 0.02f)
                    {
                        run_jump = true;
                    }
                    else if (speed == 0.04f)
                    {
                        sprint_jump = true;
                    }
                }
            }
        }

        if (crawling)
        {
            if (crawl_position == "Right Wall")
            {
                if (Input.GetButton("Up") && !Input.GetButton("Down") && switchDirectionTimer < 1)
                {
                    walk = true;

                    if (facing_left)
                    {
                        switchDirection = true;
                        facing_left = false;
                    }

                }
                else if (Input.GetButton("Down") && !Input.GetButton("Up") && switchDirectionTimer < 1)
                {
                    walk = true;

                    if (facing_left == false)
                    {
                        switchDirection = true;
                        facing_left = true;
                    }
                }

                if ((Input.GetButton("Left") || Input.GetButtonDown("Jump")) && aerial == false) // NOTE: I would like to allow space to have you jump off wall
                {
                    pos.x = pos.x - 0.5f;
                    if (facing_left == false)
                    {
                        pos.y = pos.y + 1.0f;
                    }

                    transform.position = pos;

                    rigidbody.AddForce(new Vector2(0f, 4f), ForceMode2D.Impulse);

                    Debug.Log("Jump off wall?");
                    aerial = true;
                    crawling = false;
                    crawl_position = "NA";
                    switchDirectionTimer = 40;
                    run_jump = true;
                    facing_left = true;
                }
            }

            if (crawl_position == "Left Wall")
            {
                if (Input.GetButton("Up") && !Input.GetButton("Down") && switchDirectionTimer < 1)
                {
                    walk = true;

                    if (facing_left == false)
                    {
                        switchDirection = true;
                        facing_left = true;
                    }

                }
                else if (Input.GetButton("Down") && !Input.GetButton("Up") && switchDirectionTimer < 1)
                {
                    walk = true;

                    if (facing_left)
                    {
                        switchDirection = true;
                        facing_left = false;
                    }
                }

                if ((Input.GetButton("Right") || Input.GetButtonDown("Jump")) && aerial == false) // NOTE: I would like to allow space to make you jump off wall
                {
                    pos.x = pos.x + 0.5f;

                    if (facing_left)
                    {
                        pos.y = pos.y + 1.0f;
                    }

                    transform.position = pos;

                    rigidbody.AddForce(new Vector2(0f, 4f), ForceMode2D.Impulse);

                    Debug.Log("Jump off wall?");
                    aerial = true;
                    switchDirectionTimer = 40;
                    run_jump = true;
                    facing_left = false;
                }
            }

            if (crawl_position == "Ceiling")
            {
                if (Input.GetButton("Left") && !Input.GetButton("Right") && switchDirectionTimer < 1)
                {
                    walk = true;

                    if (facing_left)
                    {
                        switchDirection = true;
                        facing_left = false;
                    }

                }
                else if (Input.GetButton("Right") && !Input.GetButton("Left") && switchDirectionTimer < 1)
                {
                    walk = true;

                    if (facing_left == false)
                    {
                        switchDirection = true;
                        facing_left = true;
                    }
                }

                if ((Input.GetButton("Down") || Input.GetButtonDown("Jump")) && aerial == false) // NOTE: I would like to allow space to make you drop from ceiling
                {
                    if (facing_left)
                    {
                        pos.x = pos.x + 0.7f;
                        facing_left = false;
                    }
                    else
                    {
                        pos.x = pos.x - 0.7f;
                        facing_left = true;
                    }

                    pos.y = pos.y - 1.0f;
                    transform.position = pos;

                    rigidbody.AddForce(new Vector2(0f, -2.5f), ForceMode2D.Impulse);

                    aerial = true;
                    flip = true;
                    switchDirectionTimer = 40;
                }
            }
        }
    }

    void CheckToStopAttack()
    {
        if (standby)
        {
            attack = false;
            uppercut = false;
            hard_punch = false;
            punch = false;
            kick = false;
            chomp = false;
        }

        if (special_enable == false || Input.GetButton("Hard Punch") == false || energy == 0)
        {
            attack = false;
            whip = false;
        }
    }

    void CheckToAttack()
    {
        if (whip)
        {
            energy = energy - 4;
        }

        if ((standby || walk) && standing)
        {
            if (special_enable)
            {
                if (Input.GetButtonDown("Kick") && energy > 2499)
                {
                    attack = true;
                    chomp = true;
                    energy = energy - 2500;
                }
                else if (Input.GetButton("Hard Punch") && energy > 0)
                {
                    attack = true;
                    whip = true;
                }
            }
            else
            {
                if (Input.GetButtonDown("Upper") && energy > 99)
                {
                    attack = true;
                    uppercut = true;
                    energy = energy - 100;
                }
                else if (Input.GetButtonDown("Hard Punch") && energy > 99)
                {
                    attack = true;
                    hard_punch = true;
                    energy = energy - 100;
                }
                else if (Input.GetButtonDown("Punch"))
                {
                    attack = true;
                    punch = true;
                }
                else if (Input.GetButtonDown("Kick"))
                {
                    attack = true;
                    kick = true;
                }
            }
        }

        if (energy < 0)
        {
            energy = 0;
        }
    }

    void ResetAttackCollider()
    {
        attackCollider.offset = new Vector2(0.5f, 0.7f);
        attackCollider.size = new Vector2(0.001f, 0.001f);
        attackCollider.edgeRadius = 0;
    }

    void ResetBodyCollider()
    {
        bodyCollider.offset = new Vector2(0.7f, 0.5f);
        bodyCollider.size = new Vector2(1.6f, 1.0f);
        bodyCollider.direction = (CapsuleDirection2D)(1);
    }

    bool TransitionToStandby()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Suit_Up -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Punch -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Kick -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Uppercut -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Hard_Punch -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Chomp -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Whip -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Land -> Standby"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Standby_Crawl -> Standby"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToWalk()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Walk"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToPunch()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Punch"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Punch"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToKick()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Kick"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Kick"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToUppercut()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Uppercut"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Uppercut"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToHardPunch()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Hard_Punch"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Hard_Punch"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToChomp()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Chomp"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Chomp"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToWhip()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Begin_Whip"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Begin_Whip"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToJump()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Jump"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Jump"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Standby_Crawl -> Jump"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Crawling -> Jump"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToLand()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Jump -> Land"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToStandbyCrawl()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby -> Standby_Crawl"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Walk -> Standby_Crawl"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Jump -> Standby_Crawl"))
        {
            return true;
        }
        else if (anim.GetAnimatorTransitionInfo(0).IsName("Crawling -> Standby_Crawl"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TransitionToCrawling()
    {
        if (anim.GetAnimatorTransitionInfo(0).IsName("Standby_Crawl -> Crawling"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
