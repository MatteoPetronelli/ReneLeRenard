using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.UI.Selectable;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Camera")]
    [SerializeField] private Camera _camera;

    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;

    [Header("Profile System")]
    [SerializeField] private CameraProfile _defaultCameraProfile;
    public CameraProfile _currentCameraProfile;

    //Transition
    private float _profileTransitionTimer = 0f;
    private float _profileTransitionDuration = 0f;
    private Vector3 _profileTransitionStartPosition;
    private float _profileTransitionStartSize;

    //Follow
    private Vector3 _profileLastFollowDestination;

    //Damping
    private Vector3 _dampedPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _InitToDefaultProfile();
    }

    private void Update()
    {
        Vector3 nextPosition = _FindCameraNextPosition();
        nextPosition = _ClampPositionIntoBounds(nextPosition);
        nextPosition = _ApplyDamping(nextPosition);

        if(_IsPlayingProfileTransition())
        {
            _profileTransitionTimer += Time.deltaTime;
            Vector3 transitionPosition = _CalculateProfileTransitionPosition(nextPosition);
            _SetCameraPosition(transitionPosition);
            float transitionSize = _CalculateProfileTransitionCameraSize(_currentCameraProfile.CameraSize);
            _SetCameraSize(transitionSize);
        }
        else
        {
            _SetCameraPosition(nextPosition);
            _SetCameraSize(_currentCameraProfile.CameraSize);
        }
    }

    private void _SetCameraPosition(Vector3 position)
    {
        Vector3 newCameraPosition = _camera.transform.position;
        newCameraPosition.x = position.x;
        newCameraPosition.y = position.y;
        _camera.transform.position = newCameraPosition;
    }

    private void _SetCameraSize(float size)
    {
        _camera.orthographicSize = size;
    }

    private void _InitToDefaultProfile()
    {

        _currentCameraProfile = _defaultCameraProfile;
        _SetCameraPosition(_currentCameraProfile.Position);
        _SetCameraSize(_currentCameraProfile.CameraSize);
        _SetCameraDampingPosition(_ClampPositionIntoBounds(_FindCameraNextPosition()));

    }

    public void EnterProfile(CameraProfile cameraProfile, CameraProfileTransition transition = null)
    {
        _currentCameraProfile = cameraProfile;
        if(transition != null)
        {
            _PlayProfileTrransition(transition);
        }
        _SetCameraDampingPosition(_FindCameraNextPosition());
    }

    public void ExitProfile(CameraProfile cameraProfile, CameraProfileTransition transition = null)
    {
        if(_currentCameraProfile != cameraProfile) return;
        _currentCameraProfile = _defaultCameraProfile;
        if (transition != null)
        {
            _PlayProfileTrransition(transition);
        }
        _SetCameraDampingPosition(_FindCameraNextPosition());
    }

    private void _PlayProfileTrransition(CameraProfileTransition transition)
    {
        _profileTransitionStartPosition = _camera.transform.position;

        _profileTransitionStartSize = _camera.orthographicSize;

        _profileTransitionTimer = 0;
        _profileTransitionDuration = transition.duration;
    }

    private bool _IsPlayingProfileTransition()
    {
        return _profileTransitionTimer < _profileTransitionDuration;
    }

    private float _CalculateProfileTransitionCameraSize(float endSize)
    {
        float percent = _profileTransitionTimer / _profileTransitionDuration;
        float startSize = _profileTransitionStartSize;
        return Mathf.Lerp(startSize, endSize, percent);
    }

    private Vector3 _CalculateProfileTransitionPosition(Vector3 destination)
    {
        float percent = _profileTransitionTimer / _profileTransitionDuration;
        Vector3 origin = _profileTransitionStartPosition;
        return Vector3.Lerp(origin, destination, percent);
    }

    private Vector3 _FindCameraNextPosition()
    {
        if (_currentCameraProfile.ProfileType == CameraProfile.CameraProfileType.FollowTarget)
        {
            if(_currentCameraProfile.TargetToFollow != null)
            {
                CameraFollowable targetToFollow = _currentCameraProfile.TargetToFollow;
                _profileLastFollowDestination.x = targetToFollow.FollowPositionX;
                _profileLastFollowDestination.y = targetToFollow.FollowPositionY;
                return _profileLastFollowDestination;
            }
        }

        if (_currentCameraProfile.ProfileType == CameraProfile.CameraProfileType.AutoScroll)
        {
            if (_currentCameraProfile._horizontalAutoScroll > 0)
            {
                _currentCameraProfile.transform.position += new Vector3(_currentCameraProfile._horizontalAutoScroll * Time.deltaTime, 0, 0);
                return _currentCameraProfile.transform.position;
            }

            else if (_currentCameraProfile._verticalAutoScroll > 0)
            {
                _currentCameraProfile.transform.position += new Vector3(0, _currentCameraProfile._verticalAutoScroll * Time.deltaTime, 0);
                return _currentCameraProfile.Position;
            }
        }

        return _currentCameraProfile.Position;
    }

    private Vector3 _ApplyDamping(Vector3 position)
    {
        if(_currentCameraProfile.UseDampingHorizontally)
        {
            _dampedPosition.x = Mathf.Lerp(
                    _dampedPosition.x,
                    position.x,
                    _currentCameraProfile.HorizontalDampingFactor /_currentCameraProfile._folloxOffsetDamping * Time.deltaTime
                );
        } else
        {
            _dampedPosition.x = position.x;
        }

        if(_currentCameraProfile.UseDampingVertically)
        {
            _dampedPosition.y = Mathf.Lerp(
                    _dampedPosition.y,
                    position.y,
                    _currentCameraProfile.VerticalDampingFactor * Time.deltaTime
                );
        } else
        {
            _dampedPosition.y = position.y;
        }

        return _dampedPosition;
    }

    private void _SetCameraDampingPosition(Vector3 position)
    {
        _dampedPosition.x = position.x;
        _dampedPosition.y = position.y;
    }

    private Vector3 _ClampPositionIntoBounds(Vector3 position)
    {
        if(!_currentCameraProfile.HasBounds) return position;

        Rect boundRect = _currentCameraProfile.BoundRect;
        Vector3 worldBottomLeft = _camera.ScreenToWorldPoint(new Vector3(0f, 0f));
        Vector3 worldTopRight = _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight));
        Vector2 worldScreedSize = new Vector2(worldTopRight.x - worldBottomLeft.x, worldTopRight.y  - worldBottomLeft.y);
        Vector2 worldHalfScreenSize = worldScreedSize / 2f;

        if (position.x > boundRect.xMax - worldHalfScreenSize.x)
        {
            position.x = boundRect.xMax - worldHalfScreenSize.x;
        }

        if (position.x < boundRect.xMin + worldHalfScreenSize.x)
        {
            position.x = boundRect.xMin + worldHalfScreenSize.x;
        }

        if (position.y > boundRect.yMax - worldHalfScreenSize.y)
        {
            position.y = boundRect.yMax - worldHalfScreenSize.y;
        }

        if (position.y < boundRect.yMin + worldHalfScreenSize.y)
        {
            position.y = boundRect.yMin + worldHalfScreenSize.y;
        }

        return position;
    }
}