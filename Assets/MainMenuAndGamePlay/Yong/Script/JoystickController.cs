using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    // movement speed of the character
    public float speed = 10f;
    // reference to the character's rigidbody
    private Rigidbody rb;
    // reference to the joystick object
    public RectTransform joystick;
    // maximum distance that the joystick can be dragged from its center
    public float maxDragDistance = 50f;
    // current position of the joystick
    private Vector2 joystickPosition;
    // flag indicating whether the joystick is being dragged
    private bool isDragging = false;

    void Start()
    {
        // get the rigidbody component of the character
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // if the joystick is being dragged, move the character
        if (isDragging)
        {
            // calculate the movement vector based on the joystick position
            Vector3 movement = new Vector3(joystickPosition.x, 0f, joystickPosition.y);

            // normalize the movement vector to ensure that the character moves at a consistent speed
            movement = movement.normalized * speed * Time.deltaTime;

            // move the character by applying a force to the rigidbody
            rb.AddForce(movement, ForceMode.VelocityChange);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // calculate the current position of the joystick
        joystickPosition = (Vector2)joystick.position - eventData.position;

        // limit the distance that the joystick can be dragged from its center
        joystickPosition = Vector2.ClampMagnitude(joystickPosition, maxDragDistance);

        // update the position of the joystick
        joystick.position = eventData.position + joystickPosition;

        // set the flag indicating that the joystick is being dragged
        isDragging = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // set the flag indicating that the joystick is being dragged
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // reset the position and flag of the joystick
        joystick.position = joystick.parent.position;
        isDragging = false;
    }
}
