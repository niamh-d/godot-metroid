
using Godot;

public class HeroStateJump : IHeroState
{
    private const float CutJumpThreshold = -200.0f;
    private const float JumpForceAfterJumpCutShort = -320.0f;
    private int MaxJumps = 1;
    private int JumpCount = 0;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return Jump(hero, delta);
    }

    private IHeroState Jump(HeroStateMachine hero, float delta)
    {
        hero.HeroMoveLogic.DisableSnap();

        hero.HeroAnimations.Play("HeroJump");

        if (Input.IsActionJustReleased("Jump") && hero.HeroMoveLogic.Velocity.y < CutJumpThreshold)
        {
            hero.HeroMoveLogic.Velocity.y = JumpForceAfterJumpCutShort;
            return hero.StateFall;
        }

        if (Input.IsActionPressed("Glide"))
        {
            hero.HeroAnimations.Play("HeroFall");
            hero.HeroEquipment.Glider.OpenGlider();
            return hero.StateGlide;
        }

        if (!hero.IsOnFloor())
        {
            if (hero.HeroMoveLogic.Velocity.y < 0)
            {
                return hero.StateJump;
            }

            if (hero.HeroMoveLogic.Velocity.y > 0)
            {
                return hero.StateFall;
            }
        }
        else if (hero.IsOnFloor())
        {
            ResetJumpCounter();
            return hero.StateIdle;
        }
        return hero.StateJump;
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

    public bool CanWallJump(HeroStateMachine hero)
    {
        if (hero.HeroRayCasts.LeftWallRayCast.IsColliding() && !hero.HeroAnimations.FlipH)
        {
            return true;
        }

        if (hero.HeroRayCasts.RightWallRayCast.IsColliding() && hero.HeroAnimations.FlipH)
        {
            return true;
        }
        return false;
    }
}
