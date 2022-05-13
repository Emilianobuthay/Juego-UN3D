

using UnityEngine;

namespace Juego.unity
{

    /// <resumen>
    /// Comportamiento simple para animar partículas alrededor para crear un "Ajax Loader" típico. esto es realmente muy importante para informar visualmente al usuario que algo está sucediendo
    /// o mejor decir que la aplicación no está congelada, por lo que una animación de algún tipo ayuda a asegurar al usuario que el sistema está inactivo y bien.
    ///
    /// TODO: se oculta cuando falla la conexión.
    ///
    /// </sresumen>
    public class LoaderAnime : MonoBehaviour {

		#region Public Variables

		[Tooltip("Velocidad angular en grados por segundo")]
		public float speed = 180f;

		[Tooltip("Radio del cargador")]
		public float radius = 1f;

		public GameObject particles;

		#endregion
		
		#region Private Variables

		Vector3 _offset;

		Transform _transform;

		Transform _particleTransform;

		bool _isAnimating;

        #endregion

        #region MonoBehaviour CallBacks


        /// <resumen>
        /// Método MonoBehaviour llamado en GameObject por Unity durante la fase de inicialización temprana.
        /// </resumen>
        void Awake()
		{

            // caché para eficiencia
            _particleTransform = particles.GetComponent<Transform>();
			_transform = GetComponent<Transform>();
		}



        /// <resumen>
        /// Método MonoBehaviour llamado en GameObject por Unity en cada cuadro.
        /// </resumen>
        void Update () {


            // solo nos interesan las partículas giratorias si estamos animando
            if (_isAnimating)
			{

                // rotamos con el tiempo. Time.deltaTime es obligatorio para tener una animación independiente de la velocidad de fotogramas,
                _transform.Rotate(0f,0f,speed*Time.deltaTime);


                // nos movemos desde el centro al radio deseado para evitar que los artefactos visuales de las partículas salten desde su lugar actual, no es muy agradable visualmente
                // entonces la partícula se centra en la escena para que cuando comience a girar, no salte y lentamente la animamos a su radio final dando una transición suave.
                _particleTransform.localPosition = Vector3.MoveTowards(_particleTransform.localPosition, _offset, 0.5f*Time.deltaTime);
			}
		}
        #endregion

        #region Public Methods

        /// <resumen>
        /// Inicia la animación del cargador. Se hace visible
        /// </resumen>
        public void StartLoaderAnimation()
		{
			_isAnimating = true;
			_offset = new Vector3(radius,0f,0f);
			particles.SetActive(true);
		}


        /// <resumen>
        /// Detiene la animación del cargador. Se vuelve invisible
        /// </resumen>
        public void StopLoaderAnimation()
		{
			particles.SetActive(false);
		}

		#endregion
	}
}