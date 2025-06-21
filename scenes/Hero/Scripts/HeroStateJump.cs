
using Godot;

public class HeroStateJump : IHeroState
{
    private const float CutJumpThreshold = -200.0f;
    private const float JumpForceAfterJumpCutShort = -320.0f;

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
            return hero.StateIdle;
        }
        return hero.StateJump;
    }
}
