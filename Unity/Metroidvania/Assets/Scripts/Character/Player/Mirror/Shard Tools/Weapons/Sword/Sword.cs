using System.Collections;
using UnityEngine;

public class Sword : SingleWeapon
{
    [Header("Light Attack - Root Nudge")]
    [SerializeField] private bool enableLightStep = true;
    [SerializeField] private float stepDistance = 0.55f;
    [SerializeField] private float stepDuration = 0.10f;
    [SerializeField] private bool fallbackTranslateRoot = true;

    [Header("Heavy Thrust - Física Integrada")]
    [SerializeField] private float heavyAnticipationBackMin = 0.12f;
    [SerializeField] private float heavyAnticipationBackMax = 0.30f;
    [SerializeField] private float heavyAnticipationTime   = 0.10f;

    [SerializeField] private float heavyForwardMin = 1.00f;
    [SerializeField] private float heavyForwardMax = 1.80f;
    [SerializeField] private float heavyAccelBase  = 30f;
    [SerializeField] private float heavyAccelBoost = 35f;
    [SerializeField] private float heavyDamping    = 10f;
    [SerializeField] private float heavyBackFrac   = 0.35f;
    [SerializeField] private float heavyBackTime   = 0.24f;

    [Header("Hit Stop (opcional)")]
    [SerializeField] private bool  useHitStop       = true;
    [SerializeField] private float hitStopDuration  = 0.055f;
    [SerializeField] private float hitStopTimeScale = 0.0f;

    // ---- FIX: separar flags / corrutinas ----
    private bool isChargingPose = false;            // SOLO para la pose de carga
    private bool isPerformingThrust = false;        // SOLO para la estocada física
    private Coroutine chargeRoutineCo = null;       // handler para poder cancelar
    private Coroutine thrustRoutineCo = null;

    protected override void Awake()
    {
        shardToolName = "Sword";
        shardToolDescription = shardToolName;
        base.Awake();

        comboAnimations = new ComboPose[]
        {
            new() { startRotation = new Vector3(0f, -95f, -35f), endRotation = new Vector3(0f, 95f, 35f),
                    duration = 0.48f, impactTime = 0.34f, shakeIntensity = 3.8f,
                    recoilDegrees = 16f, recoilDuration = 0.24f, forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true },

            new() { startRotation = new Vector3(-90f, -55f, -20f), endRotation = new Vector3(65f, 55f, 25f),
                    duration = 0.54f, impactTime = 0.36f, shakeIntensity = 4.2f,
                    recoilDegrees = 18f, recoilDuration = 0.26f, forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true },

            new() { startRotation = new Vector3(90f, -55f, -25f), endRotation = new Vector3(-85f, 55f, 30f),
                    duration = 0.52f, impactTime = 0.36f, shakeIntensity = 4.4f,
                    recoilDegrees = 18f, recoilDuration = 0.26f, forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true },

            new() { startRotation = new Vector3(-120f, -20f, 0f), endRotation = new Vector3(120f, 20f, 0f),
                    duration = 0.64f, impactTime = 0.42f, shakeIntensity = 5.4f,
                    recoilDegrees = 24f, recoilDuration = 0.30f, forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true },

            new() { startRotation = new Vector3(30f, 15f, -15f), endRotation = Vector3.zero,
                    duration = 0.44f, impactTime = 0.26f, shakeIntensity = 1.0f,
                    recoilDegrees = 8f, recoilDuration = 0.20f, forceDeltaX = true, forceDeltaY = true, forceDeltaZ = true },
        };
    }

    // ========= Light Attack (igual + root nudge) =========
    public override void LightAttack(int comboIndex)
    {
        if (isPerformingThrust || !gameObject.activeInHierarchy) return;

        base.LightAttack(comboIndex);

        if (enableLightStep && comboIndex >= 0 && comboIndex < comboAnimations.Length)
            StartCoroutine(StepAtImpactRoutine(comboAnimations[comboIndex]));
    }

