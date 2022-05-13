
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;


namespace Juego.unity
{


   #pragma warning disable 649

    /// Administrador del juego.
    /// Se conecta y mira el estado de los fotones, Reproductor instantáneo
    /// Se ocupa de abandonar la sala y el juego
     /// Se ocupa de la carga de nivel (fuera de la sincronización en la sala)
    /// </resumen>
    public class GameManager : MonoBehaviourPunCallbacks
    {

		#region Public Fields

		static public GameManager Instance;

		#endregion

		#region Private Fields

		private GameObject instance;

         //[Tooltip("El prefab para usar para representar al jugador")]
         //[SerializeField]
         //private GameObject playerPrefab;

        #endregion

        #region MonoBehaviour CallBacks


         /// <resumen>
         /// Método MonoBehaviour llamado a GameObject por Unity durante la fase de inicialización.
         /// </resumen>
        void Start()
		{
			Instance = this;

            // en caso de que empecemos esta demostración con la escena incorrecta activa, simplemente cargue la escena del menú
            if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Launcher");

				return;
			}

              //if (playerPrefab == null) { //# Sugerencia Nunca asuma que las propiedades públicas de los Componentes se llenan correctamente, siempre verifique e informe al desarrollador sobre el mismo.

              //Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
              //} else {


                if (PlayerManager.LocalPlayerInstance==null)
				{
				    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    if(PlayerPrefs.GetInt("Character") == 1) {

                       // estamos en una habitación. generar un personaje para el jugador local. se sincroniza utilizando PhotonNetwork.
                       PhotonNetwork.Instantiate("My Robot Kyle", new Vector3(0f,5f,0f), Quaternion.identity, 0);
                    }
                    else if (PlayerPrefs.GetInt("Character") == 2)
                    {
                             // estamos en una habitación. generar un personaje para el jugador local. se sincroniza utilizando PhotonNetwork.
                             PhotonNetwork.Instantiate("My Robot Kyle 2", new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    }
                    else if (PlayerPrefs.GetInt("Character") == 3)
                    {
                            // estamos en una habitación. generar un personaje para el jugador local. se sincroniza utilizando PhotonNetwork.
                            PhotonNetwork.Instantiate("My Robot Kyle 3", new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    }
                    else if (PlayerPrefs.GetInt("Character") == 4)
                    {
                            // estamos en una habitación. generar un personaje para el jugador local. se sincroniza utilizando PhotonNetwork.
                            PhotonNetwork.Instantiate("My Robot Kyle 4", new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    }
                }
                else{

					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				}


			//}

		}


              /// <resumen>
              /// Método MonoBehaviour llamado en GameObject por Unity en cada cuadro.
              /// </resumen>
        void Update()
		{

            // El botón "atrás" del teléfono equivale a "Escape". salir de la aplicación si se presiona
            if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
		}

        #endregion

        #region Photon Callbacks
  
          /// <resumen>
          /// Se llama cuando se conecta un Photon Player. Necesitamos luego cargar una escena más grande.
          /// </resumen>
          /// <param name="other">otro.</param>
        public override void OnPlayerEnteredRoom( Player other  )
		{
			Debug.Log( "OnPlayerEnteredRoom() " + other.NickName); // no se ve si eres el jugador que se conecta

            if ( PhotonNetwork.IsMasterClient )
			{
				Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // llamado antes de OnPlayerLeftRoom


                LoadArena();
			}
		}

           /// <resumen>
           /// Se llama cuando se desconecta un Photon Player. Necesitamos cargar una escena más pequeña.
           /// </resumen>
           /// <param name="other">otro.</param>
        public override void OnPlayerLeftRoom( Player other  )
		{
			Debug.Log( "OnPlayerLeftRoom() " + other.NickName ); // visto cuando otros desconecta


            if ( PhotonNetwork.IsMasterClient )
			{
				Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // llamado antes de OnPlayerLeftRoom

                LoadArena(); 
			}
		}

          /// <resumen>
          /// Llamado cuando el jugador local salió de la sala. Necesitamos cargar la escena del lanzador.
          /// </resumen>
        public override void OnLeftRoom()
		{
			SceneManager.LoadScene("Launcher");
		}

		#endregion

		#region Public Methods

		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		#endregion

		#region Private Methods

		void LoadArena()
		{
			if ( ! PhotonNetwork.IsMasterClient )
			{
				Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
			}

			Debug.LogFormat( "PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount );

			PhotonNetwork.LoadLevel("Room for 1"); //PhotonNetwork.CurrentRoom.PlayerCount
        }

		#endregion

	}
}


