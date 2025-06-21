using Godot;

public class HeroMoveLogic
{
    private float Gravity = 1500;
    private float MovementAcceleration = 20;
    public float MaxMovementSpeed = 280;
    private float Friction = 1.0f;
    public Vector2 SnapVector;
    public Vector2 Velocity = Vector2.Zero;
    private HeroStateMachine Hero;
    public bool MovementDisabled = false;

    public HeroMoveLogic(HeroStateMachine hero)
    {
        Hero = hero;
    }

    public void MoveHero(float delta)
    {
        Velocity = Hero.MoveAndSlideWithSnap(Velocity, SnapVector, Vector2.Up, stopOnSlope: true);

        if (!Hero.IsMoving)
        {
            if (IsHeroOnSlope() || Hero.IsOnFloor())
            {
                Velocity.x = Mathf.Lerp(Velocity.x, 0, Friction);
            }
        }
    }

    public void UpdateMovement(float delta)
    {
        float leftDirectionStrength = Input.GetActionStrength("MoveLeft");
        float rightDirectionStrength = Input.GetActionStrength("MoveRight");

        if (!MovementDisabled)
        {
            UpdateVelocity(leftDirectionStrength, rightDirectionStrength);

            UpdateRightMovement(leftDirectionStrength, rightDirectionStrength);
            UpdateLeftMovement(leftDirectionStrength, rightDirectionStrength);

            UpdateIsMoving(leftDirectionStrength, rightDirectionStrength);
        }
    }

    private void UpdateVelocity(float leftDirectionStrength, float rightDirectionStrength)
    {
        if (leftDirectionStrength > 0 && rightDirectionStrength > 0)
        {
            if (Hero.HeroAnimations.FlipH)
            {
                Velocity.x -= leftDirectionStrength * MovementAcceleration;
            }
            else
            {
                Velocity.x += rightDirectionStrength * MovementAcceleration;
            }
        }
        else
        {
            Velocity.x += (rightDirectionStrength - leftDirectionStrength) * MovementAcceleration;
        }

        Velocity.x = Mathf.Clamp(Velocity.x, -MaxMovementSpeed, MaxMovementSpeed);
    }

    private void UpdateRightMovement(float leftDirectionStrength, float rightDirectionStrength)
    {
        if (leftDirectionStrength < rightDirectionStrength)
        {
            if (IsHeroOnSlope())
            {
                if (Friction == 1.0f)
                {
                    Velocity.x = MaxMovementSpeed;
                }
            }

            Hero.HeroAnimations.FlipH = false;
        }
    }

    private void UpdateLeftMovement(float leftDirectionStrength, float rightDirectionStrength)
    {
        if (rightDirectionStrength < leftDirectionStrength)
        {
            if (IsHeroOnSlope())
            {
                if (Friction == 1.0f)
                {
                    Velocity.x = -MaxMovementSpeed;
                }
            }

            Hero.HeroAnimations.FlipH = true;
        }
    }

    private void UpdateIsMoving(float leftDirectionStrength, float rightDirectionStrength)
    {
        if (leftDirectionStrength is 0 && rightDirectionStrength is 0)
        {
            Hero.IsMoving = false;
        }
        else
        {
            Hero.IsMoving = true;
        }
    }

    private bool IsHeroOnSlope()
    {
        if (Hero.GetFloorNormal().x != 0)
        {
            return true;
        }
        return false;
    }

    public void ApplyGravity(float delta)
    {
        Velocity.y += Gravity * delta;
    }

    public void EnableSnap()
    {
        SnapVector = new Vector2(0, 15);
    }

    public void DisableSnap()
    {
        SnapVector = Vector2.Zero;
    }
}
