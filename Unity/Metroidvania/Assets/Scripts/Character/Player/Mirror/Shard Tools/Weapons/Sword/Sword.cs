using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    public float attackSpeed = 1.0f;

    private Quaternion startRotation;
    private Quaternion endRotation;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Coroutine currentRotationRoutine;
    private bool isAttacking = false;

    protected override void Awake()
    {
        shardToolName = "Sword";
        shardToolDescription = shardToolName;
        base.Awake();
    }

    public override void LightAttack(int comboIndex)
    {
        if (isAttacking || comboIndex >= positions.Length - 1)
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
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(positions[comboIndex + 1]);

        startPosition = transform.localPosition;
        endPosition = startPosition + transform.forward * 0.3f;

        if (currentRotationRoutine != null)
            StopCoroutine(currentRotationRoutine);

        currentRotationRoutine = StartCoroutine(RotateSword(startRotation, endRotation, startPosition, endPosition, attackSpeed));
    }

    private IEnumerator RotateSword(Quaternion startRot, Quaternion endRot, Vector3 startPos, Vector3 endPos, float speed)
    {
        isAttacking = true;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime);

            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        transform.rotation = endRot;
        transform.localPosition = endPos;

        yield return new WaitForSeconds(GetRecoveryTime());

        StartCoroutine(SmoothReset(0.2f)); // Vuelve a la posición original con suavidad
        isAttacking = false;
    }

    private IEnumerator SmoothReset(float duration)
    {
        Quaternion currentRot = transform.rotation;
        Vector3 currentPos = transform.localPosition;

        float t = 0f;
        while (t < 1f)
        {
            transform.rotation = Quaternion.Slerp(currentRot, originalRotation, t);
            transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, t);
            t += Time.deltaTime / duration;
            yield return null;
        }

        transform.rotation = originalRotation;
        transform.localPosition = Vector3.zero;
    }
}
