
using Godot;

public class HeroStateJump : IHeroState
{
    private const float CutJumpThreshold = -200.0f;
    private const float JumpForceAfterJumpCutShort = -320.0f;
    private int MaxJumps = 2;
    private int JumpCount = 0;
    private bool Initialized = false;
    private HeroStateMachine Hero;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return Jump(delta);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;
            Hero = hero;
        }
    }

    public string GetStateName()
    {
        return "StateJump";
    }

    private IHeroState Jump(float delta)
    {
        Hero.HeroMoveLogic.DisableSnap();

        Hero.HeroAnimations.Play("HeroJump");

        if (Input.IsActionJustReleased("Jump") && Hero.HeroMoveLogic.Velocity.y < CutJumpThreshold)
        {
            Hero.HeroMoveLogic.Velocity.y = JumpForceAfterJumpCutShort;
            return Hero.StateFall;
        }

        if (Input.IsActionPressed("Glide"))
        {
            Hero.HeroAnimations.Play("HeroFall");
            Hero.HeroEquipment.Glider.OpenGlider();
            return Hero.StateGlide;
        }

        if (Input.IsActionJustPressed("Attack"))
        {
            return Hero.StateAttack;
        }

        if (!Hero.IsOnFloor())
        {
            if (Hero.HeroMoveLogic.Velocity.y < 0)
            {
                CornerCorrectJump(delta);
                return Hero.StateJump;
            }

            if (Hero.HeroMoveLogic.Velocity.y > 0)
            {
                return Hero.StateFall;
            }
        }
        else if (Hero.IsOnFloor())
        {
            ResetJumpCounter();
            return Hero.StateIdle;
        }
        return Hero.StateJump;
    }

    public void SetMaxJumps(int numJumps)
    {
        this.MaxJumps = numJumps;
    }

    public void ResetJumpCounter()
    {
        this.JumpCount = 0;
    }

    public bool CanJumpAgainInAir()
    {
        JumpCount++;
        return JumpCount < MaxJumps;
    }

    public bool CanWallJump()
    {
        if (Hero.HeroRayCasts.LeftWallRayCast.IsColliding() && !Hero.HeroAnimations.FlipH)
        {
            return true;
        }

        if (Hero.HeroRayCasts.RightWallRayCast.IsColliding() && Hero.HeroAnimations.FlipH)
        {
            return true;
        }
        return false;
    }

    public bool CanHeroPerformBufferJump()
    {
        if (!Hero.IsOnFloor()
        && Hero.HeroRayCasts.JumpBufferRayCast.IsColliding()
        && Hero.HeroMoveLogic.Velocity.y > 0)
        {
            return true;
        }
        return false;
    }

    private void CornerCorrectJump(float delta)
    {
        if (Hero.HeroRayCasts.CornerCorrectionLeftRayCast.IsColliding()
        && !Hero.HeroRayCasts.CornerCorrectionMiddleRayCast.IsColliding())
        {
            if (!Hero.HeroRayCasts.LedgeGrabRayCastTileHead.IsColliding())
            {
                Hero.Translate(new Vector2(400 * delta, 0));
            }
        }

        if (Hero.HeroRayCasts.CornerCorrectionRightRayCast.IsColliding()
        && !Hero.HeroRayCasts.CornerCorrectionMiddleRayCast.IsColliding())
        {
            if (!Hero.HeroRayCasts.LedgeGrabRayCastTileHead.IsColliding())
            {
                Hero.Translate(new Vector2(-400 * delta, 0));
            }
        }
    }
}
