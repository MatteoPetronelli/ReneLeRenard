using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{

    #region variables
    [Header("Physics")]
    public Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_movementsSettings")]
    [SerializeField] private HeroHorizontalMovementSettings _groundHorizontalMovementsSettings;
    [SerializeField] private HeroHorizontalMovementSettings _airHorizontalMovementsSettings;
    public float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Dash")]
    [SerializeField] private DashSettings _groundDashSettings;
    [SerializeField] private DashSettings _airDashSettings;
    public bool _isDashing;
    private int _dashAllowed = 1;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    public float _orientX = 1f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallSettings;
    private float _verticalSpeed = 0f;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround { get; set; } = false;

    [Header("Wall")]
    [SerializeField] private WallDetector _wallDetector;
    [SerializeField] private HeroHorizontalMovementSettings _wallHorizontalMovementSettings;
    public bool IsTouchingWall { get; private set; } = false;
    public bool _WasTouchingWall = false;

    [Header("Jump")]
    [SerializeField] private HeroJumpSettings[] _AllJumpsSettings = new HeroJumpSettings[3];
    [SerializeField] private HeroFallSettings _jumpFallSettings;
    [SerializeField] private HeroHorizontalMovementSettings _jumpHorizontalMovementSettings;

    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        Falling
    }

    private JumpState _jumpState = JumpState.NotJumping;
    private float _jumpTimer = 0f;
    public bool IsJumping => _jumpState != JumpState.NotJumping;
    public bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;
    public HeroJumpSettings currerntJumpSetting;
    public bool IsJumpingMinDuration => _jumpTimer >= currerntJumpSetting.jumpMinDuration;
    
    public int _indexJumpSetting = 2;

    [Header("Wall Slide")]
    [SerializeField] private HeroFallSettings _WallSlidingFallSettings;
    public bool IsWallSliding => _jumpState == JumpState.Falling && IsTouchingWall && _verticalSpeed < 0 && !IsTouchingGround;

    [Header("Wall Jump")]
    [SerializeField] private HeroJumpSettings _WallJumpSettings;
    public HeroHorizontalMovementSettings _WallJumpHorizontalMovementSettings;
    public float _wallJumpFallspeed = 1.5f;
    public float _wallJumpDuration = 0.2f;
    public bool IsWallJumping;

    [Header("Platforms")]
    [SerializeField] private HeroJumpSettings _JumpPadSettings;
    public bool IsJumpingWithPad;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    // Camera  Follow
    private CameraFollowable _cameraFollowable;
    #endregion

    #region Awake
    private void Awake()
    {
        _cameraFollowable = GetComponent<CameraFollowable>();
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        _cameraFollowable.FollowPositionY = _rigidbody.position.y;
    } 
    #endregion

    #region MoveDirX
    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    } 
    #endregion

    #region FixedUpdate
    private void FixedUpdate()
    {
        _ApplyWallDetection();
        _ApplyGroundDetection();
        _UpdateCameraFollowPosition();
        if (IsTouchingGround || IsTouchingWall && _dashAllowed == 0)
        {
            _dashAllowed = 1;
        }
        DashSettings _dashSettings = _GetCurrentDashSettings();
        if (_isDashing)
            _dashSettings.Duration -= Time.fixedDeltaTime;

        HeroHorizontalMovementSettings horizontalMovementSettings = _GetCurrentHorizontalMovementSettings();

        if (_moveDirX == 0 && IsTouchingWall)
            _horizontalSpeed = 0;

        if (_AreOrientAndMovementOpposite() && !_isDashing)
        {
            _TurnBack(horizontalMovementSettings);
        }
        else
        {
            _UpdateHorizontalSpeed(horizontalMovementSettings, _dashSettings);
            _ChangeOrientFromOrizontalMovement();
        }

        if (IsTouchingWall || IsTouchingWall)
        {
            IsWallJumping = false;
            _wallJumpDuration = 0.2f;
        }


        if (IsWallSliding && _indexJumpSetting >= 0)
        {
            IsWallJumping = true;
            if (_moveDirX == -1 && _orientX == 1)
            {
                _rigidbody.AddForce(new Vector2(-_WallJumpHorizontalMovementSettings.speedMax, _WallJumpSettings.jumpSpeed));
                _orientX = 1;
            } 
            if (_moveDirX == 1 && _orientX == -1)
            {
                _rigidbody.AddForce(new Vector2(_WallJumpHorizontalMovementSettings.speedMax, _WallJumpSettings.jumpSpeed));
                _orientX = -1;
            }
        }
        else if (_WasTouchingWall && !IsTouchingWall)
            WallJump();
        else if (IsJumping && _indexJumpSetting >= 0 && !IsTouchingWall)
        {
            currerntJumpSetting = _GetHeroJumpSettings(_indexJumpSetting);
            _UpdateJump(currerntJumpSetting);
        }
        else
        {
            if (!IsTouchingGround)
            {
                _ApplyFallGravity(_fallSettings);
            }
            else
            {
                _ResetVerticalSpeed();
            }
        }

        if (IsWallSliding)
            _verticalSpeed = -_WallSlidingFallSettings.fallSpeedMax;

        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
    } 
    #endregion

    #region HorizontalMovement
    private void _ChangeOrientFromOrizontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
    }

    private void _Accelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed = settings.acceleration + Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax)
        {
            _horizontalSpeed = settings.speedMax;
        }
    }

    private void _Decelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    private void _ApplyHorizontalSpeed()
    {
        if (IsWallSliding) _horizontalSpeed = 0f;
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }

    private void _UpdateHorizontalSpeed(HeroHorizontalMovementSettings settings, DashSettings dashSettings)
    {
        if (_isDashing)
        {
            _Dash(settings, dashSettings);
        }
        else if (_moveDirX != 0f)
        {
            _Accelerate(settings);
        }
        else
        {
            _Decelerate(settings);
        }
    }

    private void _TurnBack(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFriction * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromOrizontalMovement();
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    } 
    

    private void _Dash(HeroHorizontalMovementSettings settings, DashSettings _dashSettings)
    {
        if(IsTouchingWall) _horizontalSpeed = 0f;
        else if (_dashAllowed > 0)
        {
            _jumpState = JumpState.Falling;
            _verticalSpeed = 0f;
            _horizontalSpeed = _dashSettings.Speed;
        }
        if (_dashSettings.Duration < 0)
        {
            _isDashing = false;
            _dashSettings.Duration = 0.1f;
            _horizontalSpeed = settings.speedMax;
        }
        else if(_dashSettings == _airDashSettings && _dashAllowed == 1)
        {
            _dashAllowed -= 1;
        }
    }
    #endregion

    #region Gravity
    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        if (!IsWallSliding)
            _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
    }

    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }
    #endregion

    #region Wall
    private void _ApplyWallDetection()
    {
        IsTouchingWall = _wallDetector.DetectWallNearBy();
    }

    private void WallJump()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < _wallJumpDuration)
            _verticalSpeed = _WallJumpSettings.jumpSpeed;
        else
        {
            StopJumpImpulsion();
            _verticalSpeed -= _wallJumpFallspeed + Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Jump
    public void JumpStart()
    {
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }

    private void _UpdateJumpStateImpulsion(HeroJumpSettings jump)
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < jump.jumpMaxDuration)
        {
            _verticalSpeed = jump.jumpSpeed;
        }
        else
        {
            StopJumpImpulsion();
        }
    }

    private void _UpdateJumpStateFalling()
    {
        if (IsTouchingGround)
        {
            _ResetVerticalSpeed();
            _jumpState = JumpState.NotJumping;
        }
        else
        {
            _ApplyFallGravity(_jumpFallSettings);
        }
    }

    public void _UpdateJump(HeroJumpSettings jump)
    {
        switch (_jumpState)
        {
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion(jump);
                break;

            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
        }
    }

    public void StopJumpImpulsion()
    {
        _jumpState = JumpState.Falling;
    }

    private void _ResetJumps()
    {
        
        if (_indexJumpSetting < 0)
            StopJumpImpulsion();

        if (IsTouchingGround)
            _indexJumpSetting = 2;
        if (IsWallSliding)
        {
            _indexJumpSetting = 0;
        }
        if ((IsTouchingWall && !IsWallSliding && (!Input.GetKey(KeyCode.Q)) && !Input.GetKey(KeyCode.D)))
        {
            _verticalSpeed = -_WallSlidingFallSettings.fallSpeedMax;
            _indexJumpSetting = -1;
        }
    }

    #endregion

    #region GetSettings
    private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettings()
    {
        if (IsJumping || IsTouchingWall)
        {
            return IsTouchingWall && IsWallSliding ? _wallHorizontalMovementSettings : _jumpHorizontalMovementSettings;
        }
        return IsTouchingGround ? _groundHorizontalMovementsSettings : _airHorizontalMovementsSettings;
    }

    private DashSettings _GetCurrentDashSettings()
    {
        return IsTouchingGround ? _groundDashSettings : _airDashSettings;
    }

    private HeroJumpSettings _GetHeroJumpSettings(int i)
    {
        if (IsTouchingWall || _WasTouchingWall) return _WallJumpSettings;
        if (IsJumpingWithPad) return _JumpPadSettings;
        return _AllJumpsSettings[i];
    }
    #endregion

    #region Update
    private void Update()
    {
        _UpdateOrientVisual();
        _ResetJumps();
    } 
    #endregion

    #region UpdateOrientVisual
    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }
    #endregion

    #region GUI
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if (IsTouchingGround)
        {
            GUILayout.Label("OnGround");
        }
        else
        {
            GUILayout.Label("InAir");
        }
        GUILayout.Label($"{_jumpState}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.Label($"Is touching wall = {IsTouchingWall}");
        GUILayout.Label($"Is wall sliding = {IsWallSliding}");
        GUILayout.Label($"Was touching wall = {_WasTouchingWall}");
        GUILayout.Label($"Index Jump Settings = {_indexJumpSetting}");
        GUILayout.EndVertical();
    }
    #endregion

    #region Camera
    private void _UpdateCameraFollowPosition()
    {
        _cameraFollowable.FollowPositionX = _rigidbody.position.x + (CameraManager.Instance._currentCameraProfile._followOffsetX * _orientX);
        if (IsTouchingGround && !IsJumping)
        {
            _cameraFollowable.FollowPositionY = _rigidbody.position.y;
        }
    } 
    #endregion
}