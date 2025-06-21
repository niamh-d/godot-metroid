using Godot;

public class HeroStateSlide : Timer, IHeroState
{
    private HeroStateMachine Hero;
    private bool SlideTimerHasTimedOut = false;
    private bool SlidingInitiated = false;
    private bool Initialized = false;

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        InitState(hero);
        return Slide(deltatime);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Hero = hero;
            ConnectSlideTimerSignal();
            Initialized = true;
        }
    }

    private void ConnectSlideTimerSignal()
    {
        Hero.HeroTimers.SlideTimer.Connect("timeout", this, nameof(OnSlideTimerTimeout));
    }

    private void OnSlideTimerTimeout()
    {
        Hero.HeroTimers.SlideTimer.Stop();
        SlideTimerHasTimedOut = true;
    }

    private IHeroState Slide(float delta)
    {
        CheckSlideStateInitiated();
        Hero.HeroMoveLogic.EnableSnap();
        Hero.HeroAnimations.Play("HeroSlide");
        Hero.HeroCollisionShapes.ChangeCollisionShapeToSlide();

        if (!Hero.IsOnFloor())
        {
            SlidingInitiated = false;
            Hero.HeroMoveLogic.MovementDisabled = false;
            Hero.HeroCollisionShapes.ChangeCollisionShapeToStanding();
            return Hero.StateFall;
        }


        if (SlideTimerHasTimedOut)
        {
            SlidingInitiated = false;
            return Hero.StateSlideStandUp;
        }
        return PerformSlide();
    }

    private void CheckSlideStateInitiated()
    {
        if (!SlidingInitiated)
        {
            SlideTimerHasTimedOut = false;
            Hero.HeroTimers.SlideTimer.Start();
            SlidingInitiated = true;
        }
    }

    private IHeroState PerformSlide()
    {
        if (Hero.HeroAnimations.FlipH)
        {
            Hero.HeroMoveLogic.Velocity.x = -Hero.HeroMoveLogic.MaxMovementSpeed;
            Hero.HeroMoveLogic.MovementDisabled = true;
        }

        if (!Hero.HeroAnimations.FlipH)
        {
            Hero.HeroMoveLogic.Velocity.x = Hero.HeroMoveLogic.MaxMovementSpeed;
            Hero.HeroMoveLogic.MovementDisabled = true;
        }
        return Hero.StateSlide;
    }
}
