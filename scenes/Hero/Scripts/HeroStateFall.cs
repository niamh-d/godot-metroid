using Godot;

public class HeroStateFall : IHeroState
{

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return Fall(hero, delta);
    }

    private IHeroState Fall(HeroStateMachine hero, float delta)
    {
        hero.HeroMoveLogic.EnableSnap();

        hero.HeroAnimations.Play("HeroFall");

        if (hero.StateLedgeGrab.CanHeroLedgeGrab(hero))
        {
            return hero.StateLedgeGrab;
        }

        if (Input.IsActionPressed("Glide"))
        {
            hero.HeroEquipment.Glider.OpenGlider();
            return hero.StateGlide;
        }

        if (hero.IsOnFloor())
        {
            hero.StateJump.ResetJumpCounter();

            if (hero.IsMoving)
            {
                return hero.StateRun;
            }
            return hero.StateIdle;
        }

        if (Input.IsActionJustPressed("Jump"))
        {

            if (hero.StateJump.CanWallJump(hero))
            {
                return hero.StateInitJump;
            }

            if (hero.StateJump.CanJumpAgainInAir())
            {
                return hero.StateInitJump;
            }
        }

        return hero.StateFall;
    }
}
