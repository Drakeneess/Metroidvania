using System.Collections;
using UnityEngine;

public class SingleWeapon : Weapon
{
    private Coroutine currentRotationRoutine;

    public override void LightAttack(int comboIndex)
    {
        if (isAttacking || !gameObject.activeInHierarchy) return;
        PlayMotion(comboIndex);
    }

    public override void HeavyAttack(float value)
    {
        if (isAttacking) return;
        PlayMotion(comboAnimations.Length - 1); // usa la última animación
    }

    protected void PlayMotion(int comboIndex)
    {
        if (comboAnimations == null || comboAnimations.Length == 0) return;
        if (comboIndex < 0 || comboIndex >= comboAnimations.Length) return;

        var pose = comboAnimations[comboIndex];
        if (currentRotationRoutine != null)
            StopCoroutine(currentRotationRoutine);

        int facing = GetFacingDirection();

        // Global FROM (rotación actual en mundo)
        Quaternion fromQ = transform.rotation;

        // Global TO (parent rotation * endRotation)
        Quaternion toQ = transform.parent.rotation * Quaternion.Euler(pose.endRotation);

        // Flip global en Y si está mirando a la izquierda
        if (facing < 0)
            toQ = Quaternion.Euler(0f, 180f, 0f) * toQ;

        currentRotationRoutine = StartCoroutine(RotateToPose(pose, fromQ, toQ));
    }

    private IEnumerator RotateToPose(ComboPose pose, Quaternion fromQ, Quaternion toQ)
    {
        isAttacking = true;

        float duration = Mathf.Max(0.0001f, pose.duration);
        float elapsed  = 0f;
        bool impacted  = false;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            // Interpolación global
            transform.rotation = Quaternion.Slerp(fromQ, toQ, t);

            // Shake en Z (sobre rotación actual en mundo)
            float mid   = 1f - Mathf.Abs(0.5f - t) * 2f;
            float shake = (pose.shakeIntensity > 0f)
                ? pose.shakeIntensity * Mathf.Sin(Time.time * 80f) * mid
                : 0f;
            transform.rotation *= Quaternion.Euler(0f, 0f, shake);

            if (!impacted && t >= pose.impactTime)
            {
                impacted = true;
                ActivateDamageArea();
                isAttacking = false;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = toQ;

        // Recoil en global
        if (pose.recoilDegrees > 0f && pose.recoilDuration > 0f)
        {
            float rElapsed = 0f;
            while (rElapsed < pose.recoilDuration)
            {
                float rt = rElapsed / pose.recoilDuration;
                float recoil = pose.recoilDegrees * Mathf.Sin(rt * Mathf.PI);
                transform.rotation = toQ * Quaternion.Euler(0f, 0f, -recoil);
                rElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = toQ;
        }

        yield return new WaitForSeconds(GetRecoveryTime());

        // Reset global -> relativo al parent
        StartCoroutine(SmoothReset(0.2f));
        isAttacking = false;
    }
}
