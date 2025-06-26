using Godot;

public class HeroStateSlideStandUp : Timer, IHeroState
{
    private bool IsInNarrowPassage;
    private bool Initialized = false;
    private bool SlideStandUpTimerHasTimedOut = false;
    private bool SlidingStandUpInitiated = false;
    private HeroStateMachine Hero;

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        InitState(hero);
        return SlideStandUp(deltatime);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Hero = hero;
            ConnectSlideStandUpTimerSignal();
            Initialized = true;
        }
    }

    public string GetStateName()
    {
        return "StateSlideStandUp";
    }

    private void ConnectSlideStandUpTimerSignal()
    {
        Hero.HeroTimers.SlideStandUpTimer.Connect("timeout", this, nameof(OnSlideStandUpTimerTimeout));
    }

    private void OnSlideStandUpTimerTimeout()
    {
        Hero.HeroTimers.SlideStandUpTimer.Stop();
        SlideStandUpTimerHasTimedOut = true;
    }

    private IHeroState SlideStandUp(float delta)
    {
        CheckSlideStandUpStateInitiated();
        CheckIfInNarrowPassage();

        if (Hero.IsOnFloor() || IsInNarrowPassage)
        {
            Hero.HeroCollisionShapes.ChangeCollisionShapesToSlideStandUp();
            Hero.HeroAnimations.Play("HeroSlideStandUp");

            if (SlideStandUpTimerHasTimedOut)
            {
                Hero.HeroMoveLogic.MovementDisabled = false;

                if (Input.IsActionJustPressed("Jump") && Input.IsActionPressed("Down"))
                {
                    ResetSlideStandUpState();
                    Hero.HeroCollisionShapes.ChangeCollisionShapeToSlide();
                    return Hero.StateSlide;
                }

                if (!IsInNarrowPassage)
                {
                    ResetSlideStandUpState();
                    Hero.HeroCollisionShapes.ChangeCollisionShapeToStanding();
                    return Hero.StateIdle;
                }
            }
        }


        return Hero.StateSlideStandUp;
    }

    private void ResetSlideStandUpState()
    {
        SlideStandUpTimerHasTimedOut = false;
        SlidingStandUpInitiated = false;
    }

    private void CheckSlideStandUpStateInitiated()
    {
        if (!SlidingStandUpInitiated)
        {
            Hero.HeroTimers.SlideStandUpTimer.Start();
            SlidingStandUpInitiated = true;
        }
    }

    private void CheckIfInNarrowPassage()
    {
        if (Hero.HeroCollisionShapes.IsCollisionShape2DColliding("CollisionShapeHead"))
        {
            IsInNarrowPassage = true;
        }
        else { IsInNarrowPassage = false; }
    }
}