    private IEnumerator StepAtImpactRoutine(ComboPose pose)
    {
        float wait = Mathf.Max(0f, pose.duration * Mathf.Clamp01(pose.impactTime) - 0.001f);
        yield return new WaitForSeconds(wait);

        float elapsed = 0f;
        int facing = GetFacingDirection();
        Vector3 axis = Vector3.right * facing;

        while (elapsed < stepDuration)
        {
            float t = elapsed / Mathf.Max(0.0001f, stepDuration);
            float vel01 = 1f - Mathf.Abs(2f * t - 1f);
            float speed = (stepDistance / stepDuration) * vel01;
            Vector3 delta = axis * speed * Time.deltaTime;

            ApplyRootDisplacement(delta);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void ApplyRootDisplacement(Vector3 delta)
    {
        if (combatController != null)
        {
            combatController.SendMessage("ExternalTranslate", delta, SendMessageOptions.DontRequireReceiver);
            combatController.SendMessage("AddImpulse", delta, SendMessageOptions.DontRequireReceiver);
            combatController.SendMessage("ApplyRootImpulse", delta, SendMessageOptions.DontRequireReceiver);
        }
        else if (fallbackTranslateRoot && transform.root != null)
        {
            transform.root.Translate(delta, Space.World);
        }
    }

    // ========= Heavy Attack (FIX: no bloquear por pose de carga) =========
    public override void HeavyAttack(float chargeFactor)
    {
        // ⛔ NO uses 'isAttacking' para bloquear aquí; la pose podría estar activa.
        if (isPerformingThrust) return; // solo bloquea si ya hay una estocada en curso

        // Si aún estamos en la pose de carga, la cancelamos para iniciar el thrust
        if (isChargingPose && chargeRoutineCo != null)
        {
            StopCoroutine(chargeRoutineCo);
            chargeRoutineCo = null;
            isChargingPose = false;
        }

        // Arranca la estocada física
        if (thrustRoutineCo != null) StopCoroutine(thrustRoutineCo);
        thrustRoutineCo = StartCoroutine(ChargedThrustRoutine_Physical(chargeFactor));
    }

    private IEnumerator ChargedThrustRoutine_Physical(float chargeFactor)
    {
        isPerformingThrust = true;

        chargeFactor = Mathf.Clamp01(chargeFactor);
        int facing = GetFacingDirection();
        Vector3 axis = Vector3.right * facing;

        // 1) Anticipación
        float antiDist = Mathf.Lerp(heavyAnticipationBackMin, heavyAnticipationBackMax, chargeFactor);
        yield return Anticipation(axis, -antiDist, heavyAnticipationTime);

        // 2) Empuje
        float target = Mathf.Lerp(heavyForwardMin, heavyForwardMax, chargeFactor);
        float accel  = heavyAccelBase + heavyAccelBoost * chargeFactor;
        float vel    = 0f;
        float pos    = 0f;
        bool impacted = false;

        while (pos < target)
        {
            vel += accel * Time.deltaTime;
            vel -= vel * heavyDamping * Time.deltaTime;

            float step = vel * Time.deltaTime;
            pos += step;

            ApplyRootDisplacement(axis * step);

            if (!impacted && pos >= target * 0.60f)
            {
                impacted = true;
                ActivateDamageArea(true, chargeFactor);

                if (useHitStop)
                    yield return HitStop(hitStopDuration, hitStopTimeScale);
            }

            yield return null;
        }

        // 3) Retroceso parcial
        float backDist = Mathf.Min(target * heavyBackFrac, 0.6f);
        float backElapsed = 0f;
        float backT = Mathf.Max(heavyBackTime, 0.01f);
        while (backElapsed < backT)
        {
            float t = backElapsed / backT;
            float vel01 = Mathf.Sin(t * Mathf.PI);
            float step = (backDist / backT) * vel01 * Time.deltaTime;
            ApplyRootDisplacement(-axis * step);
            backElapsed += Time.deltaTime;
            yield return null;
        }

        // 4) Recoil visual
        float recoilTime = 0.25f;
        float recoilAngle = Mathf.Lerp(15f, 30f, chargeFactor);
        float tRecoil = 0f;
        while (tRecoil < 1f)
        {
            float swing = Mathf.Sin(tRecoil * Mathf.PI) * recoilAngle;
            transform.localRotation = Quaternion.Euler(0f, 0f, swing);
            tRecoil += Time.deltaTime / recoilTime;
            yield return null;
        }
        transform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(GetRecoveryTime());
        StartCoroutine(SmoothReset(0.2f));

        isPerformingThrust = false;
        thrustRoutineCo = null;
        // No llamo EndCombo aquí: tu BaseAttack abre ventana y el CombatController la cierra.
    }

    private IEnumerator Anticipation(Vector3 axis, float distance, float time)
    {
        float elapsed = 0f;
        float T = Mathf.Max(0.0001f, time);
        while (elapsed < T)
        {
            float t = elapsed / T;
            float vel01 = 1f - Mathf.Abs(2f * t - 1f);
            float step = (Mathf.Abs(distance) / T) * vel01 * Time.deltaTime;
            ApplyRootDisplacement(axis * Mathf.Sign(distance) * step);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator HitStop(float duration, float timescale)
    {
        float prev = Time.timeScale;
        Time.timeScale = Mathf.Clamp(timescale, 0f, 1f);
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration));
        Time.timeScale = prev;
    }

    // ========= Pose de carga (FIX: NO usar isAttacking global) =========
    public override void PlayChargePose()
    {
        // Permitimos relanzar la pose reiniciándola
        if (chargeRoutineCo != null) StopCoroutine(chargeRoutineCo);
        chargeRoutineCo = StartCoroutine(SwordChargeRoutine());
    }

    private IEnumerator SwordChargeRoutine()
    {
        isChargingPose = true;

        Quaternion startRot = transform.localRotation;
        Quaternion chargeRot = startRot * Quaternion.Euler(0f, 0f, -90f);

        float chargeTime = 0.35f;
        float t = 0f;
        while (t < 1f)
        {
            transform.localRotation = Quaternion.Slerp(startRot, chargeRot, t);
            t += Time.deltaTime / chargeTime;
            yield return null;
        }
        transform.localRotation = chargeRot;

        isChargingPose = false;
        chargeRoutineCo = null;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        // Limpieza defensiva
        if (chargeRoutineCo != null) { StopCoroutine(chargeRoutineCo); chargeRoutineCo = null; }
        if (thrustRoutineCo != null) { StopCoroutine(thrustRoutineCo); thrustRoutineCo = null; }
        isChargingPose = false;
        isPerformingThrust = false;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }
}
