// --------------------------------------------------------------------------------------------------------------------


// <resumen>
//  tratar con los controles del componente Animator del reproductor en red.
// </resumen>

// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using Photon.Pun;


namespace Juego.unity
{
	public class PlayerAnimatorManager : MonoBehaviourPun 
	{
        #region Private Fields

        [SerializeField]
	    private float directionDampTime = 0.25f;
        Animator animator;

        #endregion

        #region MonoBehaviour CallBacks


        /// <resumen>
        /// M�todo MonoBehaviour llamado a GameObject por Unity durante la fase de inicializaci�n.
        /// </resumen>
        void Start () 
	    {
	        animator = GetComponent<Animator>();
	    }


        /// <resumen>
        /// M�todo MonoBehaviour llamado en GameObject por Unity en cada cuadro.
        /// </resumen>
        void Update () 
	    {


            // El control de prevenci�n est� conectado a Photon y representa al jugador local
            if ( photonView.IsMine == false && PhotonNetwork.IsConnected == true )
	        {
	            return;
	        }


            // failSafe no tiene el componente Animator en GameObject
            if (!animator)
	        {
				return;
			}


            // lidiar con el salto
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // solo permitimos saltar si estamos corriendo.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // Cuando se usa el par�metro disparador
                if (Input.GetButtonDown("Fire2")) animator.SetTrigger("Jump"); 
			}


            // lidiar con el movimiento
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");


            // evita la velocidad negativa.
            if ( v < 0 )
            {
                v = 0;
            }


            // establece los par�metros de Animator
            animator.SetFloat( "Speed", h*h+v*v );
            animator.SetFloat( "Direction", h, directionDampTime, Time.deltaTime );
	    }

		#endregion

	}
}