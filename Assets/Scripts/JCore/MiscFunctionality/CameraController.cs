using UnityEngine;

public class CameraController : MonoBehaviour 
{
    //TODO: maybe also rotate to face the same direction at the target's direction (with a bool _copyTargetForwardRotation)
    //TODO: we might also want a collider and if it hits anything then we move out of its way.
    
    // Direction to look in
    private enum DIRECTION {FORWARD, RIGHT, UP, BACKWARD, LEFT, DOWN };
    [SerializeField, Tooltip("To change the axis around which the camera will rotate")]
    private DIRECTION _direction = DIRECTION.FORWARD;
    private Vector3 _forwardDir;
    private Vector3 _upDir;
    private Vector3 _rightDir;
    
    // Targeting
    [Header("Targeting")]
	[SerializeField, Tooltip("A transform to orbit around and follow\nCan be null, if so it will target the last position of its last target, you can use setTargetPosition to target any position when your target it null.")]
    private Transform _target;
    [SerializeField]
    private Vector3 _targetOffset;
    
    private Vector3 _targetPos = Vector3.zero;

    // Orbiting
    [Header("Orbiting")]
    [SerializeField]
    private Vector2MinMaxSet _rotationSpeed = new Vector2MinMaxSet(new Vector2(100.0f, 100.0f), new Vector2(200.0f, 200.0f)); 
    [SerializeField, Tooltip("This is between the rotation limits so you'll probably want to consider using ^ or V shaped curves so that the rotation speed is faster or slower in the middle respectively.")]
    private AnimationCurve2 _rotationSpeedEasing = default;
    [SerializeField]
    private Vector2MinMaxSet _rotationLimits = new Vector2MinMaxSet(new Vector2(100.0f, 100.0f), new Vector2(200.0f, 200.0f));
    [SerializeField]
    private float _rotationDampening = 5.0f;
    //if there is no rotation input return to the return rotation, make sure the values are within the _rotationLimits.
    [SerializeField, Tooltip("If there is no rotation input return to this rotation, will still be within the Rotation Limits.")]
    private Vector2 _returnRotation = new Vector2(0, 0);
    [SerializeField]
    private Vector2MinMaxSet _returnRotationSpeed = new Vector2MinMaxSet(new Vector2(5.0f, 10.0f), new Vector2(20.0f, 40.0f)); 
    [SerializeField, Tooltip("Distance from the return rotation angle at which to consider the return rotation speed at max, always positive")]
    private Vector2 _returnRotSpeedDist = new Vector2(60, 60);
    [SerializeField]
    private AnimationCurve2 _returnRotationSpeedEasing = default;

    private Vector2 _currRotation;
    private Vector2 _normalizedRotation = Vector2.zero;
    private Quaternion _desiredRotation;
    
    // Zooming
    [Header("Zooming")]
	[SerializeField]
    private MinMaxFloat _zoomSpeed = new MinMaxFloat(20.0f, 50.0f);
    [SerializeField]
    private AnimationCurve _zoomSpeedEasing = default;
    [SerializeField]
	private MinMaxFloat _distanceLimits = new MinMaxFloat(1.0f, 20.0f);
    [SerializeField]
    private float _zoomDampening = 5.0f;
    
    private float _currDistance;
    private float _desiredDistance;
    private float _targetDistN;

    // Input
    private struct InputState
    {
        public float zoom;
        public bool rotationEnabled;
        public Vector2 rotation;

        public InputState(float zoom, bool rotationEnabled, Vector2 rotation)
        {
            this.zoom = zoom;
            this.rotationEnabled = rotationEnabled;
            this.rotation = rotation;
        }
    }
    private InputState _inputState = new InputState(0, false, new Vector2(0,0));

    // ===================
    //        INIT
    // ===================
    void Start() 
	{
        if (isActiveAndEnabled)
        {
		    Init();
        }
	}

    void OnEnable() 
	{
		Init();
	}

    public void Init()
    {
        //set the current distance and rotation so we keep our initial position
        setDistanceToTarget();
        
        //figure out the direction to aim in.
        switch( _direction )
        {
            default:
                _forwardDir = Vector3.forward;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.up;
                
                _currRotation.x = AngleUtil.Normalize(transform.eulerAngles.y);
                _currRotation.y = AngleUtil.Normalize(transform.eulerAngles.x);
                break;
            case DIRECTION.RIGHT:
                _forwardDir = Vector3.right;
                _upDir      = Vector3.forward;
                _rightDir   = Vector3.up;

                _currRotation.x = AngleUtil.Normalize(transform.eulerAngles.y);
                _currRotation.y = AngleUtil.Normalize(transform.eulerAngles.z);
                break;
            case DIRECTION.UP:
                _forwardDir = Vector3.up;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.forward;

                _currRotation.x = AngleUtil.Normalize(transform.eulerAngles.z);
                _currRotation.y = AngleUtil.Normalize(transform.eulerAngles.x);
                break;
            case DIRECTION.BACKWARD:
                _forwardDir = -Vector3.forward;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.up;

                _currRotation.x = AngleUtil.Normalize(transform.eulerAngles.y);
                _currRotation.y = AngleUtil.Normalize(transform.eulerAngles.x);
                break;
            case DIRECTION.LEFT:
                _forwardDir = -Vector3.right;
                _upDir      = Vector3.forward;
                _rightDir   = Vector3.up;

                _currRotation.x = AngleUtil.Normalize(transform.eulerAngles.y);
                _currRotation.y = AngleUtil.Normalize(transform.eulerAngles.z);
                break;
            case DIRECTION.DOWN:
                _forwardDir = -Vector3.up;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.forward;

                _currRotation.x = AngleUtil.Normalize(transform.eulerAngles.z);
                _currRotation.y = AngleUtil.Normalize(transform.eulerAngles.x);
                break;
        }

    }

