using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class ProceduralTerrainBackground : MonoBehaviour
{
    [Header("Seed persistente")]
    public int seed = 0;
    public bool generarNuevaSeed = false;

    [Header("Altura")]
    public float alturaMaxima = 10f;
    public float escalaNoise = 0.05f;

    [Header("Exclusión central")]
    public int exclusionInicioX = 100;
    public int exclusionFinX = 150;

    [Header("Transición suave")]
    public int falloffRange = 20; // Cuánto suavizar antes/después de la zona jugable

    [Header("Montaña icónica")]
    public float alturaMontanaIconica = 20f;

    private Terrain terreno;
    private TerrainData datos;

    void Start()
    {
        //generarNuevaSeed = !SaveDataController.AreSavedData();
        terreno = GetComponent<Terrain>();
        datos = terreno.terrainData;

        if (generarNuevaSeed)
            seed = System.Guid.NewGuid().GetHashCode();

        Random.InitState(seed);
        GenerarTerreno();
    }

    void GenerarTerreno()
    {
        int width = datos.heightmapResolution;
        int height = datos.heightmapResolution;

        float[,] alturas = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nx = x * escalaNoise;
                float ny = y * escalaNoise;

                // Base redondeada con combinación de ruidos suaves
                float base1 = Mathf.PerlinNoise(nx * 0.3f, ny * 0.3f);
                float base2 = Mathf.PerlinNoise(nx * 0.1f + 100, ny * 0.1f + 100);
                float baseSuave = Mathf.Lerp(base1, base2, 0.6f);

                float smooth = Mathf.SmoothStep(0f, 1f, baseSuave);

                // Pulso orgánico en el fondo
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(width / 2f, height / 2f));
                float pulso = Mathf.Sin(dist * 0.01f + seed % 100) * 0.08f;

                float alturaFinal = (smooth + pulso) * (alturaMaxima / datos.size.y);
                alturaFinal = Mathf.Clamp01(alturaFinal);

                int zonaInicio = exclusionInicioX - falloffRange;
                int zonaFin = exclusionFinX + falloffRange;

                if (x >= zonaInicio && x <= zonaFin)
                {
                    float t;

                    if (x < exclusionInicioX)
                    {
                        // Transición de entrada a la zona jugable
                        t = Mathf.InverseLerp(zonaInicio, exclusionInicioX, x); // 0 a 1
                        t = Mathf.SmoothStep(1f, 0f, t);
                        alturas[x, y] = alturaFinal * t;
                    }
                    else if (x > exclusionFinX)
                    {
                        // Transición de salida de la zona jugable
                        t = Mathf.InverseLerp(exclusionFinX, zonaFin, x); // 0 a 1
                        t = Mathf.SmoothStep(0f, 1f, t);
                        alturas[x, y] = alturaFinal * t;
                    }
                    else
                    {
                        // Zona jugable plana
                        alturas[x, y] = 0f;
                    }
                }
                else
                {
                    alturas[x, y] = alturaFinal;
                }
            }
        }

        // Montaña icónica del jugador (puede mantenerse o comentarse)
        AgregarVallePrincipal(ref alturas, width, height);
        AgregarAccidentesGeograficos(ref alturas, width, height);
        PintarMontañaIconica(ref alturas, width, height);

        datos.SetHeights(0, 0, alturas);
    }

    void PintarMontañaIconica(ref float[,] alturas, int width, int height)
    {
        int inicioX = (int)(width * 0.5f);    // Donde empieza la montaña
        int finX = (int)(width * 0.9f);       // Hasta dónde llega
        int inicioY = (int)(height * 0.2f);
        int finY = (int)(height * 0.7f);

        for (int px = inicioX; px < finX; px++)
        {
            for (int py = inicioY; py < finY; py++)
            {
                float deformacion1 = Mathf.PerlinNoise((px + seed * 2) * 0.06f, (py - seed * 2) * 0.06f);
                float deformacion2 = Mathf.PerlinNoise((px - seed) * 0.1f, (py + seed) * 0.1f);
                float mezcla = Mathf.Lerp(deformacion1, deformacion2, 0.5f);

                // Envolvimiento suave: altura más baja en bordes del área
                float factorX = Mathf.InverseLerp(inicioX, finX, px);
                float factorY = Mathf.InverseLerp(inicioY, finY, py);
                float envoltura = Mathf.SmoothStep(1f, 0f, Mathf.Abs(factorX - 0.5f) * 2f) *
                                Mathf.SmoothStep(1f, 0f, Mathf.Abs(factorY - 0.5f) * 2f);

                float alturaExtra = mezcla * envoltura * (alturaMontanaIconica / datos.size.y);
                alturas[px, py] += alturaExtra;
                alturas[px, py] = Mathf.Clamp01(alturas[px, py]);
            }
        }
    }
    void AgregarAccidentesGeograficos(ref float[,] alturas, int width, int height)
    {
        int cantidad = 8; // Número de accidentes a generar
        float escalaAccidente = 0.03f;
        float alturaAccidente = alturaMaxima * 1.5f;

        for (int i = 0; i < cantidad; i++)
        {
            // Posición aleatoria lejos de la zona jugable
            int centroX = Random.Range((int)(width * 0.1f), (int)(width * 0.9f));
            int centroY = Random.Range((int)(height * 0.2f), (int)(height * 0.8f));

            int radio = Random.Range(10, 25);

            // Verificación completa: si cualquier parte del accidente invade la zona jugable, se descarta
            int minX = centroX - radio;
            int maxX = centroX + radio;

            if (maxX >= exclusionInicioX && minX <= exclusionFinX)
                continue;

            for (int x = -radio; x <= radio; x++)
            {
                for (int y = -radio; y <= radio; y++)
                {
                    int px = centroX + x;
                    int py = centroY + y;

                    if (px < 0 || py < 0 || px >= width || py >= height)
                        continue;

                    float distNorm = Mathf.Sqrt(x * x + y * y) / (float)radio;
                    if (distNorm > 1f) continue;

                    float ruido = Mathf.PerlinNoise(px * escalaAccidente, py * escalaAccidente);
                    float forma = Mathf.SmoothStep(1f, 0f, distNorm) * ruido;

                    // Alternamos entre elevación y hundimiento para variedad
                    float signo = (i % 2 == 0) ? 1f : -1f;

                    float alturaModificada = forma * signo * (alturaAccidente / datos.size.y);
                    alturas[px, py] += alturaModificada;
                    alturas[px, py] = Mathf.Clamp01(alturas[px, py]);
                }
            }
        }
    }

    void AgregarVallePrincipal(ref float[,] alturas, int width, int height)
    {
        int centroY = (int)(height * 0.5f); // Altura central del valle
        int grosor = 20;                    // Grosor vertical del valle
        float profundidad = alturaMaxima * 2.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = -grosor; y <= grosor; y++)
            {
                int py = centroY + y;

                if (py < 0 || py >= height)
                    continue;

                float dist = Mathf.Abs(y) / (float)grosor;
                float falloff = Mathf.SmoothStep(1f, 0f, dist); // Bordes suaves

                float alturaRestada = falloff * (profundidad / datos.size.y);
                alturas[x, py] -= alturaRestada;
                alturas[x, py] = Mathf.Clamp01(alturas[x, py]);
            }
        }
    }
}
