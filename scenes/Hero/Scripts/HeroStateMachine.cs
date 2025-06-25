using Godot;

public class HeroStateMachine : KinematicBody2D
{
    public HeroStateIdle StateIdle = new HeroStateIdle();
    public HeroStateRun StateRun = new HeroStateRun();
    public HeroStateFall StateFall = new HeroStateFall();
    public HeroStateInitJump StateInitJump = new HeroStateInitJump();
    public HeroStateJump StateJump = new HeroStateJump();
    public HeroStateSlide StateSlide = new HeroStateSlide();
    public HeroStateSlideStandUp StateSlideStandUp = new HeroStateSlideStandUp();
    public HeroStateLedgeGrab StateLedgeGrab = new HeroStateLedgeGrab();
    public HeroStateLedgeClimb StateLedgeClimb = new HeroStateLedgeClimb();
    public HeroStateGlide StateGlide = new HeroStateGlide();
    public HeroStateAttack StateAttack = new HeroStateAttack();
    public HeroMoveLogic HeroMoveLogic;
    public HeroCollisionShapes HeroCollisionShapes;
    public HeroTimers HeroTimers;
    public Hero2DRayCasts HeroRayCasts;
    public AnimatedSprite HeroAnimations;
    public HeroEquipment HeroEquipment;
    public IHeroState CurrentState;
    private bool IsInitialized = false;
    public string LastPlayedHeroAnimation = string.Empty;

    public override void _Ready()
    {
        IsInitialized = InitHeroStateMachine();

        if (!IsInitialized)
        {
            GD.Print("HeroStateMachine.cs – Error, could not initialize hero state machine.");
        }
    }

    private bool InitHeroStateMachine()
    {
        bool initOk = true;
        CurrentState = StateIdle;
        HeroMoveLogic = new HeroMoveLogic(this);

        HeroCollisionShapes = new HeroCollisionShapes(this, ref initOk);
        if (!initOk) return false;

        HeroTimers = new HeroTimers(this, ref initOk);
        if (!initOk) return false;

        HeroRayCasts = new Hero2DRayCasts(this, ref initOk);
        if (!initOk) return false;

        HeroEquipment = new HeroEquipment(this, ref initOk);
        if (!initOk) return false;

        initOk = GetHeroAnimationsNode();
        if (!initOk) return false;

        return true;
    }

    private bool GetHeroAnimationsNode()
    {
        HeroAnimations = GetNode<AnimatedSprite>("./HeroAnimations");

        if (HeroAnimations is null)
        {
            GD.PrintErr("HeroStateMachine.cs – GetHeroAnimationsNode() – HeroAnimations is null.");
            return false;
        }
        return true;
    }

    private void HeroAnimationDone()
    {
        LastPlayedHeroAnimation = HeroAnimations.Animation.ToString();
    }


    private void UpdateHeroState(float delta)
    {
        CurrentState = CurrentState.DoState(this, delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (IsInitialized)
        {
            UpdateHeroState(delta);
            HeroMoveLogic.ApplyGravity(delta);
            HeroMoveLogic.UpdateMovement(delta);
            HeroMoveLogic.MoveHero(delta);
        }
    }
}
