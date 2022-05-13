// --------------------------------------------------------------------------------------------------------------------
// <resumen>
// tratar con la instancia de jugador en red
// </resumen>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.UI;

namespace Juego.unity
{
    #pragma warning disable 649

    /// <resumen>
    /// Administrador de jugadores.
    /// Maneja la entrada de fuego y las vigas.
    /// </resumen>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("La salud actual de nuestro jugador.")]
        public float Health = 1f;

        [Tooltip("La instancia de jugador local. Use esto para saber si el jugador local está representado en la escena")]
        public static GameObject LocalPlayerInstance;

        #endregion

        #region Private Fields

        [Tooltip("La interfaz de usuario del jugador Prefab GameObject")]
        [SerializeField]
        private GameObject playerUiPrefab;

        [Tooltip("The Beams GameObject para controlar")]
        [SerializeField]
        private GameObject beams;


        // Verdadero, cuando el usuario está disparando
        bool IsFiring;

        Text textoo;
        Text texto2;
        Text tiempo;
        Text Tiemporecord;
        GameObject panelfin;
        float tiempp;
        public int tiemp;
        public float contadorpanel;
        public int monedas;
        bool Activo;
        bool actmoneda1;
        bool actmoneda2;
        bool actmoneda3;
        bool actmoneda4;





        #endregion

        #region MonoBehaviour CallBacks

        /// <resumen>
        /// Método MonoBehaviour llamado en GameObject por Unity durante la fase de inicialización temprana.
        /// </resumen>
        public void Awake()
        {
            if (this.beams == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
            }
            else
            {
                this.beams.SetActive(false);
            }

            // # Importante
            // utilizado en GameManager.cs: hacemos un seguimiento de la instancia localPlayer para evitar la instanciación cuando los niveles están sincronizados
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            // # Crítico
            // marcamos como no destruir en carga para que la instancia sobreviva a la sincronización de nivel, lo que brinda una experiencia perfecta cuando los niveles se cargan.
            DontDestroyOnLoad(gameObject);
        }


