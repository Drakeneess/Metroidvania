using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    private Quaternion startRotation;
    private Quaternion endRotation;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Coroutine currentRotationRoutine;

    protected override void Awake()
    {
        shardToolName = "Sword";
        shardToolDescription = shardToolName;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void LightAttack(int comboIndex)
    {
        if (isAttacking)
            return;

        base.LightAttack(comboIndex);
        Slash(comboIndex);
    }

    public override void HeavyAttack(float value)
    {
        if (isAttacking) return;

        base.HeavyAttack(value);
        // Puedes agregar lógica similar a Slash aquí si lo deseas
    }

    private void Slash(int comboIndex)
    {
        if (comboIndex >= comboAnimations.Length) return;

        var pose = comboAnimations[comboIndex];

        Quaternion startRot = Quaternion.Euler(pose.startRotation);
        Quaternion endRot = Quaternion.Euler(pose.endRotation);

        if (currentRotationRoutine != null)
            StopCoroutine(currentRotationRoutine);

        currentRotationRoutine = StartCoroutine(RotateToPose(startRot, endRot, pose.duration, pose.rotationCurve, pose.shakeIntensity));
    }

    private IEnumerator RotateToPose(Quaternion startRot, Quaternion endRot, float duration, AnimationCurve curve, float shakeIntensity)
    {
        isAttacking = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float evaluatedT = curve != null ? curve.Evaluate(t) : t;

            Quaternion baseRot = Quaternion.Slerp(startRot, endRot, evaluatedT);

            float shake = shakeIntensity * Mathf.Sin(Time.time * 80f) * (1f - Mathf.Abs(0.5f - t) * 2f); // Shake más fuerte en el medio
            Quaternion shakeOffset = Quaternion.Euler(0f, 0f, shake);

            transform.localRotation = baseRot * shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;

        yield return new WaitForSeconds(GetRecoveryTime());

        StartCoroutine(SmoothReset(0.2f));
        isAttacking = false;
    }
}
