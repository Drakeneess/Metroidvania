using System.Collections;
using UnityEngine;

public class Sword : SingleWeapon
{
    protected override void Awake()
    {
        base.Awake();
        shardToolName = "Sword";
        shardToolDescription = shardToolName;

        comboAnimations = new ComboPose[]
        {
            // 0 — Horizontal sweep ULTRA abierto (barrido de izquierda a derecha)
            new() {
                startRotation = new Vector3(0f, -95f, -35f),
                endRotation   = new Vector3(0f,  95f,  35f),
                duration      = 0.52f,
                impactTime    = 0.38f,
                shakeIntensity= 3.8f,
                recoilDegrees = 16f,
                recoilDuration= 0.24f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 1 — Diagonal ascendente ULTRA abierta
            new() {
                startRotation = new Vector3(-90f, -55f, -20f),
                endRotation   = new Vector3( 65f,  55f,  25f),
                duration      = 0.58f,
                impactTime    = 0.42f,
                shakeIntensity= 4.2f,
                recoilDegrees = 18f,
                recoilDuration= 0.26f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 2 — Diagonal descendente ULTRA abierta
            new() {
                startRotation = new Vector3(90f, -55f, -25f),
                endRotation   = new Vector3(-85f, 55f,  30f),
                duration      = 0.56f,
                impactTime    = 0.42f,
                shakeIntensity= 4.4f,
                recoilDegrees = 18f,
                recoilDuration= 0.26f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 3 — Vertical cleave ULTRA dramático
            new() {
                startRotation = new Vector3(-120f, -20f, 0f),
                endRotation   = new Vector3( 120f,  20f, 0f),
                duration      = 0.70f,
                impactTime    = 0.54f,
                shakeIntensity= 5.4f,
                recoilDegrees = 24f,
                recoilDuration= 0.30f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 4 — Recall más expresivo (vuelve marcando arco)
            new() {
                startRotation = new Vector3(30f, 15f, -15f),
                endRotation   = Vector3.zero,
                duration      = 0.48f,
                impactTime    = 0.28f,
                shakeIntensity= 1.0f,
                recoilDegrees = 8f,
                recoilDuration= 0.20f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },
        };
    }

    // 🔹 Heavy Attack real (Release): estocada
    public override void HeavyAttack(float chargeFactor)
    {
        if (isAttacking) return;
        StartCoroutine(ChargedThrustRoutine(chargeFactor));
    }

    private IEnumerator ChargedThrustRoutine(float chargeFactor)
    {
        isAttacking = true;

        Vector3 startPos = transform.localPosition;
        Vector3 fwdPos   = startPos + Vector3.right * Mathf.Lerp(1.0f, 1.8f, chargeFactor);

        float thrustTime = Mathf.Lerp(0.22f, 0.12f, chargeFactor); 
        float backTime   = 0.28f;

        // 🔹 Avance inmediato
        float t = 0f;
        while (t < 1f)
        {
            float eased = 1f - Mathf.Pow(1f - t, 2f);
            float shake = Mathf.Sin(Time.time * 90f) * 0.02f;
            transform.localPosition = Vector3.Lerp(startPos, fwdPos, eased) + new Vector3(shake, 0f, 0f);
            t += Time.deltaTime / thrustTime;
            yield return null;
        }
        transform.localPosition = fwdPos;

        // 🔹 Impacto con daño escalado
        ActivateDamageArea(true, chargeFactor);

        // 🔹 Retroceso
        t = 0f;
        while (t < 1f)
        {
            float eased = t * t;
            transform.localPosition = Vector3.Lerp(fwdPos, startPos, eased);
            t += Time.deltaTime / backTime;
            yield return null;
        }
        transform.localPosition = startPos;

        // 🔹 Recoil visual extra
        float recoilTime = 0.25f;
        float recoilAngle = Mathf.Lerp(15f, 30f, chargeFactor);
        t = 0f;
        while (t < 1f)
        {
            float swing = Mathf.Sin(t * Mathf.PI) * recoilAngle;
            transform.localRotation = Quaternion.Euler(0f, 0f, swing);
            t += Time.deltaTime / recoilTime;
            yield return null;
        }
        transform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(GetRecoveryTime());
        StartCoroutine(SmoothReset(0.2f));

        isAttacking = false;
    }

    // 🔹 Sobrescribimos la carga para la espada
    public override void PlayChargePose()
    {
        if (isAttacking) return;
        StartCoroutine(SwordChargeRoutine());
    }

    private IEnumerator SwordChargeRoutine()
    {
        isAttacking = true;

        Quaternion startRot = transform.localRotation;
        Quaternion chargeRot = startRot * Quaternion.Euler(0f, 0f, -90f); // espada en vertical

        float chargeTime = 0.35f;
        float t = 0f;
        while (t < 1f)
        {
            transform.localRotation = Quaternion.Slerp(startRot, chargeRot, t);
            t += Time.deltaTime / chargeTime;
            yield return null;
        }
        transform.localRotation = chargeRot;

        isAttacking = false;
    }
}
