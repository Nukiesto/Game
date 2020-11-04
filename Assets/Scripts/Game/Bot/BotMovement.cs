using System;
using UnityEngine;
using System.Collections;
using Game.ChunkSystem;
using Game.Misc;
using Game.Player;
using Prime31;
using static UnityEngine.Physics2D;
using Random = UnityEngine.Random;
using static UsefulScripts.RandomScripts;

public class BotMovement : Movement
{
	public enum DirMove
	{
		Left,
		Idle,
		Right
	}
	#region Init

	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	[HideInInspector] private float normalizedHorizontalSpeed = 0;

	
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	
	private LayerMask _entityMask;
	private LayerMask _platformMask;
	#endregion

	[Header("PlayerMoving")] 
	[SerializeField] private float viewDistance = 12;
	[SerializeField] private bool mustMovingToPlayer;
	[SerializeField] private float stopDistanceFindedPlayer = 1f;
	[SerializeField] private bool findPlayerOverSolid = false;
	[SerializeField] private bool canBreackBlock = false;
	
	private bool _isGoingRight;
	private bool _playerDir;
	private bool _isMovingToPlayer;
	private bool _canToInvoke = true;
	private bool _randomMoving = true;
	private bool _toJumpCliff;
	private bool _stopFindedPlayer;
	private void Awake()
	{
		_animator = GetComponent<Animator>();
		Controller = GetComponent<EntityMovement>();
		_platformMask = Controller.platformMask;
		_entityMask = Controller.entityMask;
		
		// listen to some events for illustration purposes
		Controller.onControllerCollidedEvent += OnControllerCollider;
	}

	private void OnEnable()
	{
		//Debug.Log("Enabled");
		_isGoingRight = RandomBool();
		StartCoroutine(RanDomMoving());
		StartCoroutine(CheckFallHp());
		if (canBreackBlock)
			StartCoroutine(RandomBreakBlock());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void OnValidate()
	{
		if (gameObject.activeInHierarchy)
		{
			StopCoroutine(RandomBreakBlock());
			if (canBreackBlock)
				StartCoroutine(RandomBreakBlock());
		}
	}

	#region Event Listeners

	private void OnControllerCollider(RaycastHit2D hit)
	{
		if (hit.normal.y == 1f)
			return;
	}

	#endregion
	
	private void Update()
	{
		if (Controller.isGrounded)
			_velocity.y = 0;

		Movement();

		EndUpdate();
	}

	#region Internal
	#endregion
	
	private bool CheckSolidForDir(bool dir)
	{
		//Проверка коллизии на пути движения
		queriesStartInColliders = false;
		var direction = new Vector2(dir ? 0.2f : -0.2f, 0);
		var pos = dir ? Controller._raycastOrigins.bottomRight : Controller._raycastOrigins.bottomLeft;
		Debug.DrawRay(pos, new Vector3(direction.x, direction.y, 0), Color.blue);
		return Raycast( pos, direction, 0.2f, _platformMask);
	}

	private bool CheckForCleft(bool dir, bool check1)
	{
		queriesStartInColliders = false;
		var pos0 = transform.position;
		var pos = new Vector2(pos0.x, pos0.y);
		
		var ray1 = true;
		var ray2 = CheckRay(pos, dir, 2, -1);
		//Debug.Log(check1);
		if (check1)
		{
			ray1 = CheckRay(pos, dir, 1, -1);
		}

		return !ray1 || !ray2;
	}
	private bool CheckForJumpSolid(bool dir, bool check2 = true)
	{
		queriesStartInColliders = true;
		var pos0 = transform.position;
		var pos = new Vector2(pos0.x, pos0.y) {y = pos0.y + 0.8f};

		var ray1 = CheckRay(pos, dir, 1);
		var ray2 = true;
		if (check2)
		{
			pos.y = pos0.y + 1.8f;
			ray2 = CheckRay(pos, dir, 1);
		}

		return !ray1 || !ray2;
	}

	private bool CheckRay(Vector2 pos, bool dir, float distance, float yy = 0)
	{
		var posCheck = Vector2.zero;
		posCheck.x = pos.x + (dir ? distance : -distance);
		posCheck.y = pos.y + yy;
		var distance1 = Vector2.Distance(pos, posCheck);
		var direction = posCheck - pos;
		Debug.DrawRay(pos, direction, Color.blue);
		var ray = Raycast(pos, direction, distance1, _platformMask);
		if (ray)
		{
			Debug.DrawRay(pos, direction, Color.white);
		}

		return ray;
	}
	private bool CheckPlayerForView(float viewDistance)
	{
		queriesStartInColliders = false;
		var pos = transform.position;
		var pos2D = new Vector2(pos.x, pos.y);
		
		var posPlayer = PlayerController.Instance.transform.position;
		var posPlayer2D = new Vector2(posPlayer.x, posPlayer.y);
		var playerDistance = Vector2.Distance(pos2D, posPlayer2D);
		var direction = posPlayer2D - pos2D;
		_toJumpCliff = playerDistance >= stopDistanceFindedPlayer;
		_stopFindedPlayer = playerDistance <= stopDistanceFindedPlayer;
		if (playerDistance <= viewDistance && playerDistance >= stopDistanceFindedPlayer)
		{
			var solid = Raycast( pos, direction, playerDistance,_platformMask);
			Debug.DrawRay(pos, direction, Color.blue);
			if (!solid || findPlayerOverSolid)
			{
				Debug.DrawRay(pos, direction, Color.white);
				_playerDir = direction.x > 0;
				return true;
			}
		}
		
		return false;
	}
	private RaycastHit2D GetHitForDir(bool dir, float distance, LayerMask mask)
	{
		queriesStartInColliders = false;
		var direction = new Vector2(dir ? 1 : -1, 0);
		var pos = transform.position;
		
		return Raycast( pos, direction, distance, mask);
	}
	private void EndUpdate()
	{
		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor =
			Controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed,
			Time.deltaTime * smoothedMovementFactor);

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;
		
		Controller.move(_velocity * Time.deltaTime);

		// grab our current _velocity to use as a base for all calculations
		_velocity = Controller.velocity;
	}

