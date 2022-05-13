// --------------------------------------------------------------------------------------------------------------------


// <resumen>
//  conectarse y unirse / crear sala automáticamente
// </resumen>

// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;

namespace Juego.unity
{
#pragma warning disable 649

    /// <resumen>
    /// Gerente de lanzamiento. Conéctese, únase a una sala aleatoria o cree una si no hay ninguna o todas están llenas.
    /// </resumen>
    public class Launcher : MonoBehaviourPunCallbacks
    {

		#region Private Serializable Fields

		[Tooltip("El Panel Ui para permitir que el usuario ingrese el nombre, se conecte y juegue")]
		[SerializeField]
		private GameObject controlPanel;

		[Tooltip("El texto de la interfaz de usuario para informar al usuario sobre el progreso de la conexión")]
		[SerializeField]
		private Text feedbackText;

		[Tooltip("El número máximo de jugadores por habitación.")]
		[SerializeField]
		private byte maxPlayersPerRoom = 10;

		[Tooltip("The UI Loader Anime")]
		[SerializeField]
		private LoaderAnime loaderAnime;

        [Tooltip("The UI Loader Anime")]
        [SerializeField]
        private GameObject paneljuego;



        #endregion

        #region Private Fields

        /// <resumen>
        /// Mantenga un registro del proceso actual. Como la conexión es asíncrona y se basa en varias devoluciones de llamada de Photon,
        /// necesitamos hacer un seguimiento de esto para ajustar adecuadamente el comportamiento cuando recibimos una llamada de Photon.
        /// Normalmente, esto se usa para la devolución de llamada OnConnectedToMaster ().
        /// </resumen>
        bool isConnecting;

        /// <resumen>
        /// El número de versión de este cliente. GameVersion separa a los usuarios entre sí (lo que te permite hacer cambios importantes).
        /// </resumen>
        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks


        /// <resumen>
        /// Método MonoBehaviour llamado en GameObject por Unity durante la fase de inicialización temprana.
        /// </resumen>
        void Awake()
		{
			if (loaderAnime==null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.",this);
			}

            // # Crítico
            // esto asegura que podamos usar PhotonNetwork.LoadLevel () en el cliente maestro y todos los clientes en la misma sala sincronizan su nivel automáticamente
            PhotonNetwork.AutomaticallySyncScene = true;

		}

        #endregion


        #region Public Methods


        /// <resumen>
        /// Inicia el proceso de conexión.
        /// - Si ya está conectado, intentamos unirnos a una sala aleatoria
        /// - si aún no está conectado, conecte esta instancia de aplicación a Photon Cloud Network
        /// </resumen>
        public void Connect()
		{
            // queremos asegurarnos de que el registro esté limpio cada vez que nos conectemos, podríamos tener varios intentos fallidos si falla la conexión.
            feedbackText.text = "";


            // lleva un registro de la voluntad de unirte a una sala, porque cuando volvamos del juego recibiremos una devolución de llamada de que estamos conectados, por lo que debemos saber qué hacer.
            isConnecting = true;


            // oculta el botón Reproducir para obtener consistencia visual
            controlPanel.SetActive(false);


            // inicia la animación del cargador para efectos visuales.
            if (loaderAnime!=null)
			{
				loaderAnime.StartLoaderAnimation();
			}


            // comprobamos si estamos conectados o no, nos unimos si lo estamos, de lo contrario iniciamos la conexión con el servidor.
            if (PhotonNetwork.IsConnected)
			{
				LogFeedback("Joining Room...");
                // # Crítico, necesitamos en este punto intentar unirnos a una sala aleatoria. Si falla, seremos notificados en OnJoinRandomFailed () y crearemos uno.
                PhotonNetwork.JoinRandomRoom();
			}else{

				LogFeedback("Connecting...");


                // # Crítico, primero debemos conectarnos a Photon Online Server.
                PhotonNetwork.GameVersion = this.gameVersion;
				PhotonNetwork.ConnectUsingSettings();
			}
		}


