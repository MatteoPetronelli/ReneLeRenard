using TMPro;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;
    public bool _entityWasTouchingGround = false;

    [Header("Jump Buffer")]
    [SerializeField] private float _jumpBufferDuration = 0.2f;
    private float _jumpBufferTimer = 0f;

    [Header("Coyote Time")]
    [SerializeField] private float _coyoteTimeDuration = 0.2f;
    private float _coyoteTimeCountDown = -1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void Start()
    {
        _CancelJumpBuffer();
    }

    private void Update()
    {
        _UpdateJumpBuffer();

        _entity.SetMoveDirX(GetInputMoveX());

        if(_EntityHasExitGround())
        {
            _ResetCoyoteTime();
        }
        else
        {
            _UpdateCoyoteTime();
        }

        if (_entity.IsTouchingWall)
            _entity._WasTouchingWall = true;
        if (_entity._WasTouchingWall && _entity.IsTouchingGround)
        {
            _entity._WasTouchingWall = false;
        }

        if (_GetInputDownJump())
        {
            if (_entity.IsTouchingGround || _IsCoyoteTimeActive() || _entity._indexJumpSetting >= 0)
            {
                _entity.JumpStart();
            }
            else
            {
                _ResetJumpBuffer();
            }
            if (_entity._indexJumpSetting >= 0)
            {
                _entity._indexJumpSetting--;
            }  
        }

        if (IsJumpBufferActive())
        {
            if(_entity.IsTouchingGround || _IsCoyoteTimeActive() && !_entity.IsJumping)
            {
                _entity.JumpStart();
            }
        }

        if (_entity.IsJumpImpulsing)
        {
            if (!_GetInputJump() && _entity.IsJumpingMinDuration)
            {
                _entity.StopJumpImpulsion();
            }
        }
        _entityWasTouchingGround = _entity.IsTouchingGround;
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"Jump Buffer Timer = {_jumpBufferTimer}");
        GUILayout.Label($"CoyoteTime Countdown = {_coyoteTimeCountDown}");
        GUILayout.EndVertical();
    }

    private float GetInputMoveX()
    {
        float inputMoveX = 0f;
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            // To the left <=
            inputMoveX -= 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            // To the right =>
            inputMoveX = 1f;
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            _entity._isDashing = true;
        }

        return inputMoveX;
    }

    private bool _GetInputDownJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool _GetInputJump()
    {
        return Input.GetKey(KeyCode.Space);
    }

    private void _ResetJumpBuffer()
    {
        _jumpBufferTimer = 0f;
    }

    private bool IsJumpBufferActive()
    {
        return _jumpBufferTimer < _jumpBufferDuration;
    }

    private void _UpdateJumpBuffer()
    {
        if(!IsJumpBufferActive()) return;
        _jumpBufferTimer += Time.deltaTime;
    }

    private void _CancelJumpBuffer()
    {
        _jumpBufferTimer = _jumpBufferDuration;
    }

    private void _UpdateCoyoteTime()
    {
        if(!_IsCoyoteTimeActive()) return;
        _coyoteTimeCountDown -= Time.deltaTime;
    }

    private bool _IsCoyoteTimeActive()
    {
        return _coyoteTimeCountDown > 0f;
    }

    private void _ResetCoyoteTime()
    {
        _coyoteTimeCountDown = _coyoteTimeDuration;
    }

    private bool _EntityHasExitGround()
    {
        return _entityWasTouchingGround && !_entity.IsTouchingGround;
    }
}