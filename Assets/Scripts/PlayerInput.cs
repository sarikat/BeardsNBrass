using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour, IMovement
{
    public bool armed;
    public bool ConnectMoveAndAttack = false;

    public float MovementSpeed = 1.8f;
    public float DodgeCooldown = 2f;
    public float DodgeJumpStrength = 1f;
    public float DodgeTime = 0.3f;
    public int DodgeStamina = 40;
    public float DrunkSpeed = 0.1f;
    public float DrunkStaggerStrength = 5f;
    public GameObject Sprites;
    public CanGrapple GrappleControl;
    public ParticleSystem DrunkParticles;

    public ParticleSystem dust;
    public Transform weapon_trans;

    Animator player_anim;
    Gamepad player_gamepad;
    int player_num;
    bool is_keyboard;
    Weapon player_weapon;
    Health player_health;
    Rigidbody2D rigidbody2D;
    StatusEffects player_status;
    float part_1 = -100;
    float part_2 = 0;

    // for combo atks
    bool canPress = true;
    int numButtonPresses = 0;
    float drink_timer = 40;
    float stagger_timer = 0;
    float stagger_timer_old = 0;
    Vector2 dir_offset;
    static float prompt_wait = 90;
    static float pause = 0f;
    public GameObject level_up;
    bool is_op = false;



    private bool controlsAreLocked = false;
    //private float viewportMin = 0.1f, viewportMax = 0.9f;
    float dodge_timer = 999f;
    Stamina stamina;
    HasHeading heading;
    float speed_mod = 1;

    private void Awake()
    {
        player_anim = GetComponent<Animator>();
        player_weapon = GetComponent<Weapon>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        stamina = GetComponent<Stamina>();
        heading = GetComponent<HasHeading>();
        player_health = GetComponent<Health>();
        player_status = GetComponent<StatusEffects>();

        if (DrunkParticles)
        {
            DrunkParticles.Stop();
        }
    }

    public void SetPlayerInput(bool using_keyboard, int new_player_num)
    {
        player_num = new_player_num;
        is_keyboard = using_keyboard;
        if (player_num < Gamepad.all.Count)
        {
            player_gamepad = Gamepad.all[player_num];
        }
    }

    // Update is called once per frame
    void Update()
    {
        // player_anim.SetInteger("Anim_num", 0);
        if (controlsAreLocked) return;
        dodge_timer += TimeManager.deltaTime;
        drink_timer += TimeManager.deltaTime;

        // sverg: catalyst for respawn testing
        // if ((player_gamepad != null && player_gamepad.rightShoulder.wasPressedThisFrame) || Input.GetKeyDown(KeyCode.X))
        // {
        //     GetComponent<Health>().TakeDamage(5000);
        // }

        if (dust != null)
        {
            var em = dust.emission;
            em.enabled = true;
        }

        Vector2 input_dir = is_keyboard ? GetKeyboardMove() : GetGamepadMove();
        if (input_dir != Vector2.zero)
        {
            heading.SetHeading(input_dir);
        }

        if (player_gamepad != null)
        {
            if (player_gamepad.rightStickButton.wasPressedThisFrame)
            {
                part_1 = TimeManager.time;
            }

            if (player_gamepad.leftStickButton.wasPressedThisFrame)
            {
                part_2 = TimeManager.time;
            }
        }

        if (!is_op && (Mathf.Abs(part_1 - part_2) < 0.2f || Input.GetKeyDown(KeyCode.I)))
        {
            Instantiate(level_up, transform.position, Quaternion.identity);
            CameraFollow.ShakeScreen(2f, 5f);
            player_weapon.AttackUp(90, 0.8f);
            player_health.RegenUp(90, 10);
            player_status.Stun(2);
            player_weapon.as_mod = 0.001f;
            speed_mod = 3;
            player_health.MaxHealth = 1000;
            SoundManager.PlaySound(SoundManager.Sound.EnemyDeath1);
            is_op = true;
        }

        bool drinking = is_keyboard ? GetKeyboardDrink() : GetGamepadDrink();
        if (drinking & drink_timer > 30)
        {
            Debug.Log("Drinking");
            player_anim.SetTrigger("Drinking");
            drink_timer = 0;
            stagger_timer = 0;
            stagger_timer_old = 0;
            prompt_wait = 70f;
            pause = 0;
            DrunkParticles.Play();
            rigidbody2D.velocity = Vector2.zero;
            player_status.Stun(1.5f);
            player_weapon.AttackUp(30, 2);
            player_health.RegenUp(30, 10);
        }
        if (drink_timer < 30)
        {
            if (drink_timer > stagger_timer)
            {
                stagger_timer_old = stagger_timer;
                float noise = Random.Range(0, 360f);
                dir_offset = new Vector2(Mathf.Cos(noise), Mathf.Sin(noise));
                stagger_timer += Random.Range(1f, 3f);
            }
        }
        else
        {
            dir_offset = Vector2.zero;
            DrunkParticles.Stop();
        }

        if (drink_timer - pause > prompt_wait)
        {
            pause = drink_timer;
            Toast.main.ShowToast(("I'm Getting a Bit Thirsty!!! (Press Left Bumper for an Attack and Regen Boost!)", 6));
        }

        if (controlsAreLocked) return;

        // Vector2 attack_dir = input_dir; //is_keyboard ? GetKeyboardAttackDir() : GetGamepadAttackDir();
        Vector2 attack_dir = is_keyboard ? GetKeyboardAttackDir() : GetGamepadAttackDir();
        attack_dir = ConnectMoveAndAttack ? heading.GetHeading() : attack_dir;

        var dir_indicator = gameObject.transform.GetChild(4).GetComponent<DirectionIndicator>();
        if (attack_dir == Vector2.zero)
        {
            dir_indicator.SetAngle(Mathf.Atan2(input_dir.x, input_dir.y) * Mathf.Rad2Deg);
        }
        else
        {
            dir_indicator.SetAngle(Mathf.Atan2(attack_dir.x, attack_dir.y) * Mathf.Rad2Deg);
        }


        bool dodging = is_keyboard ? GetKeyboardRoll() : GetGamepadRoll();
        if (dodge_timer >= DodgeCooldown && dodging && stamina.TryExertStamina(DodgeStamina))
        {
            Vector2 dodge_dir = is_keyboard ? attack_dir : input_dir;
            StartCoroutine(Slide(dodge_dir, DodgeJumpStrength, DodgeTime));
            player_anim.SetTrigger("Rolling");
        }
        else
        {
            Move(input_dir);
        }


        bool attack = is_keyboard ? GetKeyboardAttack() : GetGamepadAttack();
        weapon_trans.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(attack_dir.x, attack_dir.y) * Mathf.Rad2Deg);
        if (attack)
        {
            int idx1 = Random.Range(0, 3);
            SoundManager.Sound[] hitsoundarr = { SoundManager.Sound.Hit1, SoundManager.Sound.Hit2, SoundManager.Sound.Hit3 };
            SoundManager.PlaySound(hitsoundarr[idx1]);
            player_weapon.Attack(attack_dir);

        }

        AnimateFacing(attack_dir, input_dir);

        UpdateGrapple(attack_dir);

    }

    void AnimateFacing(Vector2 attack_direction, Vector2 movement_direction)
    {
        Vector2 facing_direction = Vector2.zero;
        if (attack_direction == Vector2.zero)
        {
            facing_direction = movement_direction;
        }
        else
        {
            facing_direction = attack_direction;
        }
        player_anim.SetFloat("HorizontalFacing", facing_direction.y * -3);
        if (facing_direction.x > 0.01f)
        {
            Sprites.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (facing_direction.x < -0.01f)
        {
            Sprites.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void ComboStarter()
    {
        if (canPress)
        {
            Debug.Log("Incrementing button presses");
            numButtonPresses++;
        }
        else
        {
            player_anim.SetInteger("Anim_num", 1);
        }
        Debug.Log("Starting the combo");
        ComboCheck();
    }

    void ComboCheck()
    {
        canPress = false;
        // if we first anim is playing and only 1 press happened, return to idle
        if (player_anim.GetCurrentAnimatorStateInfo(0).IsName("atk_1"))
        {
            player_anim.SetInteger("Anim_num", 0);
            canPress = true;
            numButtonPresses = 0;
        }
        // if player has pressed again, continue combo
        else if (player_anim.GetCurrentAnimatorStateInfo(0).IsName("atk_1") && numButtonPresses == 2)
        {
            player_anim.SetInteger("Anim_num", 1);
            canPress = true;
        }
        // if second anim still playing, and only 2 presses, return to idle
        else if (player_anim.GetCurrentAnimatorStateInfo(0).IsName("atk_2") && numButtonPresses == 2)
        {
            player_anim.SetInteger("Anim_num", 0);
            numButtonPresses = 0;
        }
        // if second anim still playing and we get another press, continue the combo
        else if (player_anim.GetCurrentAnimatorStateInfo(0).IsName("atk_2") && numButtonPresses >= 3)
        {
            player_anim.SetInteger("Anim_num", 2);
            canPress = true;
        }
        // we are on the third and last combo, so return to idle
        else if (player_anim.GetCurrentAnimatorStateInfo(0).IsName("atk_3"))
        {
            player_anim.SetInteger("Anim_num", 0);
            canPress = true;
            numButtonPresses = 0;
        }

    }


    public bool IsInteracting()
    {
        if (is_keyboard)
        {
            return Input.GetKey(KeyCode.E);
        }
        else
        {
            return player_gamepad.xButton.isPressed;
        }
    }

    public void LockControls()
    {
        controlsAreLocked = true;
    }

    // public void LockControlsFor(float time) {
    //     StartCoroutine(LockControlsForCoroutine(time));
    // }

    // IEnumerator LockControlsForCoroutine(float time) {
    //     controlsAreLocked = true;
    //     float timer = 0;
    //     while (timer < time) {
    //         timer += TimeManager.deltaTime;
    //         yield return null;
    //     }
    //     controlsAreLocked = false;
    // }

    public void UnlockControlsAndRestartMovement()
    {
        controlsAreLocked = false;
    }

    Vector2 GetKeyboardMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector2(horizontal, vertical).normalized;
    }

    bool GetKeyboardAttack()
    {
        return Input.GetMouseButton(0);
    }

    Vector2 GetKeyboardAttackDir()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
    }

    bool GetKeyboardRoll()
    {
        return Input.GetMouseButtonDown(1);
    }

    bool GetKeyboardGrapple()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    bool GetKeyboardDrink()
    {
        return Input.GetKeyDown(KeyCode.V);
    }

    Vector2 GetGamepadMove()
    {
        if (player_gamepad == null)
        {
            return Vector2.zero;
        }

        return new Vector2(
            player_gamepad.leftStick.x.ReadValue(),
            player_gamepad.leftStick.y.ReadValue()
        ).normalized;
    }

    bool GetGamepadAttack()
    {
        if (player_gamepad == null)
        {
            return false;
        }
        return ConnectMoveAndAttack ? player_gamepad.xButton.isPressed : player_gamepad.leftTrigger.wasPressedThisFrame;
    }

    bool GetNextDialogueOption()
    {
        if (player_gamepad == null)
        {
            return false;
        }
        return player_gamepad.xButton.wasPressedThisFrame;
    }


    Vector2 GetGamepadAttackDir()
    {
        if (player_gamepad == null)
        {
            return Vector2.zero;
        }

        return new Vector2(
            player_gamepad.rightStick.x.ReadValue(),
            player_gamepad.rightStick.y.ReadValue()
        ).normalized;

    }

    bool GetGamepadRoll()
    {
        if (player_gamepad == null)
        {
            return false;
        }
        return player_gamepad.bButton.wasPressedThisFrame;
    }

    bool GetGamepadGrapple()
    {
        if (player_gamepad == null)
        {
            return false;
        }
        return ConnectMoveAndAttack ? player_gamepad.yButton.wasPressedThisFrame : player_gamepad.leftShoulder.wasPressedThisFrame;
    }

    bool GetGamepadDrink()
    {
        if (player_gamepad == null)
        {
            return false;
        }
        return ConnectMoveAndAttack ? player_gamepad.leftShoulder.wasPressedThisFrame : player_gamepad.yButton.wasPressedThisFrame;
    }

    void Move(Vector2 direction)
    {
        direction = (direction).normalized;

        player_anim.SetFloat("Speed", direction.magnitude * MovementSpeed);
        // float angle = Vector2.SignedAngle(Vector2.right, direction);
        // angle *= Mathf.Rad2Deg;
        // Debug.Log("fah");

        float lurch_strength = Mathf.Max(drink_timer - stagger_timer_old, 0f);
        lurch_strength = lurch_strength > 1f ? 0f : (float)lurch_strength;
        // Debug.Log("Lurch: " + lurch_strength);
        // Debug.Log(dir_offset);
        // Debug.Log("Dif: " + drink_timer + " : " + stagger_timer_old);

        rigidbody2D.velocity = direction * MovementSpeed * speed_mod + dir_offset.normalized * lurch_strength * DrunkSpeed * 2.5f * direction.magnitude;
        //var em = dust.emission;
        //em.enabled = true;
        //dust.Play();
        ClampPosition();
    }

    void ClampPosition()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        viewportPos.x = Mathf.Clamp01(viewportPos.x);
        viewportPos.y = Mathf.Clamp01(viewportPos.y);

        Vector3 clampedPos = Camera.main.ViewportToWorldPoint(viewportPos);
        clampedPos.z = 0;
        transform.position = clampedPos;
        // Debug.Log("Player " + player_num + ": "+ viewportPos + " (Viewport); " + clampedPos + " (Clamp)");
    }

    public Gamepad GetGamepad()
    {
        return player_gamepad;
    }

    public int GetPlayerNum()
    {
        return player_num;
    }

    IEnumerator Slide(Vector2 direction, float distance, float time)
    {
        SoundManager.PlaySound(SoundManager.Sound.Dash);
        dodge_timer = -10f;
        controlsAreLocked = true;
        float slide_time = 0;
        float current_distance = 0;

        while (slide_time < time)
        {
            slide_time += TimeManager.deltaTime;
            float new_distance = Mathf.Exp(5 * ((slide_time / time) - 1)) * distance;

            rigidbody2D.velocity = direction * distance / time;

            // Vector3 current_position = transform.position;
            // current_position.x = current_position.x + direction.x * (new_distance - current_distance);
            // current_position.y = current_position.y + direction.y * (new_distance - current_distance);
            // transform.position = current_position;
            // current_distance = new_distance;
            yield return null;
        }
        rigidbody2D.velocity = Vector2.zero;
        controlsAreLocked = false;
        dodge_timer = 0;
    }

    void UpdateGrapple(Vector2 attackDir)
    {
        bool grapple_button = is_keyboard ? GetKeyboardGrapple() : GetGamepadGrapple();

        if (grapple_button)
        {
            var g = GetComponent<CanGrapple>();
            g?.Grapple(attackDir);
        }
    }
}
