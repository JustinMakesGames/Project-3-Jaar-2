using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;

public class PillarPlayerMovement : MinigamePlayerMovement
{

    [Header("CPU Variables")]
    public Transform rightArena;
    public bool isColorRevealed;
    [SerializeField] private Transform normalArena;
    private bool hasReachedArea = true;
    private Vector3 _destination;
    private Vector3 cpuDirection;
    private bool _isIdle;
    private bool _wantsToPush;
    private Transform _targetPlayer;
    
    

    public override void StartMinigame()
    {
        hasMinigameStart = true;

        if (_playerInput.enabled)
        {
            _isPlayer = true;
        }

        else
        {
            StartCoroutine(CPUPush());
        }

        
    }

    
    protected override void FixedUpdate()
    {
        if (hasMinigameStart)
        {
            if (_isPlayer)
            {
                HandlePlayerMovement();
            }

            else
            {
                HandleCPU();
                
                
            }
        }
    }

    private IEnumerator CPUPush()
    {
        while (true)
        {
            StartCoroutine(PlayerPushes());
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    protected override IEnumerator PlayerPushes()
    {
        _isPlayerPushing = true;
        yield return new WaitForSeconds(0.2f);
        _isPlayerPushing = false;
    }
    protected override void GetHit(Transform player)
    {
        if (!_isPlayer)
        {
            _targetPlayer = player;
            StartCoroutine(AttackPlayer());
        }
        
        
        StartCoroutine(HitDelay());
    }

    private IEnumerator AttackPlayer()
    {
        _destination = _targetPlayer.position;
        yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
        StartCoroutine(PlayerPushes());
    }

    protected override IEnumerator HitDelay()
    {
        Color originalColor = GetComponentInChildren<Renderer>().material.color;
        _isHit = true;
        _isStunned = true;
        GetComponentInChildren<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
        _isHit = false;
        yield return StartCoroutine(Flicker());
        _isStunned = false;
    }

    protected override IEnumerator Flicker()
    {
        for (int i = 0; i < flickerAmount; i++)
        {
            GetComponentInChildren<Renderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);
            GetComponentInChildren<Renderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }

        print("Done");
    }
    protected override void HandlePlayerMovement()
    {
        if (!_isHit)
        {
            _rb.velocity = new Vector3(xValue * walkSpeed * Time.deltaTime, _rb.velocity.y, yValue * walkSpeed * Time.deltaTime);
        }
        

        RotationCheck();
    }

    protected override void RotationCheck()
    {
        if (_rb.velocity.x != 0 || _rb.velocity.z != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_rb.velocity.normalized);

            transform.rotation = targetRotation;
        }
    }
   
    public IEnumerator SetColorOn(Transform rightColorPlatform)
    {
        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        hasReachedArea = true;
        isColorRevealed = true;
        rightArena = rightColorPlatform;
        StartCoroutine(HandleColorRevealedMovement());
        
    }

    public void SetColorOff()
    {
        hasReachedArea = true;
        isColorRevealed = false;
        _isIdle = false;
        StopCoroutine(HandleColorRevealedMovement());
    }
    private void HandleCPU()
    {

        if (!isColorRevealed)
        {
            HandleNormalCPUMovement();
        }

        if (_isIdle) return;
        cpuDirection = _destination - _rb.position;
        cpuDirection.Normalize();

        if (!_isHit)
        {
            _rb.velocity = new Vector3(cpuDirection.x * walkSpeed * Time.deltaTime, _rb.velocity.y, cpuDirection.z * walkSpeed * Time.deltaTime);
        }
        
        RotationCheck();
    }

    
    private IEnumerator HandleColorRevealedMovement()
    {
        _destination = CalculateColorDestination();
        hasReachedArea = false;
        while (true)
        {
            if (hasReachedArea)
            {
                _destination = CalculateColorDestination();

                hasReachedArea = false;
            }

            if (Vector3.Distance(transform.position, _destination) < 0.1f)
            {
                _isIdle = true;
                _rb.velocity = Vector3.zero;
                yield return new WaitForSeconds(1);
                _isIdle = false;
                hasReachedArea = true;
            }

            yield return null;
        }
        
    }
    private Vector3 CalculateColorDestination()
    {
        Bounds bounds = rightArena.GetComponentInChildren<Collider>().bounds;
        Vector3 destination = new Vector3(Random.Range(bounds.min.x + 2, bounds.max.x - 2),
            transform.position.y,
            Random.Range(bounds.min.z + 2, bounds.max.z - 2));

        return destination;
    }

    private void HandleNormalCPUMovement()
    {
        if (hasReachedArea)
        {
            _destination = CalculateNormalDestination();
            hasReachedArea = false;
        }

        if (Vector3.Distance(transform.position, _destination) < 0.1f)
        {
            hasReachedArea = true;
        }
    }

    private Vector3 CalculateNormalDestination()
    {
        Bounds bounds = normalArena.GetComponentInChildren<Collider>().bounds;
        Vector3 destination = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z));

        return destination;
    }

    public void CallChangeSpeed()
    {
        StartCoroutine(ChangeSpeed());
    }
    protected override IEnumerator ChangeSpeed()
    {
        walkSpeed = slowSpeed;
        yield return new WaitForSeconds(3);
        walkSpeed = normalSpeed;
    }

  

    protected override void OnTriggerEnter(Collider hitInfo)
    {
        if (!_isPlayerPushing) return;
        if (!hitInfo.transform.CompareTag("Player")) return;
        if (hitInfo.transform.GetComponent<PillarPlayerMovement>().StunCheck()) return;
        print("Hit the player");
        hitInfo.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Vector3 pushDirection = (hitInfo.transform.position - _rb.position).normalized;
        pushDirection.y = hitJumpForce;
        hitInfo.transform.GetComponent<Rigidbody>().velocity = pushDirection * pushForce;

        hitInfo.transform.GetComponent<PillarPlayerMovement>().GetHit(transform);

    }


}
