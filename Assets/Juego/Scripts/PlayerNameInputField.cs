// --------------------------------------------------------------------------------------------------------------------

// <resumen>
// Deje que el jugador ingrese su nombre para guardarlo como el nombre del jugador de la red, visto por todos los jugadores que se encuentren arriba de cada uno cuando estén en la misma habitación.
// </resuken>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Juego.unity
{

    /// <resumen>
    /// Campo de entrada del nombre del jugador. Deje que el usuario ingrese su nombre, aparecerá sobre el jugador en el juego.
    /// </resumen>
    [RequireComponent(typeof(InputField))]
	public class PlayerNameInputField : MonoBehaviour
	{
        #region Private Constants

        // Almacene la clave PlayerPref para evitar errores tipográficos
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        /// <resumen>
        /// Método MonoBehaviour llamado a GameObject por Unity durante la fase de inicialización.
        /// </resumen>
        void Start () {
		
			string defaultName = string.Empty;
			InputField _inputField = this.GetComponent<InputField>();

			if (_inputField!=null)
			{
				if (PlayerPrefs.HasKey(playerNamePrefKey))
				{
					defaultName = PlayerPrefs.GetString(playerNamePrefKey);
					_inputField.text = defaultName;
				}
			}

			PhotonNetwork.NickName =	defaultName;
		}

        #endregion

        #region Public Methods


        /// <resumen>
        /// Establece el nombre del jugador y guárdelo en PlayerPrefs para futuras sesiones.
        /// </resumen>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
		{
			// #Importante
		    if (string.IsNullOrEmpty(value))
		    {
                Debug.LogError("Player Name is null or empty");
		        return;
		    }
			PhotonNetwork.NickName = value;

			PlayerPrefs.SetString(playerNamePrefKey, value);
		}
		
		#endregion
	}
}
