
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //varible de velocidad del jugador
    public float speed;

    //varible de velocidad de rotacion del jugador
    public float rotationSpeed;

    //varible de gravedad que afecta al jugador
    public float gravity = -9.81f;

    //varible de rotacion actual del jugador
    private float currentRotation;

    //llamr a la clase InputController
    private InputController inputController;

    //llamar al componente CharacterController
    private CharacterController characterController;


    // Start is called before the first frame update
    void Start()
    {
        inputController = GetComponent<InputController>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //llamar al metodo de movimiento del jugador cada frame
        Movement();

        //llamar al metodo de rotacion del jugador cada frame
        Rotation();
    }

    //metodo de movimiento del jugador
    public void Movement()
    {
        //mover hacia adelante y hacia atras al jugador
        Vector3 inputVector = new Vector3(0, 0, inputController.moveVector.y);

        inputVector = transform.TransformDirection(inputVector);

        //mantener gravedad en el jugador sin impactar la velocidad de movimiento
        Vector3 finalMovement = (inputVector * speed) + (Vector3.up * gravity);

        characterController.Move(finalMovement * Time.deltaTime);

    }

    //metodo de rotacion del jugador
    public void Rotation()
    {
        float rotationInput = inputController.moveVector.x * rotationSpeed * Time.deltaTime;
        currentRotation += rotationInput;
        transform.localRotation = Quaternion.AngleAxis(currentRotation, transform.up);
    }
}
