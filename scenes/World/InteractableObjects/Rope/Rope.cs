using System.Collections.Generic;
using Godot;

public class Rope : Node2D
{
    [Export]
    public bool StaticRopeEnd = false;

    private float SegmentLength = 4f;
    private PackedScene RopeSegment;
    private RopeSegment RopeStart;
    private RopeSegment RopeEnd;
    private PinJoint2D RopeStartPinJoint;
    private PinJoint2D RopeEndPinJoint;
    private List<Vector2> RopePointsLine2D;
    private Line2D Line2DNode;
    private float MinDistanceToRopeEndSegment = 4.0f;

    public List<RopeSegment> RopeSegments = new List<RopeSegment>();
    public int ActualNumRopeSegments = 0;

    public override void _Ready()
    {
        RopePointsLine2D = new List<Vector2>();
        RopeSegment = GD.Load<PackedScene>("res://Scenes/World/InteractableObjects/Rope/RopeSegment.tscn");
        Line2DNode = GetNode<Line2D>("Line2D");
        RopeStart = GetNode<RopeSegment>("RopeStart");
        RopeEnd = GetNode<RopeSegment>("RopeEnd");
        RopeStart.Rope = this;
        RopeEnd.Rope = this;
        RopeStartPinJoint = GetNode<PinJoint2D>("RopeStart/Collision/PinJoint");
        RopeEndPinJoint = GetNode<PinJoint2D>("RopeEnd/Collision/PinJoint");
        SpawnRope(RopeStart.GlobalPosition, RopeEnd.GlobalPosition);
    }

    private void SpawnRope(Vector2 ropeStartPos, Vector2 ropeEndPos)
    {
        if (StaticRopeEnd)
        {
            RopeEnd.Mode = RigidBody2D.ModeEnum.Static;
        }

        else
        {
            RopeEnd.Mode = RigidBody2D.ModeEnum.Rigid;
        }

        var dist = ropeStartPos.DistanceTo(ropeEndPos);
        var numSegmentsEstimate = (int)Mathf.Ceil(dist / SegmentLength);
        var rotationAngle = (ropeEndPos - ropeStartPos).Angle() - Mathf.Pi / 2;

        RopeStart.IndexInArray = 0;
        ropeEndPos = RopeEndPinJoint.GlobalPosition;

        CreateRope(numSegmentsEstimate, RopeStart, ropeEndPos, rotationAngle);

        RopeEnd.IndexInArray = ActualNumRopeSegments;
    }

    private void CreateRope(int numSegmentsEstimate, RopeSegment sibling, Vector2 RopeEnd, float rotationAngle)
    {
        RopeSegments.Clear();

        for (int i = 0; i < numSegmentsEstimate; ++i)
        {
            sibling = AddRopeSegment(sibling, i, rotationAngle);
            sibling.Name = "RopePiece" + i;
            RopeSegments.Add(sibling);
            var jointPos = sibling.GetNode<PinJoint2D>("Collision/PinJoint").GlobalPosition;

            if (jointPos.DistanceTo(RopeEnd) < MinDistanceToRopeEndSegment)
            {
                break;
            }
        }

        ActualNumRopeSegments = RopeSegments.Count - 1;
        RopeEndPinJoint.NodeA = this.RopeEnd.GetPath();
        RopeEndPinJoint.NodeB = RopeSegments[ActualNumRopeSegments].GetPath();
    }

    private RopeSegment AddRopeSegment(RopeSegment sibling, int id, float rotationAngle)
    {
        PinJoint2D pinJoint = sibling.GetNode("Collision/PinJoint") as PinJoint2D;
        var segment = RopeSegment.Instance() as RopeSegment;
        segment.GlobalPosition = pinJoint.GlobalPosition;
        segment.Rotation = rotationAngle;
        segment.Rope = this;
        segment.IndexInArray = id;

        if (StaticRopeEnd)
        {
            segment.Mass = 30.0f;
            segment.GravityScale = 2.0f;
        }

        AddChild(segment);
        pinJoint.NodeA = sibling.GetPath();
        pinJoint.NodeB = segment.GetPath();
        pinJoint.Bias = 0.9f;
        pinJoint.Softness = 0.003f;
        return segment;
    }

    private void GetLine2DRopePoints()
    {
        RopePointsLine2D.Clear();

        RopePointsLine2D.Add(RopeStartPinJoint.GlobalPosition);

        foreach (var segment in RopeSegments)
        {
            RopePointsLine2D.Add(segment.GlobalPosition);
        }
        RopePointsLine2D.Add(RopeEndPinJoint.GlobalPosition);
        Line2DNode.Points = RopePointsLine2D.ToArray();
    }

    public override void _Process(float delta)
    {
        GetLine2DRopePoints();
    }
}
