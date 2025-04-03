using Spine.Unity;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    [SpineBone]
    public string boneName;
    Spine.Bone bone;

    public Vector3 targetPosition;
    public float targetRotation;

    void Start()
    {
        SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();

        this.bone = skeletonAnimation.Skeleton.FindBone(boneName);
        skeletonAnimation.UpdateLocal += SkeletonAnimation_UpdateLocal;
    }

    void SkeletonAnimation_UpdateLocal(ISkeletonAnimation animated)
    {
        bone.SetLocalPosition(targetPosition);
        bone.Rotation = targetRotation;
    }

}