        /// <resumen>
        /// Método MonoBehaviour llamado a GameObject por Unity durante la fase de inicialización.
        /// </resumen>
        public void Start()
        {
            contadorpanel = 10;
            monedas = 0;
            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            //Crea la UI
            if (this.playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif

            textoo = GameObject.FindWithTag("text").GetComponent<Text>();
            // aquí la referencia variable de myText al juego Objeto Texto coliscion
            texto2 = GameObject.FindWithTag("text2").GetComponent<Text>();
            
            tiempo = GameObject.FindWithTag("tiempo").GetComponent<Text>();
            
            Tiemporecord = GameObject.FindWithTag("tiempo2").GetComponent<Text>();
            Tiemporecord.text = "BEST:" + GetMaxScore().ToString();
            panelfin = GameObject.FindWithTag("panelfin");
            panelfin.SetActive(false);
            bool actmoneda1 = false;
            bool actmoneda2 = false;
            bool actmoneda3 = false;
            bool actmoneda4 = false;


        }


        public override void OnDisable()
		{

            // Siempre llame a la base para eliminar devoluciones de llamada

            base.OnDisable ();

			#if UNITY_5_4_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
			#endif
		}



        /// <resumen>
        /// Método MonoBehaviour llamado en GameObject por Unity en cada cuadro.
        /// Procesar entradas si jugador local.
        /// Mostrar y ocultar las vigas
        /// Esté atento al final del juego, cuando la salud del jugador local es 0.
        /// </resumen>
        public void Update()
        {
            // solo procesamos entradas y verificamos el estado si somos el jugador local
            if (photonView.IsMine)
            {
                this.ProcessInputs();

                if (this.Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                    
                }

                else
                {

                        Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
                
                
            }

            if (this.beams != null && this.IsFiring != this.beams.activeInHierarchy)
            {
                this.beams.SetActive(this.IsFiring);
            }


            
            if (photonView.IsMine)
            {
                //TIEMPO
                tiempo.text = tiemp.ToString();
                tiempp += Time.deltaTime;
                
                tiemp = Mathf.FloorToInt(tiempp);
                //PANEL FINAL 
                if (Activo == true)
                {
                    contadorpanel -=1f*Time.deltaTime;
                    tiempp += 0;
                    if(tiemp <= GetMaxScore())
                    {
                        Tiemporecord.text ="Best:" + tiemp.ToString();
                        SaveScore(tiemp);
                    }

                    if (contadorpanel <= 0)
                    {
                        panelfin.SetActive(false);
                        GameManager.Instance.LeaveRoom();
                    }
                }

                //ESTADO MONEDAS
                if (monedas == 1)
                {
                    texto2.text = "1 DE 4 Monedas";
                }
                if (monedas == 2)
                {
                    texto2.text = "2 DE 4 Monedas";
                }
                if (monedas == 3)
                {
                    texto2.text = "3 DE 4 Monedas";
                }
                if (monedas == 4)
                {
                    texto2.text = "4 DE 4, Encuentra la Capsula";
                }
            }
            


        }

        public void OnCollisionEnter(Collision col)
        {
            if (!photonView.IsMine)
            {
                return;

            }
            // ENEMIGOS
            if (col.collider.CompareTag("Respawn"))
            {
                this.Health -= 0.5f;
                textoo.text = "CORREEEE";
                Debug.Log("Collision con Enemigo");
            }

            if (col.collider.CompareTag("enemy"))
            {
                this.Health -= 0.10f;
                textoo.text = "CORREEEE";
                Debug.Log("Collision con Enemigo");
            }

            if (col.collider.CompareTag("enemy2"))
            {
                this.Health -= 0.3f;
                textoo.text = "CORREEEE";
                Debug.Log("Collision con Enemigo");
            }
            //VIDAA
            if (col.collider.CompareTag("Healt"))
            {
                this.Health += 0.1f;
                textoo.text = "VIDAA";
                Debug.Log("Collision con Vida");
            }
            //MONEDAS

            if (col.collider.CompareTag("moneda1"))
            {
                if(!actmoneda1 == true)
                {
                    monedas += 1;
                    actmoneda1 = true;
                    Debug.Log("Collision con modeda1");

                }
               
            }

            if (col.collider.CompareTag("moneda2"))
            {
                if (!actmoneda2 == true)
                {
                    monedas += 1;
                    actmoneda2 = true;
                    Debug.Log("Collision con modeda2");

                }
            }

            if (col.collider.CompareTag("moneda3"))
            {
                if (!actmoneda3 == true)
                {
                    monedas += 1;
                    actmoneda3 = true;
                    Debug.Log("Collision con modeda3");

                }
            }

            if (col.collider.CompareTag("moneda4"))
            {
                if (!actmoneda4 == true)
                {
                    monedas += 1;
                    actmoneda4 = true;
                    Debug.Log("Collision con modeda4");

                }
            }



            //FIN
            if (col.collider.CompareTag("fin"))
            {
                if(monedas == 4)
                {
                

                   panelfin.SetActive(true);
                   Activo = true;
                  Debug.Log("Has ganado");
                }
            }




        }

        public void OnCollisionStay(Collision col)
        {
            if (!photonView.IsMine)
            {
                return;

            }

            if (col.collider.CompareTag("Respawn"))
            {
                this.Health -= 0.1f*Time.deltaTime;
                textoo.text = "CORREEEE";
                Debug.Log("Collision estatica con el Eenemigo");
            
            }

            if (col.collider.CompareTag("enemy"))
            {
                this.Health -= 0.5f*Time.deltaTime;
                textoo.text = "CORREEEE";
                Debug.Log("Collision estatica con el Eenemigo");

            }

            if (col.collider.CompareTag("enemy2"))
            {
                this.Health -= 0.2f*Time.deltaTime;
                textoo.text = "CORREEEE";
                Debug.Log("Collision estatica con el Eenemigo");

            }

            if (col.collider.CompareTag("Healt"))
            {
                this.Health += 0.3f * Time.deltaTime;
                textoo.text = "VIDAAA";
                Debug.Log("Collision con Vida");
            }


        }

        /// <resumen>
        /// Se llama al método MonoBehaviour cuando el Collider 'otro' entra en el disparador.
        /// Afecta la salud del jugador si el colisionador es un rayo
        /// Nota: cuando saltas y disparas al mismo tiempo, encontrarás que la viga del jugador se cruza consigo misma
        /// Uno podría mover el colisionador más lejos para evitar esto o verificar si el rayo pertenece al jugador.
        /// </resumen>
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;

            }



            // Solo estamos interesados ​​en Beamers
            // deberíamos estar usando etiquetas pero por el bien de la distribución, simplemente verifiquemos por nombre.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            this.Health -= 0.1f;
        }


