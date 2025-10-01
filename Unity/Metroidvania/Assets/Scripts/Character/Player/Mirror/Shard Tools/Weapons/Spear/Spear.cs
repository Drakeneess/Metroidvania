using System.Collections;
using UnityEngine;

public class Spear : SingleWeapon
{
    protected override void Awake()
    {
        shardToolName = "Spear";
        shardToolDescription = shardToolName;
        base.Awake();

        // 👇 La lanza ahora descansa horizontal, apuntando a +X
        transform.localRotation = Quaternion.identity;

        comboAnimations = new ComboPose[]
        {
            // 0 — Estocada recta rápida
            new ComboPose {
                startRotation = new Vector3(0f, 0f, 0f),
                endRotation   = new Vector3(-6f, 0f, 0f), // leve pitch al empujar
                duration      = 0.22f,
                impactTime    = 0.12f,
                shakeIntensity= 2.2f,
                recoilDegrees = 5f,
                recoilDuration= 0.15f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 1 — Estocada con cruce (yaw izq → der)
            new ComboPose {
                startRotation = new Vector3(0f, -12f, 0f),
                endRotation   = new Vector3(0f,  12f, 0f),
                duration      = 0.28f,
                impactTime    = 0.18f,
                shakeIntensity= 2.5f,
                recoilDegrees = 7f,
                recoilDuration= 0.18f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 2 — Barrido lateral corto (control de espacio)
            new ComboPose {
                startRotation = new Vector3(0f, -35f, -8f),
                endRotation   = new Vector3(0f,  35f,  8f),
                duration      = 0.40f,
                impactTime    = 0.25f,
                shakeIntensity= 3.0f,
                recoilDegrees = 10f,
                recoilDuration= 0.20f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 3 — Estocada final profunda
            new ComboPose {
                startRotation = new Vector3(0f, 0f, 0f),
                endRotation   = new Vector3(-10f, 0f, 0f),
                duration      = 0.36f,
                impactTime    = 0.24f,
                shakeIntensity= 3.6f,
                recoilDegrees = 12f,
                recoilDuration= 0.22f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },

            // 4 — Recall neutro (sin flips)
            new ComboPose {
                startRotation = new Vector3(0f, 0f, 0f),
                endRotation   = new Vector3(0f, 0f, 0f),
                duration      = 0.20f,
                impactTime    = 0.10f,
                shakeIntensity= 0.5f,
                recoilDegrees = 0f,
                recoilDuration= 0f,
                forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true
            },
        };
    }


    public override void HeavyAttack(float chargeFactor)
    {
        if (isAttacking) return;
        StartCoroutine(ThrustRoutine(chargeFactor));
    }

    private IEnumerator ThrustRoutine(float chargeFactor)
    {
        isAttacking = true;

        Vector3 startPos  = transform.localPosition;
        Quaternion startRot = Quaternion.identity; // horizontal a +X

        // Avance hacia +X (más carga → más distancia y más rápido)
        Vector3 fwdPos   = startPos + Vector3.right * Mathf.Lerp(1.1f, 1.9f, chargeFactor);
        float   thrustTime = Mathf.Lerp(0.15f, 0.08f, chargeFactor);
        float   backTime   = 0.20f;

        // Avance inmediato con leve vibración
        float t = 0f;
        while (t < 1f)
        {
            float eased = 1f - (1f - t) * (1f - t);           // ease-out
            float shake = Mathf.Sin(Time.time * 160f) * 0.008f;
            transform.localPosition = Vector3.Lerp(startPos, fwdPos, eased) + new Vector3(0f, shake, 0f);
            // pequeñísimo pitch al empujar
            transform.localRotation = Quaternion.Slerp(startRot, startRot * Quaternion.Euler(-6f, 0f, 0f), eased);
            t += Time.deltaTime / thrustTime;
            yield return null;
        }
        transform.localPosition = fwdPos;
        transform.localRotation = startRot * Quaternion.Euler(-6f, 0f, 0f);

        // Impacto con daño escalado por carga
        ActivateDamageArea(true, chargeFactor);

        // Vuelta a la posición
        t = 0f;
        while (t < 1f)
        {
            float eased = t * t; // ease-in
            transform.localPosition = Vector3.Lerp(fwdPos, startPos, eased);
            transform.localRotation = Quaternion.Slerp(startRot * Quaternion.Euler(-6f, 0f, 0f), startRot, eased);
            t += Time.deltaTime / backTime;
            yield return null;
        }
        transform.localPosition = startPos;
        transform.localRotation = startRot;

        // Recoil: bamboleo leve en roll (Z) — NO “raspa” el suelo
        float recoilTime  = 0.18f;
        float recoilAngle = Mathf.Lerp(6f, 16f, chargeFactor);
        t = 0f;
        while (t < 1f)
        {
            float swing = Mathf.Sin(t * Mathf.PI) * recoilAngle;
            transform.localRotation = startRot * Quaternion.Euler(0f, 0f, swing);
            t += Time.deltaTime / recoilTime;
            yield return null;
        }
        transform.localRotation = startRot;

        yield return new WaitForSeconds(GetRecoveryTime());
        StartCoroutine(SmoothReset(0.2f));
        isAttacking = false;
    }
}
