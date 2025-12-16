using Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private float speed;
    
    private Vector2 _currentMovementValue;
    private void Start()
    {
        playerInputHandler.Movement += HandleUpdateMovementValue;
        playerInputHandler.Interaction += HandleInteraction;
    }

    private void Update()
    {
        if (_currentMovementValue.magnitude > Mathf.Epsilon)
        {
            transform.Translate(_currentMovementValue * (speed * Time.deltaTime));
        }
        // else
        // {
        //     transform.position = Vector3.MoveTowards(transform.position, closestGridPosition, m_MaxDistanceDelta);
        // }
    }

    private void HandleUpdateMovementValue(Vector2 value)
    {
        _currentMovementValue = value;
    }

    private void HandleInteraction()
    {
        
    }
}