        /// <resumen>
        /// Se llama al método MonoBehaviour una vez por cuadro por cada 'otro' Collider que toca el gatillo.
        /// Vamos a afectar la salud mientras los rayos sean interesantes para el jugador
        /// </resumen>
        /// <param name="other">Other.</param>
        public void OnTriggerStay(Collider other)
        {

            // no hacemos nada si no somos el jugador local.
            if (!photonView.IsMine)
            {
                return;
            }


            // Solo estamos interesados ​​en Beamers
            // deberíamos estar usando etiquetas pero por el bien de la distribución, simplemente verifiquemos por nombre.
            if (!other.name.Contains("Beam"))
            {
                return;
            }


            // Lentamente afectamos la salud cuando el rayo nos golpea constantemente, por lo que el jugador tiene que moverse para evitar la muerte.
            this.Health -= 0.1f*Time.deltaTime;

            
        }


        #if !UNITY_5_4_OR_NEWER
        /// <resumen>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</resumen>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif


        /// <resumen>
        /// Método MonoBehaviour llamado después de cargar un nuevo nivel de índice 'nivel'.
        /// Recreamos la IU del jugador porque se destruyó cuando cambiamos de nivel.
        /// También reposiciona al jugador si está fuera de la arena actual.
        /// </resumen>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // verifica si estamos fuera de la Arena y si es el caso, genera alrededor del centro de la arena en una zona segura
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }


        public int GetMaxScore()
        {
            return PlayerPrefs.GetInt("tiemp", tiemp);
        }

        public void SaveScore(int tiempoo)
        {
            PlayerPrefs.SetInt("tiemp", tiempoo);
        }




        #endregion

        #region Private Methods


        #if UNITY_5_4_OR_NEWER
		void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
		{
			this.CalledOnLevelWasLoaded(scene.buildIndex);
		}
        #endif

        /// <resumen>
        /// Procesa las entradas. Esto DEBE SOLO UTILIZARSE cuando el jugador tiene autoridad sobre este GameObject en red (photonView.isMine == true)
        /// </resumen>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // no queremos disparar cuando interactuamos con botones de IU, por ejemplo. IsPointerOverGameObject realmente significa IsPointerOver * UI * GameObject
                // observe que no usamos en GetbuttonUp () algunas líneas hacia abajo, porque se puede mover el mouse hacia abajo, moverse sobre un elemento de la interfaz de usuario y soltar, lo que conduciría a no bajar el indicador isFiring.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //	return;
                }

                if (!this.IsFiring)
                {
                    this.IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (this.IsFiring)
                {
                    this.IsFiring = false;
                }
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Somos dueños de este reproductor: envía a los demás nuestros datos
                stream.SendNext(this.IsFiring);
                stream.SendNext(this.Health);
            }
            else
            {

                // Reproductor de red, recibe datos
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}