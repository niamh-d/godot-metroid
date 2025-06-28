using System.Collections.Generic;
using Godot;

public class HeroStateOnRope : Area2D, IHeroState
{
    private bool Initialized = false;
    protected HeroStateMachine Hero;
    private Rope CurrentRopeHeroIsOn;
    private int CurrentRopeSegmentHeroIsOn = -1;
    private bool facingRight = true;
    private bool RopeSwinging = false;
    private float RopeSegmentProgress = 0.0f;
    private int PreviousRopeSegment = -1;
    private float HeroXVelocity = 0.0f;
    private bool HeroMassAppliedToRope = false;

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        InitState(hero);
        return OnRope(deltatime);
    }

    public string GetStateName()
    {
        return "StateOnRope";
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Hero = hero;
            Initialized = true;

            Hero.HeroArea2Ds.Area2DGrabRopeRight.Connect("body_entered", this, nameof(OnBodyEnteredGrabRopeRight));
            Hero.HeroArea2Ds.Area2DGrabRopeLeft.Connect("body_entered", this, nameof(OnBodyEnteredGrabRopeLeft));
        }
    }
    private void InitiateGrabRope(RopeSegment segment)
    {
        if (!RopeSwinging)
        {
            CurrentRopeHeroIsOn = segment.Rope as Rope;
            CurrentRopeSegmentHeroIsOn = segment.IndexInArray;
            Hero.CurrentState = Hero.StateOnRope;
            RopeSwinging = true;
            Hero.HeroMoveLogic.GravityDisabled = true;
            HeroXVelocity = Hero.HeroMoveLogic.Velocity.x;
            HeroMassAppliedToRope = false;
            Hero.HeroEquipment.Glider.CloseGlider();
        }

        if (!CurrentRopeHeroIsOn.StaticRopeEnd)
        {
            Hero.HeroMoveLogic.HorizontalMovementDisabled = true;
        }

        if (Hero.HeroAnimations.FlipH) facingRight = false;
        else facingRight = true;

        AdjustRopeSegmentHeroIsOn();
        Hero.HeroAnimations.Play("HeroLedgeGrab");
    }

    private void AdjustRopeSegmentHeroIsOn()
    {
        if (CurrentRopeHeroIsOn.StaticRopeEnd)
        {
            if (facingRight) return;
        }


        CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn + 4, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments - 2);
    }

    private void OnBodyEnteredGrabRopeRight(RopeSegment segment)
    {
        if (Input.IsActionPressed("InteractWithObject"))
        {
            InitiateGrabRope(segment);
            facingRight = true;
        }
    }

    private void OnBodyEnteredGrabRopeLeft(RopeSegment segment)
    {
        if (Input.IsActionPressed("InteractWithObject"))
        {
            InitiateGrabRope(segment);
            facingRight = false;
        }
    }

    private Vector2 GetHeroPositionOnRope()
    {
        Vector2 HeroArea2DGrabRopePosition = Vector2.Zero;

        List<RopeSegment> ropeSegments = CurrentRopeHeroIsOn.RopeSegments;

        if (!CurrentRopeHeroIsOn.StaticRopeEnd)
        {
            Hero.HeroAnimations.Rotation = ropeSegments[CurrentRopeHeroIsOn.ActualNumRopeSegments / 2].Rotation * 0.35f;
        }

        var currentSegmentPos = ropeSegments[CurrentRopeSegmentHeroIsOn].GlobalPosition;
        var nextSegment = Mathf.Clamp(CurrentRopeSegmentHeroIsOn + 1, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments);
        var nextSegmentPos = ropeSegments[nextSegment].GlobalPosition;
        var position = Vector2Lerp(currentSegmentPos, nextSegmentPos, RopeSegmentProgress);

        if (facingRight)
        {
            HeroArea2DGrabRopePosition = Hero.HeroArea2Ds.Area2DGrabRopeRight.Position;
        }
        else
        {
            HeroArea2DGrabRopePosition = Hero.HeroArea2Ds.Area2DGrabRopeLeft.Position;
        }

        return position -= HeroArea2DGrabRopePosition;
    }

    private void ClimbUpRope()
    {
        if (CurrentRopeSegmentHeroIsOn <= 2)
        {
            Hero.HeroAnimations.Play("HeroLedgeGrab");
            return;
        }

        RopeSegmentProgress -= 0.2f;

        if (RopeSegmentProgress <= 0)
        {
            RopeSegmentProgress = 1.0f;
            PreviousRopeSegment = CurrentRopeSegmentHeroIsOn;
            CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn - 1, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments - 2);
        }
    }

    private void ClimbDownRope()
    {
        if (CurrentRopeSegmentHeroIsOn >= CurrentRopeHeroIsOn.ActualNumRopeSegments)
        {
            return;
        }
        RopeSegmentProgress += 0.4f;

        if (RopeSegmentProgress >= 1)
        {
            RopeSegmentProgress = 0.0f;
            PreviousRopeSegment = CurrentRopeSegmentHeroIsOn;
            CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn + 1, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments);
        }
    }

    Vector2 Vector2Lerp(Vector2 firstVector, Vector2 secondVector, float amount)
    {
        float retX = Mathf.Lerp(firstVector.x, secondVector.x, amount);
        float retY = Mathf.Lerp(firstVector.y, secondVector.y, amount);

        return new Vector2(retX, retY);
    }

    private void ApplyHeroMassToSwingForce()
    {
        if (!HeroMassAppliedToRope)
        {
            if (HeroXVelocity == 0) return;

            HeroMassAppliedToRope = true;
            float force = 6000.0f;

            if (CurrentRopeHeroIsOn.StaticRopeEnd)
            {
                force = 1000.0f;
            }
            Vector2 forceToApply = Vector2.Zero;
            var forceStepSize = force / CurrentRopeHeroIsOn.ActualNumRopeSegments;

            for (int i = 0; i < CurrentRopeHeroIsOn.ActualNumRopeSegments; ++i)
            {
                if (HeroXVelocity > 0)
                {
                    forceToApply = new Vector2(force, 0);
                }
                else if (HeroXVelocity < 0)
                {
                    forceToApply = new Vector2(-force, 0);
                }
                CurrentRopeHeroIsOn.RopeSegments[i].ApplyCentralImpulse(forceToApply);

                force += forceStepSize;
            }
        }
    }

    private void ApplySwingForceToRope(float force)
    {
        if (CurrentRopeHeroIsOn.StaticRopeEnd) return;

        if (force >= 0 && force < 500)
        {
            force = 500;
        }
        if (force < 0 && force > -500)
        {
            force = -500;
        }
        CurrentRopeHeroIsOn.RopeSegments[CurrentRopeSegmentHeroIsOn].ApplyCentralImpulse(new Vector2(force, 0));
    }

    private IHeroState JumpOffRope()
    {
        var ropeSegment = CurrentRopeHeroIsOn.RopeSegments[CurrentRopeSegmentHeroIsOn];

        Hero.HeroMoveLogic.GravityDisabled = false;
        Hero.HeroMoveLogic.HorizontalMovementDisabled = false;
        Hero.HeroAnimations.Rotation = 0.0f;
        ropeSegment.Mass = 50.0f;

        if (!CurrentRopeHeroIsOn.StaticRopeEnd)
        {
            if (Input.IsActionPressed("MoveLeft") || Input.IsActionPressed("MoveRight"))
            {
                Hero.StateInitJump.SetJumpForce(-500, ropeSegment.LinearVelocity.x * 3);
            }
            else
            {
                Hero.StateInitJump.SetJumpForce(-500);
            }
        }

        else if (CurrentRopeHeroIsOn.StaticRopeEnd)
        {
            Hero.StateInitJump.SetJumpForce(-700, 0);
        }

        RopeSwinging = false;
        return Hero.StateInitJump;
    }

    private void UpdateRopePartMassHeroIsOn()
    {
        if (PreviousRopeSegment == CurrentRopeSegmentHeroIsOn) return;

        CurrentRopeHeroIsOn.RopeSegments[PreviousRopeSegment].Mass = 50.0f;
        CurrentRopeHeroIsOn.RopeSegments[CurrentRopeSegmentHeroIsOn].Mass = 200;
    }

    private void TraverseRopeLeft()
    {
        if (facingRight)
        {
            CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn - 2, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments - 2);
            facingRight = false;
        }
        else
        {
            if (CurrentRopeSegmentHeroIsOn <= 2)
            {
                Hero.HeroAnimations.Play("HeroLedgeGrab");
                return;
            }
        }
        Hero.HeroAnimations.Play("HeroRopeTraverse");
        RopeSegmentProgress -= 0.3f;

        if (RopeSegmentProgress <= 0)
        {
            RopeSegmentProgress = 1.0f;
            PreviousRopeSegment = CurrentRopeSegmentHeroIsOn;
            CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn - 1, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments - 2);
            UpdateRopePartMassHeroIsOn();
        }
    }

    private void TraverseRopeRight()
    {
        if (!facingRight)
        {
            CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn + 2, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments - 2);
            facingRight = true;
        }
        else
        {
            if (CurrentRopeSegmentHeroIsOn >= CurrentRopeHeroIsOn.ActualNumRopeSegments - 2)
            {
                Hero.HeroAnimations.Play("HeroLedgeGrab");
                return;
            }
        }
        Hero.HeroAnimations.Play("HeroRopeTraverse");
        RopeSegmentProgress += 0.3f;

        if (RopeSegmentProgress >= 1)
        {
            RopeSegmentProgress = 0.0f;
            PreviousRopeSegment = CurrentRopeSegmentHeroIsOn;
            CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn + 1, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments - 2);
            UpdateRopePartMassHeroIsOn();
        }
    }

    private IHeroState LetGoOfRope()
    {
        var ropeSegment = CurrentRopeHeroIsOn.RopeSegments[CurrentRopeSegmentHeroIsOn].Mass = 50.0f;
        Hero.HeroMoveLogic.GravityDisabled = false;
        Hero.HeroMoveLogic.HorizontalMovementDisabled = false;
        Hero.HeroMoveLogic.Velocity = Vector2.Zero;
        RopeSwinging = false;
        return Hero.StateFall;
    }

    private IHeroState OnRope(float deltatime)
    {
        if (RopeSwinging)
        {
            Hero.GlobalPosition = GetHeroPositionOnRope();
            ApplyHeroMassToSwingForce();

            if (Input.IsActionPressed("Jump") && !Input.IsActionPressed("InteractWithObject"))
            {
                return JumpOffRope();
            }
            else if (Input.IsActionPressed("Up") && !CurrentRopeHeroIsOn.StaticRopeEnd)
            {
                Hero.HeroAnimations.Play("HeroRopeClimbUp");
                ClimbUpRope();
            }
            else if (Input.IsActionPressed("Down"))
            {
                if (!CurrentRopeHeroIsOn.StaticRopeEnd)
                {
                    ClimbDownRope();
                }
                else if (CurrentRopeHeroIsOn.StaticRopeEnd)
                {
                    return LetGoOfRope();
                }
            }
            else if (Input.IsActionPressed("MoveLeft"))
            {
                if (!CurrentRopeHeroIsOn.StaticRopeEnd)
                {
                    Hero.HeroAnimations.Play("HeroLedgeGrab");
                    ApplySwingForceToRope(-700);
                }
                else if (CurrentRopeHeroIsOn.StaticRopeEnd)
                {
                    TraverseRopeLeft();
                }
            }
            else if (Input.IsActionPressed("MoveRight"))
            {
                if (!CurrentRopeHeroIsOn.StaticRopeEnd)
                {
                    Hero.HeroAnimations.Play("HeroLedgeGrab");
                    ApplySwingForceToRope(700);
                }
                else if (CurrentRopeHeroIsOn.StaticRopeEnd)
                {
                    TraverseRopeRight();
                }
            }
            else
            {
                Hero.HeroAnimations.Play("HeroLedgeGrab");
            }
        }
        return Hero.StateOnRope;
    }
}
