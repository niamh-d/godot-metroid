using Godot;

public class HeroStateLedgeGrab : Timer, IHeroState
{
    private bool Initialized = false;
    private HeroStateMachine Hero;
    private bool FallAfterLedgeGrab = false;

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        InitState(hero);
        return LedgeGrab();
    }

    private void InitState(HeroStateMachine hero)
    {

        if (!Initialized)
        {
            Hero = hero;
            ConnectLedgeFallTimerSignal();
            Initialized = true;
        }
    }

    private void ConnectLedgeFallTimerSignal()
    {
        Hero.HeroTimers.LedgeFallTimer.Connect("timeout", this, nameof(OnLedgeFallTimerTimeout));
    }

    private void OnLedgeFallTimerTimeout()
    {
        FallAfterLedgeGrab = false;
    }

    public IHeroState LedgeGrab()
    {
        if (IsHangingInEmptyAir())
        {
            return Hero.StateFall;
        }

        Hero.HeroMoveLogic.GravityDisabled = true;
        Hero.HeroMoveLogic.MovementDisabled = true;
        Hero.HeroMoveLogic.Velocity.y = 0;

        if (Input.IsActionJustPressed("Down"))
        {
            FallAfterLedgeGrab = true;
            Hero.HeroTimers.LedgeFallTimer.Start();
            Hero.HeroMoveLogic.GravityDisabled = false;
            Hero.HeroMoveLogic.MovementDisabled = false;
            return Hero.StateFall;
        }

        Hero.HeroAnimations.Play("HeroLedgeGrab");
        return Hero.StateLedgeGrab;
    }

    public bool CanHeroLedgeGrab(HeroStateMachine hero)
    {
        if (FallAfterLedgeGrab)
        {
            return false;
        }

        if (!hero.HeroRayCasts.LedgeGrabRayCastTileAbove.IsColliding() && hero.HeroRayCasts.LedgeGrabRayCastTileHead.IsColliding())
        {
            return true;
        }
        return false;
    }

    private bool IsHangingInEmptyAir()
    {
        if (!Hero.HeroRayCasts.LedgeGrabRayCastTileAbove.IsColliding() && !Hero.HeroRayCasts.LedgeGrabRayCastTileHead.IsColliding())
        {
            return true;
        }
        return false;
    }

}
