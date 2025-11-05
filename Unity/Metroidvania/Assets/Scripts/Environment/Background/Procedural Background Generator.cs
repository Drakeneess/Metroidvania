using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class ProceduralTerrainBackground : MonoBehaviour
{
    [Header("Seed persistente")]
    public int seed = 0;                 // Seed manual (solo si quieres forzar)
    public bool overrideSeed = false;    // Si true, usa 'seed' en lugar de student+playthrough

    [Header("Altura")]
    public float alturaMaxima = 10f;
    public float escalaNoise = 0.05f;

    [Header("Exclusión central (zona jugable)")]
    public int exclusionInicioX = 100;
    public int exclusionFinX = 150;

    [Header("Transición suave")]
    public int falloffRange = 20;

    [Header("Montaña icónica")]
    public float alturaMontanaIconica = 20f;

    // Unity refs
    private Terrain terreno;
    private TerrainData datos;

    // Threading
    private volatile bool _isGenerating = false;
    private volatile bool _applyPending = false;
    private float[,] _heightsBuffer;

    // Para evitar suscripciones múltiples
    private bool _subscribed = false;

    private void Start()
    {
        terreno = GetComponent<Terrain>();
        datos = terreno.terrainData;

        // Modo “segundo arranque”: ya hay IDs en SaveData → generar ya
        if (
            SaveDataController.AreSavedData() &&
            SaveDataController.Instance.saveData.studentID != -1 &&
            SaveDataController.Instance.saveData.playthroughID != -1)
        {
            int sid = SaveDataController.Instance.saveData.studentID;
            int pid = SaveDataController.Instance.saveData.playthroughID;
            StartGenerationWithIds(sid, pid);
        }

        // Primera ejecución: no hay IDs → esperamos el evento del PlaythroughManager
        SubscribePlaythroughEvent();
    }

    private void OnDestroy()
    {
        UnsubscribePlaythroughEvent();
    }

    private void SubscribePlaythroughEvent()
    {
        if (_subscribed) return;
        PlaythroughManager.OnPlaythroughReady += OnPlaythroughReady;
        _subscribed = true;
    }

    private void UnsubscribePlaythroughEvent()
    {
        if (!_subscribed) return;
        PlaythroughManager.OnPlaythroughReady -= OnPlaythroughReady;
        _subscribed = false;
    }

    private void OnPlaythroughReady(int studentId, int playthroughId)
    {
        // Solo generamos cuando no hay Save al iniciar; si ya generaste arriba, esto será no-op si está ocupado
        StartGenerationWithIds(studentId, playthroughId);
    }

    private void StartGenerationWithIds(int studentId, int playthroughId)
    {
        if (_isGenerating) return;

        int effectiveSeed;
        if (overrideSeed)
        {
            effectiveSeed = seed;
        }
        else
        {
            // Seed determinista a partir de Student + Playthrough (evita overflow)
            unchecked
            {
                // hash simple y seguro
                int h = 17;
                h = h * 31 + studentId;
                h = h * 31 + playthroughId;
                effectiveSeed = h;
            }
        }
        print(effectiveSeed);
        // Capturamos parámetros que NO son de Unity para el thread
        int width = datos.heightmapResolution;
        int height = datos.heightmapResolution;
        float sizeY = datos.size.y;

        float p_alturaMax = alturaMaxima;
        float p_escalaNoise = escalaNoise;
        int p_exIni = exclusionInicioX;
        int p_exFin = exclusionFinX;
        int p_falloff = falloffRange;
        float p_altMont = alturaMontanaIconica;

        _isGenerating = true;

        Task.Run(() =>
        {
            try
            {
                var heights = GenerateHeights(
                    width, height, sizeY,
                    p_alturaMax, p_escalaNoise,
                    p_exIni, p_exFin, p_falloff,
                    p_altMont, effectiveSeed
                );

                // Poner el resultado para aplicar en el main thread
                _heightsBuffer = heights;
                _applyPending = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[TerrainGen] Error en generación: {e}");
            }
            finally
            {
                _isGenerating = false;
            }
        });
    }

    private void Update()
    {
        // Aplicamos en main thread cuando el buffer está listo
        if (_applyPending && _heightsBuffer != null)
        {
            try
            {
                datos.SetHeights(0, 0, _heightsBuffer);
                // Limpiar flags
                _applyPending = false;
                _heightsBuffer = null;
                Debug.Log("🌄 Terreno procedural aplicado.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[TerrainGen] Error al aplicar SetHeights: {e}");
                // Evitamos loop infinito si algo falla
                _applyPending = false;
                _heightsBuffer = null;
            }
        }
    }

    // =================== GENERACIÓN EN BACKGROUND ===================

    private float[,] GenerateHeights(
        int width, int height, float sizeY,
        float alturaMax, float escala,
        int exIni, int exFin, int falloff,
        float altMontIcon, int genSeed)
    {
        System.Random rng = new System.Random(genSeed);

        var alturas = new float[width, height];

        // Base procedural
        for (int x = 0; x < width; x++)
        {
            // precálculo de zona jugable + falloff
            int zonaInicio = exIni - falloff;
            int zonaFin = exFin + falloff;

            for (int y = 0; y < height; y++)
            {
                float nx = x * escala;
                float ny = y * escala;

                // Dos bases de ruido suaves mezcladas
                float base1 = Mathf.PerlinNoise(nx * 0.3f, ny * 0.3f);
                float base2 = Mathf.PerlinNoise(nx * 0.1f + 100f, ny * 0.1f + 100f);
                float baseSuave = Mathf.Lerp(base1, base2, 0.6f);
                float smooth = Mathf.SmoothStep(0f, 1f, baseSuave);

                // Pulso orgánico suave
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(width * 0.5f, height * 0.5f));
                float pulso = Mathf.Sin(dist * 0.01f + (genSeed % 100)) * 0.08f;

                float alturaFinal = (smooth + pulso) * (alturaMax / sizeY);
                alturaFinal = Mathf.Clamp01(alturaFinal);

                if (x >= zonaInicio && x <= zonaFin)
                {
                    float t;
                    if (x < exIni)
                    {
                        t = Mathf.InverseLerp(zonaInicio, exIni, x);
                        t = Mathf.SmoothStep(1f, 0f, t);
                        alturas[x, y] = alturaFinal * t;
                    }
                    else if (x > exFin)
                    {
                        t = Mathf.InverseLerp(exFin, zonaFin, x);
                        t = Mathf.SmoothStep(0f, 1f, t);
                        alturas[x, y] = alturaFinal * t;
                    }
                    else
                    {
                        // zona jugable plana
                        alturas[x, y] = 0f;
                    }
                }
                else
                {
                    alturas[x, y] = alturaFinal;
                }
            }
        }

        // Detalles adicionales (idénticos a tus métodos, pero sin tocar UnityObjects)
        AgregarVallePrincipal(ref alturas, width, height, sizeY, alturaMax);
        AgregarAccidentesGeograficos(ref alturas, width, height, sizeY, alturaMax, exIni, exFin, 8, 0.03f, rng);
        PintarMontanaIconica(ref alturas, width, height, sizeY, altMontIcon, genSeed);

        return alturas;
    }

    private void PintarMontanaIconica(ref float[,] alturas, int width, int height, float sizeY, float altMontIcon, int genSeed)
    {
        int inicioX = (int)(width * 0.5f);
        int finX = (int)(width * 0.9f);
        int inicioY = (int)(height * 0.2f);
        int finY = (int)(height * 0.7f);

        for (int px = inicioX; px < finX; px++)
        {
            for (int py = inicioY; py < finY; py++)
            {
                float deformacion1 = Mathf.PerlinNoise((px + genSeed * 2) * 0.06f, (py - genSeed * 2) * 0.06f);
                float deformacion2 = Mathf.PerlinNoise((px - genSeed) * 0.1f, (py + genSeed) * 0.1f);
                float mezcla = Mathf.Lerp(deformacion1, deformacion2, 0.5f);

                float factorX = Mathf.InverseLerp(inicioX, finX, px);
                float factorY = Mathf.InverseLerp(inicioY, finY, py);
                float envoltura =
                    Mathf.SmoothStep(1f, 0f, Mathf.Abs(factorX - 0.5f) * 2f) *
                    Mathf.SmoothStep(1f, 0f, Mathf.Abs(factorY - 0.5f) * 2f);

                float alturaExtra = mezcla * envoltura * (altMontIcon / sizeY);
                alturas[px, py] = Mathf.Clamp01(alturas[px, py] + alturaExtra);
            }
        }
    }

    private void AgregarAccidentesGeograficos(ref float[,] alturas, int width, int height, float sizeY, float alturaMax,
                                          int exIni, int exFin, int cantidad, float escalaAccidente, System.Random rng)
    {
        float alturaAccidente = alturaMax * 1.5f;

        for (int i = 0; i < cantidad; i++)
        {
            int centroX = rng.Next((int)(width * 0.1f), (int)(width * 0.9f));
            int centroY = rng.Next((int)(height * 0.2f), (int)(height * 0.8f));
            int radio   = rng.Next(10, 25);



            int minX = centroX - radio;
            int maxX = centroX + radio;
            if (maxX >= exIni && minX <= exFin) continue; // evitar invadir zona jugable

            for (int x = -radio; x <= radio; x++)
            {
                for (int y = -radio; y <= radio; y++)
                {
                    int px = centroX + x;
                    int py = centroY + y;
                    if (px < 0 || py < 0 || px >= width || py >= height) continue;

                    float distNorm = Mathf.Sqrt(x * x + y * y) / (float)radio;
                    if (distNorm > 1f) continue;

                    float ruido = Mathf.PerlinNoise(px * escalaAccidente, py * escalaAccidente);
                    float forma = Mathf.SmoothStep(1f, 0f, distNorm) * ruido;

                    float signo = (i % 2 == 0) ? 1f : -1f;
                    float alturaModificada = forma * signo * (alturaAccidente / sizeY);

                    alturas[px, py] = Mathf.Clamp01(alturas[px, py] + alturaModificada);
                }
            }
        }
    }

    private void AgregarVallePrincipal(ref float[,] alturas, int width, int height, float sizeY, float alturaMax)
    {
        int centroY = (int)(height * 0.5f);
        int grosor = 20;
        float profundidad = alturaMax * 2.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = -grosor; y <= grosor; y++)
            {
                int py = centroY + y;
                if (py < 0 || py >= height) continue;

                float dist = Mathf.Abs(y) / (float)grosor;
                float falloff = Mathf.SmoothStep(1f, 0f, dist);

                float alturaRestada = falloff * (profundidad / sizeY);
                alturas[x, py] = Mathf.Clamp01(alturas[x, py] - alturaRestada);
            }
        }
    }
}
