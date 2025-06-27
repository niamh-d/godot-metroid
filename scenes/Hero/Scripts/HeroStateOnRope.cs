using System;
using Godot;

public class HeroStateOnRope : Area2D, IHeroState
{
    private bool Initialized = false;
    protected HeroStateMachine Hero;
    private Rope CurrentRopeHeroIsOn;
    private int CurrentRopeSegmentHeroIsOn = -1;

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

    private object OnBodyEnteredGrabRopeLeft(RopeSegment segment)
    {
        throw new NotImplementedException();
    }

    private object OnBodyEnteredGrabRopeRight(RopeSegment segment)
    {
        throw new NotImplementedException();
    }

    private IHeroState OnRope(float deltatime)
    {
        throw new System.NotImplementedException();
    }
}
