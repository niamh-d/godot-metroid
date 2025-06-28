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
        }

        if (!CurrentRopeHeroIsOn.StaticRopeEnd)
        {
            Hero.HeroMoveLogic.HorizontalMovementDisabled = true;
        }

        AdjustRopeSegmentHeroIsOn();
        Hero.HeroAnimations.Play("HeroLedgeGrab");
    }

    private void AdjustRopeSegmentHeroIsOn()
    {
        CurrentRopeSegmentHeroIsOn = Mathf.Clamp(CurrentRopeSegmentHeroIsOn + 4, 2, CurrentRopeHeroIsOn.ActualNumRopeSegments - 2);
    }

    private void OnBodyEnteredGrabRopeRight(RopeSegment segment)
    {
        InitiateGrabRope(segment);
        facingRight = true;
    }

    private void OnBodyEnteredGrabRopeLeft(RopeSegment segment)
    {
        InitiateGrabRope(segment);
        facingRight = false;
    }

    private Vector2 GetHeroPositionOnRope()
    {
        Vector2 HeroArea2DGrabRopePosition = Vector2.Zero;

        List<RopeSegment> ropeSegments = CurrentRopeHeroIsOn.RopeSegments;

        if (facingRight)
        {
            HeroArea2DGrabRopePosition = Hero.HeroArea2Ds.Area2DGrabRopeRight.Position;
        }
        else
        {
            HeroArea2DGrabRopePosition = Hero.HeroArea2Ds.Area2DGrabRopeLeft.Position;
        }

        return ropeSegments[CurrentRopeSegmentHeroIsOn].GlobalPosition - HeroArea2DGrabRopePosition;
    }

    private IHeroState OnRope(float deltatime)
    {
        Hero.GlobalPosition = GetHeroPositionOnRope();
        return Hero.StateOnRope;
    }
}
