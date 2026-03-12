using UnityEngine;

namespace Karbon {

public sealed class CameraAngleController : MonoBehaviour
{
    [field:SerializeField] public float JumpAngle { get; set; } = 60;
    [field:SerializeField] public float MaxAngle { get; set; } = 90;

    public void Jump()
    {
        var current = transform.localRotation;

        // Try to find a random jump that stays within the max angle limit.
        for (var i = 0; i < 32; i++)
        {
            var axis = Random.onUnitSphere;
            var delta = Quaternion.AngleAxis(JumpAngle, axis);
            var target = current * delta;

            if (Quaternion.Angle(Quaternion.identity, target) <= MaxAngle)
            {
                transform.localRotation = target;
                return;
            }
        }

        // Fallback: If no random jump was found within the limit, 
        // jump directly towards the identity rotation to stay within bounds.
        transform.localRotation = Quaternion.RotateTowards(current, Quaternion.identity, JumpAngle);
    }

    public void ResetAngle()
      => transform.localRotation = Quaternion.identity;
}

} // namespace Karbon