    // ===================
    //     SET TARGET
    // ===================
    public void setTarget(Transform target = null, bool teleportToPosition = false)
    {
        if (target == _target)
        {
            return;
        }

        _target = target;
        _targetOffset.x = _targetOffset.y = _targetOffset.z = 0;
        
        if (teleportToPosition)
        {
            setDistanceToTarget();
        }
    }

    public void setTarget(Transform target, Vector3 offset, bool teleportToPosition = false)
    {
        setTarget(target, teleportToPosition);
        _targetOffset = offset;
    }

    public void setTargetOffset(Vector3 offset)
    {
        _targetOffset = offset;
    }

    public void setTargetPosition(Vector3 postion)
    {
        if (_target != null)
        {
            Debug.LogError
            (
                "CameraController: setTargetPosition: Camera already has a target transform ("
                +_target 
                +") so you can't change the target position yet, call setTarget(null) first."
            );
        }
        else
        {
            _targetPos = postion;
        }
    }

    // Summary:
    //     Instantly sets the distance to the target, skipping all the lerping to smoothly zoom to the target.
    private void setDistanceToTarget()
    {
        updateTargetPos();
        
        float initDist = Vector3.Distance(_targetPos, transform.position);
        initDist = _distanceLimits.ClampValue(initDist);

        _currDistance = _desiredDistance = initDist;
    }

    // ===================
    //      UPDATING
    // ===================
    void LateUpdate()
    {
        updateInputState();

        updateTargetPos();

        updateRotation();
        updatePosition();
	}

    // --- Input ----
    void updateInputState()
    {
        _inputState.zoom = -Input.GetAxis("Mouse ScrollWheel");

        _inputState.rotationEnabled = Input.GetMouseButton(0);

        _inputState.rotation.x = Input.GetAxis("Mouse X");
        _inputState.rotation.y = -Input.GetAxis("Mouse Y");
    }

    // --- Target position ----
    void updateTargetPos()
    {
        if (_target != null)
            _targetPos = _target.transform.position;
    }

    // ---- Rotation ----
    void updateRotation()
    {
        if (_inputState.rotationEnabled)
        {
            _normalizedRotation = _rotationLimits.InverseLerp(_currRotation);
            _normalizedRotation = _rotationSpeedEasing.Evaluate2(_normalizedRotation);

            _currRotation += _inputState.rotation * _rotationSpeed.Lerp(_normalizedRotation) * Time.deltaTime;
        }
        else
        {
            // return rotation
            if (_currRotation != _returnRotation)
            {
                if ( (_returnRotationSpeed.min.x > 0 && _returnRotationSpeed.max.x > 0) || (_returnRotationSpeed.min.y > 0 && _returnRotationSpeed.max.y > 0) )
                {
                    //how far we are away from the return rotation
                    _normalizedRotation -= _returnRotation;
                    _normalizedRotation.x = Mathf.Abs(AngleUtil.ClampAngle(_normalizedRotation.x, -_returnRotSpeedDist.x, _returnRotSpeedDist.x) / _returnRotSpeedDist.x);
                    _normalizedRotation.y = Mathf.Abs(AngleUtil.ClampAngle(_normalizedRotation.y, -_returnRotSpeedDist.y, _returnRotSpeedDist.y) / _returnRotSpeedDist.y);

                    _normalizedRotation = _returnRotationSpeedEasing.Evaluate2(_normalizedRotation);

                    // now modify the current rotation to move towards the return rotation
                    if ( _returnRotationSpeed.min.x > 0 && _returnRotationSpeed.max.x > 0 )
                    {
                        _currRotation.x = Mathf.Lerp(_currRotation.x, _returnRotation.x, Time.deltaTime * _returnRotationSpeed.getLerpedX(_normalizedRotation.x));
                    }

                    if ( _returnRotationSpeed.min.y > 0 && _returnRotationSpeed.max.y > 0 )
                    {
                        _currRotation.y = Mathf.Lerp(_currRotation.y, _returnRotation.y, Time.deltaTime * _returnRotationSpeed.getLerpedY(_normalizedRotation.y));
                    }
                }
            }
        }

        // clamp the rotation angles
        _currRotation.x = AngleUtil.ClampAngle(_currRotation.x, _rotationLimits.min.x, _rotationLimits.max.x);
        _currRotation.y = AngleUtil.ClampAngle(_currRotation.y, _rotationLimits.min.y, _rotationLimits.max.y);
        
        // set camera rotation, smoothed
        _desiredRotation = Quaternion.Euler((_currRotation.y * _upDir) + (_currRotation.x * _rightDir));

        transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, Time.deltaTime * _rotationDampening);
    }

    // ---- Position ----
    void updatePosition()
    {
        // set zoom amount, smoothed
        // get the normalized distance between the zoom limits, then ease it.
        _targetDistN = _distanceLimits.InverseLerp(Vector3.Distance(_targetPos, transform.position));
        if (_zoomSpeedEasing.keys.Length > 0)
        {
            _targetDistN = _zoomSpeedEasing.Evaluate(_targetDistN);
        }

        _desiredDistance += _inputState.zoom * Time.deltaTime * _zoomSpeed.Lerp(_targetDistN);
        _desiredDistance = _distanceLimits.ClampValue(_desiredDistance);
        _currDistance = Mathf.Lerp(_currDistance, _desiredDistance, Time.deltaTime * _zoomDampening);

        // calculate position based on the new distance
        transform.position = _targetPos - (transform.forward * _currDistance) + _targetOffset;
    }
}