	private void Movement()
	{
		if (CheckPlayerForView(viewDistance))
		{
			_isMovingToPlayer = true;
			//StopCoroutine(TimerToStopMovingToPlayer());
			if (_canToInvoke)
			{
				_canToInvoke = false;
				StartCoroutine(ContinueMovingToPlayer());
			}
		}
		if ((_isMovingToPlayer || _stopFindedPlayer) && mustMovingToPlayer )
			ToPlayerMovement();
		else if (_randomMoving)
		{
			RandomMovement();
		}
		else
			MoveNone();
	}

	private IEnumerator ContinueMovingToPlayer()
	{
		while(true)
		{
			yield return new WaitForSeconds(4f);
			//Debug.Log("End");
			_isMovingToPlayer = CheckPlayerForView(viewDistance);
			_canToInvoke = true;
			yield break;
		}
	}

	private IEnumerator RanDomMoving()
	{
		while (true)
		{
			yield return new WaitForSeconds(_randomMoving ? Random.Range(35f, 55f) : Random.Range(4f, 14f));
			_randomMoving = !_randomMoving;
		}
	}
	private IEnumerator RandomBreakBlock()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(0f, 2f));
			BreakBlock();
		}
	}
	private void ToPlayerMovement()
	{
		if (!IsFall())
		{
			if (_stopFindedPlayer)
			{
				MoveNone();
			}
			else
			{
				if (CheckSolidForDir(_playerDir) && CheckForJumpSolid(_playerDir)) // && GetBoolChance(50)) // ChangeDirection
				{
					Jump();
				}
				if (_toJumpCliff && CheckForCleft(_playerDir, true) && Controller.isGrounded)
				{
					Jump();
				}
				MoveToDir(_playerDir);
			}
		}
	}
	private void RandomMovement()
	{
		if (!IsFall())
		{
			if (CheckSolidForDir(_isGoingRight) && CheckForJumpSolid(_isGoingRight)) // && CheckForJumpSolid(_isGoingRight)) // ChangeDirection
			{
				Jump(); 
				return;
			}
			//else
			//{
				//_isGoingRight = !_isGoingRight;
			//}

			//if (!CheckForJumpSolid(_isGoingRight))
			//{
			//	_isGoingRight = !_isGoingRight;
			//}

			if (CheckSolidForDir(_isGoingRight) && Controller.isGrounded)
			{
				_isGoingRight = !_isGoingRight;
			}

			//if (GetBoolChance(850))
			//{
			//	_isGoingRight = !_isGoingRight;
			//}

			if (CheckForCleft(_isGoingRight, true) && Controller.isGrounded)
			{ 
				if (RandomBoolChance(250))
					Jump();
				else
				{
					if (RandomBoolChance(150))
						_isGoingRight = !_isGoingRight;
				}
					
			}
			
			MoveToDir(_isGoingRight);
		}
		
	}

	private void BreakBlock()
	{
		var pos = transform.position;
		var layer = BlockLayer.Front;
		if (RandomBool())
		{
			pos.x += _isGoingRight ? 1 : -1;
		}
		else
		{
			pos.y--;
		}
		
		var chunkUnitClick = ChunkManager.Instance.GetChunk(pos);
		
		if (chunkUnitClick && chunkUnitClick.CanBreakBlock(pos, layer))
		{
			chunkUnitClick.DeleteBlock(pos, layer);
		}
	}
	private bool IsFall()
	{
		queriesStartInColliders = false;
		var direction = new Vector2(0, -1);
		var pos = transform.position;
		
		return !Raycast( pos, direction, 5, _platformMask);
	}
	private void MoveToDir(bool isRightMove)
	{
		if (isRightMove)
		{
			MoveRight();
		}
		else
		{
			MoveLeft();
		}
	}
	private void MoveLeft()
	{
		normalizedHorizontalSpeed = -1;
		if (transform.localScale.x > 0f)
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

		if (Controller.isGrounded)
			_animator.Play(Animator.StringToHash("Run"));
	}
	private void MoveRight()
	{
		normalizedHorizontalSpeed = 1;
		if (transform.localScale.x < 0f)
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

		if (Controller.isGrounded)
			_animator.Play(Animator.StringToHash("Run"));
	}
	private void MoveNone()
	{
		normalizedHorizontalSpeed = 0;

		if (Controller.isGrounded)
			_animator.Play(Animator.StringToHash("Idle"));
	}
	private void Jump()
	{
		if (Controller.isGrounded)
		{
			_velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
			_animator.Play(Animator.StringToHash("Jump"));
		}
	}
}
