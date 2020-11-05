using UnityEngine;
using System.Collections;
using Game.Misc;
using Photon.Pun;
using Prime31;


public class PlayerMovement : Movement
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	[SerializeField] private PhotonView photonView;
	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;
	
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

	private bool _canMove = true;
	
	private void Awake()
	{
		_animator = GetComponent<Animator>();
		Controller = GetComponent<EntityMovement>();

		// listen to some events for illustration purposes
		Controller.onControllerCollidedEvent += onControllerCollider;
	}

	private void Start()
	{
		StartCoroutine(CheckFallHp());
	}

	#region Event Listeners

	private void onControllerCollider(RaycastHit2D hit)
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}
	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	private void Update()
	{
		if (Controller.isGrounded)
			_velocity.y = 0;

		Moving();
		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = Controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if( Controller.isGrounded && Input.GetButton("Down"))//Input.GetKey( KeyCode.DownArrow ) )
		{
			_velocity.y *= 3f;
			Controller.ignoreOneWayPlatformsThisFrame = true;
		}
		
		Controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = Controller.velocity;
	}

	private void Moving()
	{
		if (NotMine())
		{
			normalizedHorizontalSpeed = 0;

			if( Controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Idle" ) );
			return;
		}
		if(_canMove && Input.GetButton("Right") )//Input.GetKey( KeyCode.RightArrow ) )
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( Controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else if(_canMove && Input.GetButton("Left"))//Input.GetKey( KeyCode.LeftArrow ) )
		{
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( Controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else
		{
			normalizedHorizontalSpeed = 0;

			if( Controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Idle" ) );
		}
		
		
		// we can only jump whilst grounded
		if( _canMove && Controller.isGrounded && Input.GetButton("Jump")) //Input.GetKeyDown( KeyCode.UpArrow ) )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			_animator.Play( Animator.StringToHash( "Jump" ) );
		}
	}
	private bool NotMine()
	{
		return PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !photonView.IsMine;
	}
	public void SetCanMove(bool value)
	{
		_canMove = value;
	}
}
