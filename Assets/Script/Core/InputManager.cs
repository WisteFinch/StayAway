using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public bool JumpKeyDown = false;
    public bool JumpKeyPressed = false;
    public bool JumpKeyUp = false;
    public bool DashKeyDown = false;
    public bool InteractKeyDown = false;
    public bool FireKeyDown = false;
    public bool LightKeyDown = false;
    public bool ChangeCharacterKeyDown = false;
    public bool EscapeKeyDown = false;
    public Vector2 MoveAxis = Vector2.zero;
    public Vector2 MousePosition = Vector2.zero;

    private void Update()
    {
        this.MousePosition = Mouse.current.position.ReadValue();
    }

    private void LateUpdate()
    {
        this.JumpKeyDown = false;
        this.JumpKeyUp = false;
        this.DashKeyDown = false;
        this.InteractKeyDown = false;
        this.FireKeyDown = false;
        this.LightKeyDown = false;
        this.ChangeCharacterKeyDown = false;
        this.EscapeKeyDown = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            this.JumpKeyDown = true;
        if (context.phase == InputActionPhase.Canceled)
            this.JumpKeyUp = true;
        this.JumpKeyPressed = context.phase == InputActionPhase.Performed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        this.MoveAxis = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            this.InteractKeyDown = true;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            this.DashKeyDown = true;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            this.FireKeyDown = true;
    }

    public void OnLight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            this.LightKeyDown = true;
    }

    public void OnChangeCharacter(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            this.ChangeCharacterKeyDown = true;
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            this.EscapeKeyDown = true;
    }
}
