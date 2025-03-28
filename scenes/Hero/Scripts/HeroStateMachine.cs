using Godot;

public class HeroStateMachine : KinematicBody2D
{
    public HeroStateIdle StateIdle = new HeroStateIdle();
    public HeroStateRun StateRun = new HeroStateRun();
    public HeroStateFall StateFall = new HeroStateFall();
    public HeroStateInitJump StateInitJump = new HeroStateInitJump();
    public HeroStateJump StateJump = new HeroStateJump();
    public HeroMoveLogic HeroMoveLogic;
    public AnimatedSprite HeroAnimations;
    private IHeroState CurrentState;
    private bool IsInitialized = false;
    public bool IsMoving;
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
        bool initOk;
        CurrentState = StateIdle;
        HeroMoveLogic = new HeroMoveLogic(this);
        initOk = GetHeroAnimationsNode();
        return initOk;
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

    public void EnableSnap()
    {
        HeroMoveLogic.SnapVector = new Vector2(0, 15);
    }

    public void DisableSnap()
    {
        HeroMoveLogic.SnapVector = Vector2.Zero;
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
