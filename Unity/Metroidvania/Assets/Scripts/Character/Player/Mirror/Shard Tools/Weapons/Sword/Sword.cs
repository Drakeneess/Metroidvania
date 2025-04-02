using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    // Variables para control de rotación
    public float attackSpeed = 1.0f;  // Velocidad de interpolación de rotación
    private Quaternion startRotation; // Rotación inicial
    private Quaternion endRotation;   // Rotación final

    protected override void Awake()
    {
        shardToolName = "Sword";
        shardToolDescription = shardToolName;
        base.Awake();
    }

    public override void LightAttack(int comboIndex)
    {
        base.LightAttack(comboIndex);

        Slash(comboIndex);
    }

    public override void HeavyAttack(float value)
    {
        base.HeavyAttack(value);
    }

    private void Slash(int comboIndex)
    {
        // Aquí puedes definir la rotación inicial y final para el primer ataque.
        // Supongamos que quieres girar la espada hacia un ángulo de ataque en el eje Y (puedes cambiar esto según el eje que necesites).
        if(comboIndex==0){
            originalRotation = transform.rotation;

            startRotation = transform.rotation;
            endRotation = Quaternion.Euler(positions[0]);

            SlashAnimation(startRotation, endRotation, 0.1f);
        }
        startRotation = Quaternion.Euler(positions[comboIndex]);
        endRotation = Quaternion.Euler(positions[comboIndex+1]);

        SlashAnimation(startRotation, endRotation, attackSpeed);
    }

    private void SlashAnimation(Quaternion startRot, Quaternion endRot, float animSpeed){

        StartCoroutine(RotateSword(startRot, endRot, animSpeed));  // Iniciar la rotación con un Coroutine
    }

    private void Stab(float value)
    {
        // Código para el ataque de apuñalamiento (puedes agregar más lógica de rotación aquí también si es necesario)
    }

    private IEnumerator RotateSword(Quaternion start, Quaternion end, float speed)
    {

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            //Aqui viene la corrida de toro
            transform.localRotation = Quaternion.Slerp(start, end, elapsedTime);
            elapsedTime += Time.deltaTime * speed;  // Aumentar el tiempo de rotación según la velocidad
            yield return null;  // Esperar al siguiente frame
        }
        transform.localRotation = end;  // Asegurarse de que la rotación final sea exactamente la de destino
    }

}
