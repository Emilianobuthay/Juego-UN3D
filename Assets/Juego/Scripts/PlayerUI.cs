// --------------------------------------------------------------------------------------------------------------------
// <resumen>
// tratar con la pantalla de la interfaz de usuario de la instancia de jugador en red que sigue a un jugador determinado para mostrar su estado y nombre
// </resumen>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Juego.unity
{
    #pragma warning disable 649

    /// <resumen>
    /// IU del jugador. Restrinja la interfaz de usuario para seguir un GameObject PlayerManager en el mundo,
    /// Afecta un control deslizante y un texto para mostrar el nombre y la salud del jugador
    /// </resumen>
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

	    [Tooltip("Desplazamiento de píxeles desde el objetivo del jugador")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

	    [Tooltip("Texto de la interfaz de usuario para mostrar el nombre del jugador")]
	    [SerializeField]
	    private Text playerNameText;

	    [Tooltip("Control deslizante de la interfaz de usuario para mostrar la salud del jugador")]
	    [SerializeField]
	    private Slider playerHealthSlider;

        PlayerManager target;

		float characterControllerHeight;

		Transform targetTransform;

		Renderer targetRenderer;

	    CanvasGroup _canvasGroup;
	    
		Vector3 targetPosition;

        #endregion

        #region MonoBehaviour Messages

        /// <resumen>
        /// Método MonoBehaviour llamado en GameObject por Unity durante la fase de inicialización temprana
        /// </resumen>
        void Awake()
		{

			_canvasGroup = this.GetComponent<CanvasGroup>();
			
			this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
		}

        /// <resumen>
        /// Método MonoBehaviour llamado en GameObject por Unity en cada cuadro.
        /// actualiza el control deslizante de salud para reflejar la salud del jugador
        /// </resumen>
        void Update()
		{
            // Destruirse a sí mismo si el objetivo es nulo, es a prueba de fallas cuando Photon está destruyendo instancias de un jugador a través de la red
            if (target == null) {
				Destroy(this.gameObject);
				return;
			}


            // Refleja la salud del jugador
            if (playerHealthSlider != null) {
				playerHealthSlider.value = target.Health;
			}
		}


        /// <resumen>
        /// Se llama al método MonoBehaviour después de que se hayan llamado todas las funciones de Actualización. Esto es útil para ordenar la ejecución del script.
        /// En nuestro caso, dado que estamos siguiendo un GameObject en movimiento, debemos proceder después de que el jugador se haya movido durante un cuadro en particular.
        /// </resumen>
        void LateUpdate () {

            // No muestre la IU si no somos visibles para la cámara, evite posibles errores al ver la IU, pero no el reproductor en sí.
            if (targetRenderer!=null)
			{
				this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
			}


            // # Crítico
            // Sigue el Target GameObject en la pantalla.
            if (targetTransform!=null)
			{
				targetPosition = targetTransform.position;
				targetPosition.y += characterControllerHeight;
				
				this.transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
			}

		}




        #endregion

        #region Public Methods

        /// <resumen>
        /// Asigna un objetivo de jugador para seguir y representar.
        /// </resumen>
        /// <param name="target">Target.</param>
        public void SetTarget(PlayerManager _target){

			if (_target == null) {
				Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
				return;
			}


            // Caché de referencias para eficiencia porque las vamos a reutilizar.
            this.target = _target;
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponentInChildren<Renderer>();


            CharacterController _characterController = this.target.GetComponent<CharacterController> ();

            // Obtenga datos del reproductor que no cambiarán durante la vida útil de este componente
            if (_characterController != null){
				characterControllerHeight = _characterController.height;
			}

			if (playerNameText != null) {
                playerNameText.text = this.target.photonView.Owner.NickName;
			}
		}

		#endregion

	}
}