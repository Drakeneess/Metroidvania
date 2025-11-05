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

    // ===== NUEVO SISTEMA DE ANIMACIÓN (misma firma / timings) =====
    private IEnumerator RotateToPose(ComboPose pose, Quaternion fromQ, Quaternion toQ)
    {
        isAttacking = true;

        float duration = Mathf.Max(0.0001f, pose.duration);
        float elapsed  = 0f;
        bool impacted  = false;

        // Perfil de velocidad con pico en impactTime (integrado a 's')
        AnimationCurve speedCurve = BuildImpulseCurve(Mathf.Clamp01(pose.impactTime));
        float normArea = ApproximateArea01(speedCurve);            // normalizamos la integral
        normArea = Mathf.Max(1e-4f, normArea);

        float s = 0f;                                              // progreso integrado [0..1]

        while (elapsed < duration)
        {
            // tiempo normalizado [0..1]
            float u = Mathf.Clamp01(elapsed / duration);

            // velocidad normalizada según curva ⇒ integrar a 's'
            float v = speedCurve.Evaluate(u);                      // "rapidez instantánea"
            s += (v / normArea) * (Time.deltaTime / duration);     // integral discreta
            s = Mathf.Clamp01(s);

            // Interpolación global guiada por 's'
            transform.rotation = Quaternion.Slerp(fromQ, toQ, s);

            // Shake en Z (igual que antes, centrado en medio del swing)
            float mid   = 1f - Mathf.Abs(0.5f - s) * 2f;
            float shake = (pose.shakeIntensity > 0f)
                ? pose.shakeIntensity * Mathf.Sin(Time.time * 80f) * mid
                : 0f;
            transform.rotation *= Quaternion.Euler(0f, 0f, shake);

            // Impact timing SIN CAMBIOS: se dispara al cruzar pose.impactTime
            if (!impacted && s >= pose.impactTime)
            {
                impacted = true;
                ActivateDamageArea();
                isAttacking = false; // (se mantiene tu comportamiento original)
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Aseguramos pose final
        transform.rotation = toQ;

        // Recoil en global (idéntico a tu lógica existente)
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

        // Reset global -> relativo al parent (igual que antes)
        StartCoroutine(SmoothReset(0.2f));
        isAttacking = false;
    }

    // ---- Helpers: curva impulso y su integral numérica ----
    // Pico alineado al impactTime; ancho pre/post ajustado para snap de impacto y frenado suave
    private AnimationCurve BuildImpulseCurve(float impactT)
    {
        // Ajustes suaves que podés tunear si querés más snap o más cola
        float pre   = Mathf.Lerp(0.25f, 0.6f, impactT);            // cuánto tarda en despegar
        float post  = Mathf.Lerp(0.25f, 0.4f, 1f - impactT);       // cuánto "cola" deja tras el impacto
        float k1t   = Mathf.Clamp01(impactT * (1f - pre * 0.5f));
        float k3t   = Mathf.Clamp01(impactT + (1f - impactT) * post);

        var curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0f,   0f,  0f,  0.0001f));
        curve.AddKey(new Keyframe(k1t,  0.35f, 1.8f, 1.8f));       // acelera
        curve.AddKey(new Keyframe(impactT, 1.0f, 0f, 0f));         // pico en el impacto
        curve.AddKey(new Keyframe(k3t,  0.45f, -1.6f, -1.6f));     // frena
        curve.AddKey(new Keyframe(1f,   0f, -0.0001f, 0f));        // vuelve a 0
        return curve;
    }

    private float ApproximateArea01(AnimationCurve c, int samples = 64)
    {
        float area = 0f;
        float inv = 1f / samples;
        for (int i = 0; i < samples; i++)
        {
            // muestreamos en el centro de cada subintervalo
            float u = (i + 0.5f) * inv;
            area += c.Evaluate(u) * inv;
        }
        return area;
    }
}