        /// <resumen>
        /// Registra los comentarios en la vista de la interfaz de usuario para el jugador, a diferencia del editor de Unity para el desarrollador.
        /// </resumen>
        /// <param name="message">Message.</param>
        void LogFeedback(string message)
		{

            // no asumimos que hay un feedbackText definido.
            if (feedbackText == null) {
				return;
			}


            // agrega nuevos mensajes como una nueva línea y en la parte inferior del registro.
            feedbackText.text += System.Environment.NewLine+message;
		}


        public void Sobrejuego()
        {
            paneljuego.SetActive(true);
        }

        public void AtrasSobrejuego()
        {
            paneljuego.SetActive(false);
        }

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks

        // a continuación, implementamos algunas devoluciones de llamada de PUN
        // puedes encontrar las devoluciones de llamada de PUN en la clase MonoBehaviourPunCallbacks


        /// <resumen>
        /// Se llama después de que se establece y autentica la conexión al maestro
        /// </resumen>
        public override void OnConnectedToMaster()
		{

            // no queremos hacer nada si no intentamos unirnos a una sala.
            // este caso donde isConnecting es falso es típicamente cuando perdiste o saliste del juego, cuando se cargó este nivel, se llamará a OnConnectedToMaster, en ese caso
            // no queremos hacer nada.
            if (isConnecting)
			{
				LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
				Debug.Log("/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");


                // # Crítico: lo primero que intentamos hacer es unirnos a una sala existente potencial. Si hay, bueno, de lo contrario, seremos llamados de nuevo con OnJoinRandomFailed ()
                PhotonNetwork.JoinRandomRoom();
			}
		}


        /// <resumen>
        /// Se llama cuando falla una llamada a JoinRandom (). El parámetro proporciona ErrorCode y mensaje.
        /// </resumen>
        /// <observaciones>
        /// Lo más probable es que todas las habitaciones estén llenas o no haya habitaciones disponibles. <br/>
        /// </Observaciones>
        public override void OnJoinRandomFailed(short returnCode, string message)
		{
			LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
			Debug.Log("/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");


            // # Crítico: no pudimos unirnos a una sala aleatoria, quizás no exista ninguno o estén todos llenos. No se preocupe, creamos una nueva sala.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
		}



        /// <resumen>
        /// Se llama después de desconectarse del servidor Photon.
        /// </resumen>
        public override void OnDisconnected(DisconnectCause cause)
		{
			LogFeedback("<Color=Red>OnDisconnected</Color> "+cause);
			Debug.LogError("/Launcher:Disconnected");


            // # Crítico: no pudimos conectarnos o nos desconectamos. No hay mucho que podamos hacer. Por lo general, un sistema de interfaz de usuario debe estar en su lugar para permitir que el usuario intente conectarse nuevamente.
            loaderAnime.StopLoaderAnimation();

			isConnecting = false;
			controlPanel.SetActive(true);

		}

        /// <resumen>
        /// Se llama al ingresar a una habitación (al crearla o unirse a ella). Llamado a todos los clientes (incluido el Master Client).
        /// </resumen>
        /// <observaciones>
        /// Este método se usa comúnmente para crear instancias de personajes jugadores.
        /// Si una partida debe iniciarse "activamente", puede llamar a un [PunRPC] (@ ref PhotonView.RPC) activado por la pulsación de un botón del usuario o un temporizador.
        ///
        /// Cuando se llama a esto, generalmente ya puedes acceder a los jugadores existentes en la sala a través de PhotonNetwork.PlayerList.
        /// Además, todas las propiedades personalizadas ya deberían estar disponibles como Room.customProperties. Verifique Room..PlayerCount para averiguar si
        /// hay suficientes jugadores en la sala para comenzar a jugar.
        /// </observaciones>
        public override void OnJoinedRoom()
		{
			LogFeedback("<Color=Green>OnJoinedRoom</Color> with "+PhotonNetwork.CurrentRoom.PlayerCount+" Player(s)");
			Debug.Log("/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

            // # Crítico: solo cargamos si somos el primer jugador, de lo contrario confiamos en PhotonNetwork.AutomaticallySyncScene para sincronizar nuestra escena de instancia.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
			{
				Debug.Log("We load the 'Room for 1' ");

				 // # Crítico
                  // Carga el nivel de la sala
				PhotonNetwork.LoadLevel("Room for 1");

			}
		}

		#endregion
		
	}
}