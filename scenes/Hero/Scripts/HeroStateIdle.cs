using Godot;

public class HeroStateIdle : Timer, IHeroState
{
    private bool Initialized;
    private HeroStateMachine Hero;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return Idle(delta);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;
            Hero = hero;

            Hero.HeroTimers.PassThroughPlatformTimer.Connect("timeout", this, nameof(OnPassThroughPlatformTimerTimeout));
        }
    }

    public string GetStateName()
    {
        return "StateIdle";
    }

    private void OnPassThroughPlatformTimerTimeout()
    {
        Hero.HeroCollisionShapes.TurnOnPassThroughPlatformCollision((int)HeroCollisionShapes.CollisionLayersEnum.PASS_THROUGH_PLATFORM_LAYER);
    }

    private IHeroState Idle(float delta)
    {
        if (Hero.IsOnFloor())
        {
            Hero.HeroMoveLogic.EnableSnap();

            if (Input.IsActionJustPressed("Jump") && Input.IsActionPressed("Down"))
            {
                return Hero.StateSlide;
            }

            if (Input.IsActionJustPressed("Down"))
            {
                Hero.HeroCollisionShapes.TurnOffPassThroughPlatformCollision((int)HeroCollisionShapes.CollisionLayersEnum.PASS_THROUGH_PLATFORM_LAYER);
                Hero.HeroTimers.PassThroughPlatformTimer.Start();
            }

            if (Input.IsActionJustPressed("Jump"))
            {
                return Hero.StateInitJump;
            }

            if (Input.IsActionJustPressed("Attack"))
            {
                return Hero.StateAttack;
            }

            Hero.HeroAnimations.Play("HeroIdle");

            if (Hero.HeroMoveLogic.IsMoving)
            {
                return Hero.StateRun;
            }

            return Hero.StateIdle;
        }
        else if (!Hero.IsOnFloor())
        {
            return Hero.StateFall;
        }
        return Hero.StateIdle;
    }
}