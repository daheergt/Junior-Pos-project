using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState Anim;

    private Vector2 StartPos;
    private Vector2 EndPos;
    private Vector2 Direction;

    [Header("Shooting/Animation")]
    public TargetFollow Power;
    public TargetFollow TargetRot;

    public GameObject Arrow;

    [SpineBone]
    public string ShootPoint;
    Spine.Bone bone;

    public float StringPressure;
    public float StartForce;

    [Header("Shooting Path")]
    public GameObject Point;
    GameObject[] Points;
    public int NumberOfPoints;
    public float Spacing;

    public Transform PointHolder;

    Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)bone.GetWorldPosition(transform) + (Direction.normalized * StartForce * (StringPressure * 3) * t) + 0.5f * Physics2D.gravity * (t * t);
        return position;
    }
    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        Anim = skeletonAnimation.AnimationState;

        // Get Bone to shoot from
        this.bone = skeletonAnimation.Skeleton.FindBone(ShootPoint);

        // Spawning points for path
        Points = new GameObject[NumberOfPoints];
        for (int i = 0; i < NumberOfPoints; i++)
            Points[i] = Instantiate(Point, transform.position, Quaternion.identity, PointHolder);

        PointHolder.gameObject.SetActive(false);
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            PointHolder.gameObject.SetActive(true);

            Anim.SetAnimation(0, "attack_start", false);
        }
        else if (Input.GetMouseButton(0))
        {
            EndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Rotation character
            Direction = StartPos - EndPos;
            float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;

            angle = Mathf.Clamp(angle, -90, 90);

            TargetRot.targetRotation = angle;

            // Power

            StringPressure = Vector2.Distance(StartPos, EndPos) / 5;           

            StringPressure = Mathf.Clamp(StringPressure, 0, 0.6f);

            Power.targetPosition.x = 1 - StringPressure;

            // Points 

            for (int i = 0; i < NumberOfPoints; i++)
            {
                Direction.x = Mathf.Clamp(Direction.x, 0, Mathf.Infinity);

                Points[i].transform.position = PointPosition(i * Spacing);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Anim.SetAnimation(0, "attack_finish", false);
            Anim.AddAnimation(0, "idle", true, 1);
            // Shooting Arrow
            GameObject _Arrow = Instantiate(Arrow, bone.GetWorldPosition(transform), Quaternion.Euler(0, 0, TargetRot.targetRotation));
            _Arrow.GetComponent<Rigidbody2D>().velocity = _Arrow.transform.right * StartForce * (StringPressure * 3);

            PointHolder.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(StartPos, EndPos);
    }
}
