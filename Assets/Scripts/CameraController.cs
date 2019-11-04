using UnityEngine;

public class CameraController : MonoBehaviour 
{
    //TODO: some easing curves for rotation and zooming.
    //TODO: option to change which axis we want to consider the forward axis (say we wanted it to be y rather than z)
    
    //TODO: maybe also rotate to face the same direction at the target's direction (with a bool _copyTargetForwardRotation)
    //TODO: we might also want a collider and if it hits anything then we move out of its way.
    
    // Direction to look in
    private enum DIRECTION {FORWARD, RIGHT, UP, BACKWARD, LEFT, DOWN };
    [SerializeField]
    private DIRECTION _direction = DIRECTION.FORWARD;
    private Vector3 _forwardDir;
    private Vector3 _upDir;
    private Vector3 _rightDir;
    
    // Targeting
    [Header("Targeting")]
	[SerializeField]
    private Transform _target;
    [SerializeField]
    private Vector3 _targetOffset;
    
    private Vector3 _targetPos = Vector3.zero;

    // Orbiting
    [Header("Orbiting")]
    [SerializeField]
    private Vector2 _rotationSpeed = new Vector2(200.0f, 200.0f);
    [SerializeField]
	private MinMaxFloat _horizontalRotationLimits = new MinMaxFloat(-80, 80);
    [SerializeField]
	private MinMaxFloat _verticalRotationLimits = new MinMaxFloat(-80, 80);
    [SerializeField]
    private float _rotationDampening = 5.0f;
    //if there is no rotation input return to the return rotation.
    //make sure the x and y values are within the _horizontalRotationLimits & _verticalRotationLimits respectively.
    [SerializeField]
    private Vector2 _returnRotation = new Vector2(0, 0);
    //make sure the values are lower than rotation speed.
    [SerializeField]
    private Vector2 _returnRotationSpeed = new Vector2(20.0f, 10.0f);

    private Vector2 _currRotation;
    private Quaternion _desiredRotation;
    
    // Zooming
    [Header("Zooming")]
	[SerializeField]
    private float _zoomSpeed = 40.0f;
    [SerializeField]
	private MinMaxFloat _distanceLimits = new MinMaxFloat(1.0f, 20.0f);
    [SerializeField]
    private float _zoomDampening = 5.0f;
    
    private float _currDistance;
    private float _desiredDistance;

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
		Init();
	}

    void OnEnable() 
	{
		Init();
	}

    public void Init()
    {
        //figure out the direction to aim in.
        switch( _direction )
        {
            default:
                _forwardDir = Vector3.forward;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.up;
                break;
            //TODO: setup and test these other directions
            case DIRECTION.RIGHT:
                _forwardDir = Vector3.right;
                _upDir      = Vector3.forward;
                _rightDir   = Vector3.up;
                break;
            case DIRECTION.UP:
                _forwardDir = Vector3.up;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.forward;
                break;
            case DIRECTION.BACKWARD:
                _forwardDir = -Vector3.forward;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.up;
                break;
            case DIRECTION.LEFT:
                _forwardDir = -Vector3.right;
                _upDir      = Vector3.forward;
                _rightDir   = Vector3.up;
                break;
            case DIRECTION.DOWN:
                _forwardDir = -Vector3.up;
                _upDir      = Vector3.right;
                _rightDir   = Vector3.forward;
                break;
        }

        //set the current distance and rotation so we keep our initial position
        setDistanceToTarget();
        
        //TODO: figure this shit out with _upDir and _rightDir
        _currRotation.x = Vector3.Angle(Vector3.right, transform.right);
        _currRotation.y = Vector3.Angle(Vector3.up, transform.up);

        //TODO: why is this not the other way around????
        //_currRotation.y = Vector3.Angle(_rightDir, transform.rotation * _rightDir);
        //_currRotation.x = Vector3.Angle(_upDir, transform.rotation * _upDir);

        //_currRotation.x = Vector3.Angle(_rightDir, Vector3Ex.Multiply3(transform.rotation.eulerAngles, _rightDir));
        //_currRotation.y = Vector3.Angle(_upDir, Vector3Ex.Multiply3(transform.rotation.eulerAngles, _upDir));

        //_currRotation.x = Vector3Ex.Multiply3(transform.rotation.eulerAngles, _rightDir);
        //_currRotation.y = Vector3Ex.Multiply3(transform.rotation.eulerAngles, _upDir);

        Debug.Log("Curr rotation is "+_currRotation +", transform rotation is "+transform.rotation.eulerAngles);
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
            _currRotation += _inputState.rotation * _rotationSpeed * Time.deltaTime;
        }
        else
        {
            //return rotation
            if ( _returnRotationSpeed.x > 0 )
            {
                _currRotation.x = Mathf.Lerp(_currRotation.x, _returnRotation.x, Time.deltaTime * _returnRotationSpeed.x);
            }

            if ( _returnRotationSpeed.y > 0 )
            {
                _currRotation.y = Mathf.Lerp(_currRotation.y, _returnRotation.y, Time.deltaTime * _returnRotationSpeed.y);
            }
        }

        // clamp the rotation angles
        _currRotation.x = AngleUtil.ClampAngle(_currRotation.x, _horizontalRotationLimits.min, _horizontalRotationLimits.max);
        _currRotation.y = AngleUtil.ClampAngle(_currRotation.y, _verticalRotationLimits.min, _verticalRotationLimits.max);
        
        // set camera rotation, smoothed
        _desiredRotation = Quaternion.Euler((_currRotation.y * _upDir) + (_currRotation.x * _rightDir));

        transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, Time.deltaTime * _rotationDampening);
    }

    // ---- Position ----
    void updatePosition()
    {
        // set zoom amount, smoothed
        _desiredDistance += _inputState.zoom * Time.deltaTime * _zoomSpeed * _desiredDistance;
        _desiredDistance = _distanceLimits.ClampValue(_desiredDistance);
        _currDistance = Mathf.Lerp(_currDistance, _desiredDistance, Time.deltaTime * _zoomDampening);

        // calculate position based on the new distance
        transform.position = _targetPos - (transform.rotation * _forwardDir * _currDistance + _targetOffset);
    }
}