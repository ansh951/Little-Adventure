using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float moveSpeed = 5f;
    private Vector3 _movementVelocity;
    private PlayerMovement _playerInput;
    private Animator _animator;
    private float _verticalVelocity;
    [SerializeField]
    private float gravity = -9.8f;
    public int Coin;

    public bool isPlayer = true;
    private NavMeshAgent _navMeshAgent;
    private Transform targetTransform;

    float attackStartTime;
    public float AttackSlideDuration = 0.4f;
    public float AttackSlideSpeed = 0.6f;

    private Health _health;
    private DamageCaster _damageCaster;

    public GameObject health_Orb;
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn
    }

    public CharacterState currentState;

    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    private  float spawnDuration = 2f;
    private float currentSpawnTime;

    private Vector3 impactOnCharcter;

    public bool IsInvincible;
    public float IsInvinsibleDuration = 2f;

    private float attackAnimationDuration;

    public float slideSpeed = 9f;

    public Transform healthOrbPos;

    // Start is called before the rst frame update
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        if (!isPlayer)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
            _navMeshAgent.speed = moveSpeed;
            SwitchStateTo(CharacterState.Spawn);
        }

        else
        {
            _playerInput = GetComponent<PlayerMovement>();
        }
    }

    private void CalculatePlayerMovement()
    {
        if (_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }

        else if (_playerInput.spaceKeyDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        _movementVelocity.Set(_playerInput.horizontalInput, 0f, _playerInput.verticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0f, -45f, 0f) * _movementVelocity;

        _animator.SetFloat("Speed", _movementVelocity.magnitude);
        _movementVelocity *= moveSpeed * Time.deltaTime;



        if (_movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }

    private void CalculateEnemyMovement()
    {
        if (Vector3.Distance(targetTransform.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(targetTransform.position);
            _animator.SetFloat("Speed", 0.2f);

        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);
            SwitchStateTo(CharacterState.Attacking);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                {
                    CalculatePlayerMovement();
                }

                else
                {
                    CalculateEnemyMovement();
                }
                break;

            case CharacterState.Attacking:

                if (isPlayer)
                {
                    

                    if(Time.time < attackStartTime + AttackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if (_playerInput.MouseButtonDown && _cc.isGrounded)
                    {
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if (currentClipName != "LittleAdventurerAndie_ATTACK_03" && attackAnimationDuration > 0.5f && attackAnimationDuration < 0.7f)
                        {
                            _playerInput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);
                            CalculatePlayerMovement();
                        }
                    }
                }

                break;
            case CharacterState.Dead:
                return;

            case CharacterState.BeingHit:


                break;

            case CharacterState.Slide:
                _movementVelocity = transform.forward * slideSpeed * Time.deltaTime;
                break;

            case CharacterState.Spawn:
                currentSpawnTime -= Time.deltaTime;
                if(currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        if (impactOnCharcter.magnitude > 0.2f)
        {
            _movementVelocity = impactOnCharcter * Time.deltaTime;
        }
        impactOnCharcter = Vector3.Lerp(impactOnCharcter, Vector3.zero, Time.deltaTime * 5);

        if (isPlayer)
        {

            if (_cc.isGrounded == false)
            {
                _verticalVelocity = gravity;


            }

            else
            {
                _verticalVelocity = gravity * 0.3f;
            }

            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;
            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }
        else
        {
            if(currentState != CharacterState.Normal)
            {
                _cc.Move(_movementVelocity);
                _movementVelocity = Vector3.zero;
            }
        }

    }

    public void SwitchStateTo(CharacterState newState)
    {
        if(isPlayer)
        {
            _playerInput.ClearCache();
        }

        switch (currentState)
        {
            case CharacterState.Normal:
                break;

            case CharacterState.Attacking:
                if (_damageCaster != null)
                    _damageCaster.DisableDamageCaster();

                if (isPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }

                break;

            case CharacterState.Dead:
                return;

            case CharacterState.BeingHit:
                break;

            case CharacterState.Slide:
                break;
            case CharacterState.Spawn :
                IsInvincible = false;
                break;
        }

        switch (newState)
        {
            case CharacterState.Normal: 
                break;

            case CharacterState.Attacking:

                if (!isPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(targetTransform.position - transform.position);
                    transform.rotation = newRotation;
                }

                _animator.SetTrigger("Attack");
                if (isPlayer)
                {
                    attackStartTime = Time.time;
                }
                break;

                case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());

                if (!isPlayer)
                {
                    SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                    mesh.gameObject.layer = 0;
                }

                break;

                case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                if (isPlayer)
                {
                    IsInvincible = true;
                    StartCoroutine(DelayCancelInvisible());
                }

                break;

            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;

                case CharacterState.Spawn:
                IsInvincible = true;
                currentSpawnTime = spawnDuration;
                
                StartCoroutine(DissolveAppear());
                break;

        }

        currentState = newState;

       
    }

    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackerPos = new Vector3())
    {

        if (IsInvincible)
        {
            return;
        }

        if (_health != null)
        {

            _health.ApplyDamage(damage);
        }

        if (!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
        }

        StartCoroutine(MaterialBlink());

        if (isPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);

            Debug.Log("Switched to " + currentState);
               AddImpact(attackerPos, 10f);
        }
        else
        {
            AddImpact(attackerPos, 2.5f);
        }


    }

    IEnumerator DelayCancelInvisible()
    {
        yield return new WaitForSeconds(IsInvinsibleDuration);
        IsInvincible = false;
    }


    private void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharcter = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(0.7f);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0f;
        float dissolveHeight_Start = 20f;
        float dissolveHeight_Target = -10f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_Start, dissolveHeight_Target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        ItemDrop();
    }

    public void ItemDrop()
    {
        if (health_Orb != null)
        {
           
            Instantiate(health_Orb, new Vector3( transform.position.x, healthOrbPos.position.y,transform.position.z), Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.type)
        {
            case PickUp.PickUpType.heal:
                AddHealth(item.value);
                break;
            
            case PickUp.PickUpType.coin:
                AddCoin(item.value);
                break;
        }
    }

    private void AddHealth(int health)
    {
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().HealPlayVFX();
    }

    private void AddCoin(int coin)
    {
        Coin += coin;
    }

    public void RotateToTarget()
    {
        if (currentState != CharacterState.Dead)
        {
            transform.LookAt(targetTransform, Vector3.up);
        }
    }

    IEnumerator DissolveAppear()
    {
        float dissolveTimeDuration = spawnDuration;
        float currentDissolveTime = 0;
        float dissolveHeight_start = -10f;
        float dissolveHeight_target = 20f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
