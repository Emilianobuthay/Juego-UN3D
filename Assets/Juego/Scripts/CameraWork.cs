

using UnityEngine;




/// <resumen>
/// Trabajo de c�mara. Sigue un objetivo
/// </resumen>
public class CameraWork : MonoBehaviour
	{
        #region Private Fields

	    [Tooltip("La distancia en el plano local x-z al objetivo")]
	    [SerializeField]
	    private float distance = 7.0f;

	    [Tooltip("La altura que queremos que la c�mara est� por encima del objetivo")]
	    [SerializeField]
	    private float height = 3.0f;

	    [Tooltip("El retraso de tiempo suave para la altura de la c�mara")]
	    [SerializeField]
	    private float heightSmoothLag = 0.3f;

	    [Tooltip("Permita que la c�mara se desplace verticalmente del objetivo, por ejemplo, para obtener m�s vista de la escena y menos terreno")]
	    [SerializeField]
	    private Vector3 centerOffset = Vector3.zero;

	    [Tooltip("Establezca esto como falso si un componente de un prefabricado est� siendo instanciado por Photon Network, y llame manualmente a OnStartFollowing () cuando y si es necesario")]
	    [SerializeField]
	    private bool followOnStart = false;

        // transformaci�n en cach� del objetivo
        Transform cameraTransform;

        // mantener una bandera interna para volver a conectar si se pierde el objetivo o se cambia la c�mara
        bool isFollowing;

        // Representa la velocidad actual, este valor es modificado por SmoothDamp () cada vez que lo llama.
        private float heightVelocity;

        // Representa la posici�n que estamos tratando de alcanzar usando SmoothDamp ()
        private float targetHeight = 100000.0f;

    #endregion

        #region MonoBehaviour Callbacks


        /// <resumen>
        /// M�todo MonoBehaviour llamado a GameObject por Unity durante la fase de inicializaci�n
        /// </resumen>
        void Start()
		{
            // Comience a seguir el objetivo si lo desea.
            if (followOnStart)
			{
				OnStartFollowing();
			}

		}

        /// <resumen>
        /// Se llama al m�todo MonoBehaviour despu�s de que se hayan llamado todas las funciones de Actualizaci�n. Esto es �til para ordenar la ejecuci�n del script. Por ejemplo, una c�mara de seguimiento siempre debe implementarse en LateUpdate porque rastrea objetos que podr�an haberse movido dentro de Update.
        /// </resumen>
        void LateUpdate()
		{
            // El objetivo de transformaci�n no puede destruir en carga nivelada,
            // as� que debemos cubrir casos de esquina donde la c�mara principal es diferente cada vez que cargamos una nueva escena, y volver a conectarnos cuando eso suceda
            if (cameraTransform == null && isFollowing)
			{
				OnStartFollowing();
			}


            // solo seguir se declara expl�citamente
            if (isFollowing) {
				Apply ();
			}
		}

    #endregion

        #region Public Methods

           /// <resumen>
           /// Eleva el inicio despu�s del evento.
           /// Use esto cuando no sepa en el momento de editar qu� seguir, generalmente instancias administradas por la red de fotones.
           /// </resumen>
        public void OnStartFollowing()
		{	      
			cameraTransform = Camera.main.transform;
			isFollowing = true;

            // no suavizamos nada, vamos directamente a la c�mara correcta
            Cut();
		}

    #endregion
 
        #region Private Methods

          /// <resumen>
          /// Sigue el objetivo sin problemas
          /// </resumen>
        void Apply()
	    {
			Vector3 targetCenter = transform.position + centerOffset;


            // Calcular los �ngulos de rotaci�n actual y objetivo
            float originalTargetAngle = transform.eulerAngles.y;
	        float currentAngle = cameraTransform.eulerAngles.y;

            // Ajusta el �ngulo objetivo real cuando la c�mara est� bloqueada
            float targetAngle = originalTargetAngle;

			currentAngle = targetAngle;

	        targetHeight = targetCenter.y + height;


            // Humedece la altura
            float currentHeight = cameraTransform.position.y;
	        currentHeight = Mathf.SmoothDamp( currentHeight, targetHeight, ref heightVelocity, heightSmoothLag );

            // Convierte el �ngulo en una rotaci�n, por lo que luego reposicionamos la c�mara
            Quaternion currentRotation = Quaternion.Euler( 0, currentAngle, 0 );

            // Establece la posici�n de la c�mara en el plano x-z para:
            // metros de distancia detr�s del objetivo
            cameraTransform.position = targetCenter;
	        cameraTransform.position += currentRotation * Vector3.back * distance;

            // Establecer la altura de la c�mara
            cameraTransform.position = new Vector3( cameraTransform.position.x, currentHeight, cameraTransform.position.z );

            // Siempre mira al objetivo	
            SetUpRotation(targetCenter);
	    }


         /// <resumen>
         /// Posicione directamente la c�mara en el Objetivo y centro especificados.
         /// </resumen>
        void Cut( )
	    {
	        float oldHeightSmooth = heightSmoothLag;
	        heightSmoothLag = 0.001f;

	        Apply();

	        heightSmoothLag = oldHeightSmooth;
	    }

         /// <resumen>
         /// Configura la rotaci�n de la c�mara para estar siempre detr�s del objetivo
         /// </resumen>
         /// <param name="centerPos">Posicion central.</param>
        void SetUpRotation( Vector3 centerPos )
	    {
	        Vector3 cameraPos = cameraTransform.position;
	        Vector3 offsetToCenter = centerPos - cameraPos;

            // Genera rotaci�n de base solo alrededor del eje y-axis
            Quaternion yRotation = Quaternion.LookRotation( new Vector3( offsetToCenter.x, 0, offsetToCenter.z ) );

	        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
	        cameraTransform.rotation = yRotation * Quaternion.LookRotation( relativeOffset );

	    }

		#endregion
	}
