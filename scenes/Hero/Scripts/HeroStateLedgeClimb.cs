using Godot;

public class HeroStateLedgeClimb : Timer, IHeroState
{
    private bool Initialized = false;
    private bool LedgeClimbTimerHasTimedOut = false;
    private HeroStateMachine Hero;
    private bool LedgeClimbInitiated = false;

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        InitState(hero);
        return LedgeClimb(deltatime);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Hero = hero;
            ConnectLedgeClimbTimerSignal();
            Initialized = true;
        }
    }

    public string GetStateName()
    {
        return "StateLedgeClimb";
    }

    private void ConnectLedgeClimbTimerSignal()
    {
        Hero.HeroTimers.LedgeClimbTimer.Connect("timeout", this, nameof(OnLedgeClimbTimerTimeout));
    }

    private void OnLedgeClimbTimerTimeout()
    {
        Hero.HeroTimers.LedgeClimbTimer.Stop();
        LedgeClimbTimerHasTimedOut = true;
    }

    public IHeroState LedgeClimb(float delta)
    {
        CheckLedgeClimbStateInitiated();
        PerformClimb(delta);

        if (LedgeClimbTimerHasTimedOut)
        {
            LedgeClimbInitiated = false;
            Hero.HeroCollisionShapes.ChangeCollisionShapeToStanding();
            Hero.HeroMoveLogic.MovementDisabled = false;
            Hero.HeroMoveLogic.GravityDisabled = false;
            return Hero.StateIdle;
        }

        Hero.HeroAnimations.Play("HeroLedgeClimb");
        return Hero.StateLedgeClimb;
    }

    private void CheckLedgeClimbStateInitiated()
    {
        if (!LedgeClimbInitiated)
        {
            LedgeClimbTimerHasTimedOut = false;
            Hero.HeroCollisionShapes.DisableAllCollisionShapes();
            Hero.HeroTimers.LedgeClimbTimer.Start();
            LedgeClimbInitiated = true;
        }
    }

    private void PerformClimb(float delta)
    {
        if (Hero.HeroAnimations.FlipH)
        {
            Hero.HeroMoveLogic.Velocity.x = -25;
        }
        else if (!Hero.HeroAnimations.FlipH)
        {
            Hero.HeroMoveLogic.Velocity.x = 25;
        }
        Hero.HeroMoveLogic.Velocity.y = -4300 * delta;
    }
}
