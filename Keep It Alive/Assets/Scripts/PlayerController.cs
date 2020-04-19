using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameMaster GM; 

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12f;

    Animator anim;
    CharacterController controller;

    public float turnSmoothTime = .2f;
    float turnSmoothVelocity;
    public float speedSmoothTime = .1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    Transform cameraT;

    //sound effect :
    AudioSource stepSfx;
    public AudioClip[] walkSfxClip;
    public AudioClip[] runSfxClip;
    public AudioClip deadSfxClip;

    // hugging variable :
    bool isHugging = false;
    public float hugRate = 0.5f;
    float hugTime = 0;
    public GameObject hugCollider;

    bool runOnce;

    void Start()
    {
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        stepSfx = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        hugCollider.SetActive(false);
    }

    void Update()
    {
        // player is dead :
        if (GM.HeartPoint <= 0)
        {
            PlayerIsDead();
            return;
        }

        hugTime += Time.deltaTime;
        // input
        if (Input.GetButtonDown("Fire1") && hugTime > hugRate){
            HuggingEnemy();
        }

        if(!isHugging)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            bool running = Input.GetKey(KeyCode.LeftShift);
            Move(inputDir, running);
            // animator
            float animSpeedPercent = (running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f;
            anim.SetFloat("speed", animSpeedPercent, speedSmoothTime, Time.deltaTime);
        }

    }

    void HuggingEnemy()
    {
        hugTime = 0;
        StartCoroutine("DelayForHugging");
        anim.SetTrigger("hugging");
    }

    IEnumerator DelayForHugging()
    {
        isHugging = true;
        hugCollider.SetActive(true);
        yield return new WaitForSeconds(hugRate);
        hugCollider.SetActive(false);
        isHugging = false;
    }

    void Move (Vector2 inputDir, bool running)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRot = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, turnSmoothTime);
            if (running) {
                if (!stepSfx.isPlaying) stepSfx.PlayOneShot(runSfxClip[Random.Range(0, runSfxClip.Length)]);
            }
            else {
                if (!stepSfx.isPlaying) stepSfx.PlayOneShot(walkSfxClip[Random.Range(0, walkSfxClip.Length)]);
            }
        }
        else
        {
            stepSfx.Stop();
        }

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        velocityY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded) velocityY = 0;
    }

    void PlayerIsDead()
    {
        if (!runOnce)
        {
            runOnce = true;
            stepSfx.PlayOneShot(deadSfxClip);
            controller.enabled = false;
            anim.SetBool("isDead", true);
            Destroy(this, 6f);
        }
    }
}
